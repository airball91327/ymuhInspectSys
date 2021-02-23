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
using InspectSystem.Models.DEquipment;
using PagedList;
using WebMatrix.WebData;
using InspectSystem.Filters;

namespace InspectSystem.Controllers
{
    public class DEInspectDocController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;

        // GET: DEInspectDoc
        public async Task<ActionResult> Index()
        {
            // Get cycle list.
            var inspectAreas = db.DEInspectArea.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem() { Value = "", Text = "所有" });
            foreach (var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");
            // Get cycle list.
            var inspectCycles = db.DEInspectCycle.ToList();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem(){ Value = "", Text = "所有"} );
            foreach (var item in inspectCycles)
            {
                listItem2.Add(new SelectListItem()
                {
                    Value = item.CycleId.ToString(),
                    Text = item.CycleName
                });
            }
            ViewData["CycleId"] = new SelectList(listItem2, "Value", "Text");
            //
            return View();
        }

        // GET: DEInspectDoc
        public ActionResult Index2(InspectDocQryVModel qry, int page = 1)
        {
            // query variables.
            string docid = qry.DocId;
            string shiftId = qry.ShiftId;

            // Get all inspect docs.
            var qryDocIdTable = db.DEInspectDoc.Include(d => d.InspectDoc);
            var inspectShifts = db.InspectShift.AsQueryable();
            var inspectFlows = db.InspectDocFlow.AsQueryable();
            // Get user's docs.
            inspectFlows = inspectFlows.Where(df => df.FlowStatusId == "?" && df.UserId == WebSecurity.CurrentUserId);
            inspectFlows = inspectFlows.Join(qryDocIdTable, f => f.DocId, d => d.DocId,
                                       (f, d) => new
                                       {
                                           docidTable = d,
                                           flow = f
                                       }).Where(d => d.docidTable.DocStatusId != "2").Select(d => d.flow);
            var DEInspectDoc = inspectFlows.Join(qryDocIdTable, f => f.DocId, d => d.DocId, (f, d) => d).ToList();
            //Get Shifting docs.
            var shiftingDoc = db.DEInspectDoc.Include(d => d.InspectDoc).Where(i => i.DocStatusId == "2")
                                                  .Join(db.InspectDoc, dt => dt.DocId, d => d.DocId,
                                                  (dt, d) => new
                                                  {
                                                      docIdTable = dt,
                                                      doc = d
                                                  })
                                                  .Where(r => r.doc.ShiftId == r.docIdTable.ShiftId)
                                                  .Where(r => r.doc.CheckerId == null || r.doc.CheckerId == WebSecurity.CurrentUserId)
                                                  .Select(r => r.docIdTable).ToList();
            DEInspectDoc.AddRange(shiftingDoc);
            DEInspectDoc = DEInspectDoc.GroupBy(d => d.DocId).Select(d => d.FirstOrDefault()).ToList();
            // query conditions.
            if (!string.IsNullOrEmpty(docid))   //案件編號(關鍵字)
            {
                docid = docid.Trim();
                DEInspectDoc = DEInspectDoc.Where(d => d.DocId.Contains(docid)).ToList();
            }
            if (!string.IsNullOrEmpty(shiftId))    //目前班別
            {
                int sid = Convert.ToInt32(shiftId);
                DEInspectDoc = DEInspectDoc.Where(d => d.ShiftId == sid).ToList();
            }
            // 對應班別中文名稱、目前巡檢工程師
            foreach (var doc in DEInspectDoc)
            {
                var shift = inspectShifts.Where(s => s.ShiftId == doc.ShiftId).FirstOrDefault();
                if (shift != null)
                {
                    doc.ShiftName = shift.ShiftName;
                }
                var inspectDoc = db.InspectDoc.Find(doc.DocId, doc.ShiftId);
                if (inspectDoc != null)
                {
                    var user = db.AppUsers.Find(inspectDoc.EngId);
                    doc.EngUserName = user != null ? user.UserName : "";
                    doc.EngFullName = user != null ? user.FullName : "";
                }
            }
            //
            var pageCount = DEInspectDoc.ToPagedList(page, pageSize).PageCount;
            pageCount = pageCount == 0 ? 1 : pageCount; // If no page.
            if (DEInspectDoc.ToPagedList(page, pageSize).Count <= 0)  //If the page has no items.
                return PartialView("List", DEInspectDoc.ToPagedList(pageCount, pageSize));
            return PartialView("List", DEInspectDoc.ToPagedList(page, pageSize));
        }

        // GET: DEInspectDoc/Views/5
        public async Task<ActionResult> Views(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            //
            List<InspectDocDetail> dtlShifts = new List<InspectDocDetail>();
            var docDetails = db.InspectDocDetail.Where(d => d.DocId == DEInspectDoc.DocId).ToList();
            if (docDetails.Count() > 0)
            {
                var docDetailShifts = docDetails.GroupBy(t => t.ShiftId).Select(g => g.FirstOrDefault())
                                .OrderBy(d => d.ShiftId).ToList();
                ViewBag.AreaName = docDetails.First().AreaName;
                dtlShifts = docDetailShifts;
            }
            //
            var notes = new InspectDocController().GetDocNotes(id);
            if (notes != null)
            {
                ViewBag.Notes = notes;
            }
            return View(dtlShifts);
        }

        // GET: DEInspectDoc/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = db.DEInspectDoc.Find(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return PartialView(DEInspectDoc);
        }

        // GET: DEInspectDoc/Create
        public ActionResult Create()
        {
            // Get inspect area list.
            var inspectAreas = db.DEInspectArea.Where(a => a.AreaStatus == true).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var item in inspectAreas)
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
            DEInspectDoc inspectDoc = new DEInspectDoc();
            inspectDoc.ApplyDate = DateTime.Now;
            return View(inspectDoc);
        }

        // POST: DEInspectDoc/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyErrorHandler]
        public async Task<ActionResult> Create([Bind(Include = "ApplyDate,AreaId,AreaName")] DEInspectDoc DEInspectDoc)
        {
            AppUser user;
            if (ModelState.IsValid)
            {
                var areaId = DEInspectDoc.AreaId;
                var applyDate = DEInspectDoc.ApplyDate;
                var docId = GetDocId(applyDate);
                //
                var docExist = db.DEInspectDoc.Find(docId);
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
                    var firstShift = db.ShiftsInAreas.Where(s => s.AreaId == areaId).OrderBy(s => s.ShiftId).FirstOrDefault();
                    if (firstShift != null)
                    {
                        shiftId = firstShift.ShiftId;
                    }
                    DEInspectDoc.DocId = docId;
                    DEInspectDoc.DocStatusId = "1";
                    DEInspectDoc.ShiftId = shiftId;
                    db.DEInspectDoc.Add(DEInspectDoc);
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
                    foreach (var field in inspectFields)
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

                    //Create first Flow.
                    InspectDocFlow flow = new InspectDocFlow();
                    flow.DocId = docId;
                    flow.StepId = 1;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "1";  // 流程狀態"已處理"
                    flow.Rtp = WebSecurity.CurrentUserId;
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "申請人";
                    db.InspectDocFlow.Add(flow);

                    // Create next flow.
                    flow = new InspectDocFlow();
                    flow.DocId = docId;
                    flow.StepId = 2;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "?";  // 狀態"未處理"
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "巡檢工程師";
                    flow.Opinions = "【白天班】";
                    db.InspectDocFlow.Add(flow);
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

        // GET: DEInspectDoc/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            //
            List<InspectDocDetail> dtlShifts = new List<InspectDocDetail>();
            var docDetails = db.InspectDocDetail.Where(d => d.DocId == DEInspectDoc.DocId).ToList();
            if (docDetails.Count() > 0)
            {
                var docDetailShifts = docDetails.GroupBy(t => t.ShiftId).Select(g => g.FirstOrDefault())
                                .OrderBy(d => d.ShiftId).ToList();
                ViewBag.AreaName = docDetails.First().AreaName;
                dtlShifts = docDetailShifts;
            }
            //
            var notes = new InspectDocController().GetDocNotes(id);
            if (notes != null)
            {
                ViewBag.Notes = notes;
            }
            return View(dtlShifts);
        }

        /// <summary>
        /// Get DEInspect DocId.
        /// </summary>
        /// <returns></returns>
        private string GetDocId(DateTime ApplyDate)
        {
            string docId = null;
            int yymmdd = (ApplyDate.Year - 1911) * 10000 + (ApplyDate.Month) * 100 + ApplyDate.Day;
            docId = Convert.ToString(yymmdd * 100);

            return docId;
        }

        // GET: DEInspectDoc/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (dEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDoc);
        }

        // POST: DEInspectDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            db.DEInspectDoc.Remove(dEInspectDoc);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
