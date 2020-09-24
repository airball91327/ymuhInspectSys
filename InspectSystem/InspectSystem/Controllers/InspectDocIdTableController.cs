using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;
using WebMatrix.WebData;
using InspectSystem.Filters;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocIdTableController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocIdTable
        public async Task<ActionResult> Index()
        {
            return View(await db.InspectDocIdTable.ToListAsync());
        }

        // GET: InspectDocIdTable/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Create
        public ActionResult Create()
        {
            // Get inspect area list.
            var inspectAreas = db.InspectArea.Where(a => a.AreaStatus == true).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach(var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");
            //
            // Set default value.
            InspectDocIdTable insp = new InspectDocIdTable();
            insp.ApplyDate = DateTime.Now;
            return View(insp);
        }

        // POST: InspectDocIdTable/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyErrorHandler]
        public async Task<ActionResult> Create([Bind(Include = "ApplyDate,AreaId,AreaName")] InspectDocIdTable inspectDocIdTable)
        {
            AppUser user;
            if (ModelState.IsValid)
            {
                var areaId = inspectDocIdTable.AreaId;
                var applyDate = inspectDocIdTable.ApplyDate;
                var docId = GetDocId(areaId, applyDate);
                //
                var docExist = db.InspectDocIdTable.Find(docId);
                if (docExist != null)
                {
                    return new JsonResult
                    {
                        Data = new { success = false, error = "已申請過巡檢單!" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                //
                try
                {
                    var shiftId = 1;
                    inspectDocIdTable.DocId = docId;
                    inspectDocIdTable.DocStatusId = "1";
                    inspectDocIdTable.ShiftId = shiftId;
                    db.InspectDocIdTable.Add(inspectDocIdTable);
                    await db.SaveChangesAsync();
                    // 產生白天班巡檢單
                    user = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    InspectDoc inspectDoc = new InspectDoc();
                    inspectDoc.DocId = docId;
                    inspectDoc.ShiftId = shiftId;
                    inspectDoc.ApplyDate = applyDate;
                    inspectDoc.EngId = user.Id;
                    inspectDoc.EngName = user.FullName;
                    db.InspectDoc.Add(inspectDoc);
                    await db.SaveChangesAsync();
                    // 擷取巡檢單內容
                    ShiftsInAreas shiftsInAreas = db.ShiftsInAreas.Include(i => i.InspectArea).Include(i => i.InspectShift)
                                                                  .Where(i => i.AreaId == areaId && i.ShiftId == shiftId).FirstOrDefault();
                    List<InspectClass> inspectClasses = db.InspectClass.Where(i => i.AreaId == areaId && i.ShiftId == shiftId).ToList();
                    List<InspectItem> inspectItems = db.InspectItem.Where(i => i.AreaId == areaId && i.ShiftId == shiftId).ToList();
                    List<InspectField> inspectFields = db.InspectField.Include(i => i.InspectItem).Include(i => i.InspectItem.InspectClass)
                                                                      .Where(i => i.AreaId == areaId && i.ShiftId == shiftId)
                                                                      .Where(i => i.InspectItem.InspectClass.ClassStatus == true)
                                                                      .Where(i => i.InspectItem.ItemStatus == true)
                                                                      .Where(i => i.FieldStatus == true).ToList();
                    // Insert values.
                    InspectDocDetailTemp detailTemp;
                    foreach(var field in inspectFields)
                    {
                        detailTemp = new InspectDocDetailTemp();
                        detailTemp.DocId = docId;
                        detailTemp.AreaId = field.AreaId;
                        detailTemp.AreaName = shiftsInAreas.InspectArea.AreaName;
                        detailTemp.ShiftId = field.ShiftId;
                        detailTemp.ShiftName = shiftsInAreas.InspectShift.ShiftName;
                        detailTemp.ClassId = field.ClassId;
                        detailTemp.ClassName = field.InspectItem.InspectClass.ClassName;
                        detailTemp.ClassOrder = field.InspectItem.InspectClass.ClassOrder;
                        detailTemp.ItemId = field.ItemId;
                        detailTemp.ItemName = field.InspectItem.ItemName;
                        detailTemp.ItemOrder = field.InspectItem.ItemOrder;
                        detailTemp.FieldId = field.FieldId;
                        detailTemp.FieldName = field.FieldName;
                        detailTemp.DataType = field.DataType;
                        detailTemp.UnitOfData = field.UnitOfData;
                        detailTemp.MinValue = field.MinValue;
                        detailTemp.MaxValue = field.MaxValue;
                        detailTemp.IsRequired = field.IsRequired;
                        /* If field is dropdown, set dropdownlist items to string and save to DB. */
                        if (field.DataType == "dropdownlist")
                        {
                            var itemDropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == field.AreaId &&
                                                                                      i.ShiftId == field.ShiftId &&
                                                                                      i.ClassId == field.ClassId &&
                                                                                      i.ItemId == field.ItemId &&
                                                                                      i.FieldId == field.FieldId).ToList();
                            foreach (var dropItem in itemDropDownList)
                            {
                                detailTemp.DropDownItems += dropItem.Value.ToString() + ";";
                            }
                        }
                        //
                        db.InspectDocDetailTemp.Add(detailTemp);
                    }
                    await db.SaveChangesAsync();

                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                catch (Exception e)
                {
                    throw new Exception("申請失敗!");
                }
            }
            //
            return new JsonResult
            {
                Data = new { success = false, error = "申請失敗!" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: InspectDocIdTable/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // POST: InspectDocIdTable/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ApplyDate,CloseDate,AreaId,AreaName")] InspectDocIdTable inspectDocIdTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocIdTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // POST: InspectDocIdTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            db.InspectDocIdTable.Remove(inspectDocIdTable);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get Inspect DocId by areaId and applyDate.
        /// </summary>
        /// <returns></returns>
        private string GetDocId(int AreaId, DateTime ApplyDate)
        {
            string docId = null;
            int yymmdd = (ApplyDate.Year - 1911) * 10000 + (ApplyDate.Month) * 100 + ApplyDate.Day;
            docId = Convert.ToString(yymmdd * 100 + AreaId);

            return docId;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
