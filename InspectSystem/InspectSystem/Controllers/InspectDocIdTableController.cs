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
using System.Runtime.CompilerServices;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocIdTableController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocIdTable
        public async Task<ActionResult> Index()
        {
            // Get shift list.
            var inspectShifts = db.InspectShift.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var item in inspectShifts)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.ShiftId.ToString(),
                    Text = item.ShiftName
                });
            }
            ViewData["ShiftId"] = new SelectList(listItem, "Value", "Text");
            //
            return View();
        }

        // POST: InspectDocIdTable
        [HttpPost]
        [MyErrorHandler]
        public async Task<ActionResult> Index(InspectDocQryVModel qry)
        {
            // query variables.
            string docid = qry.DocId;
            string shiftId = qry.ShiftId;

            // Get all inspect docs.
            var inspectDocIdTable = await db.InspectDocIdTable.Include(d => d.InspectDoc).ToListAsync();
            var inspectShifts = await db.InspectShift.ToListAsync();
            var inspectFlows = await db.InspectDocFlow.ToListAsync();
            // Get user's docs.
            inspectFlows = inspectFlows.Where(df => df.FlowStatusId == "?" && df.UserId == WebSecurity.CurrentUserId).ToList();
            inspectDocIdTable = inspectFlows.Join(inspectDocIdTable, f => f.DocId, d => d.DocId, (f, d) => d).ToList();
            //Get Shifting docs.
            var shiftingDoc = db.InspectDocIdTable.Include(d => d.InspectDoc).Where(i => i.DocStatusId == "2")
                                                  .Join(db.InspectDoc, dt => dt.DocId, d => d.DocId, 
                                                  (dt, d) => new 
                                                  { 
                                                      docIdTable = dt,
                                                      doc = d
                                                  })
                                                  .Where(r => r.doc.CheckerId == null || r.doc.CheckerId == WebSecurity.CurrentUserId)
                                                  .Select(r => r.docIdTable).ToList();
            inspectDocIdTable.AddRange(shiftingDoc);
            inspectDocIdTable = inspectDocIdTable.GroupBy(d => d.DocId).Select(d => d.FirstOrDefault()).ToList();
            // query conditions.
            if (!string.IsNullOrEmpty(docid))   //案件編號(關鍵字)
            {
                docid = docid.Trim();
                inspectDocIdTable = inspectDocIdTable.Where(d => d.DocId.Contains(docid)).ToList();
            }
            if (!string.IsNullOrEmpty(shiftId))    //目前班別
            {
                int sid = Convert.ToInt32(shiftId);
                inspectDocIdTable = inspectDocIdTable.Where(d => d.ShiftId == sid).ToList();
            }
            // 對應班別中文名稱、目前巡檢工程師
            foreach (var doc in inspectDocIdTable)
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

            return PartialView("List", inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Views/5
        public async Task<ActionResult> Views(string id)
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
            //
            List<InspectDocDetail> dtlShifts = new List<InspectDocDetail>();
            var docDetails = db.InspectDocDetail.Where(d => d.DocId == inspectDocIdTable.DocId).ToList();
            if (docDetails.Count() > 0)
            {
                var docDetailShifts = docDetails.GroupBy(t => t.ShiftId).Select(g => g.FirstOrDefault())
                                .OrderBy(d => d.ShiftId).ToList();
                ViewBag.AreaName = docDetails.First().AreaName;
                dtlShifts = docDetailShifts;
            }

            return View(dtlShifts);
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
            //
            List<InspectDocDetail> dtlShifts = new List<InspectDocDetail>();
            var docDetails = db.InspectDocDetail.Where(d => d.DocId == inspectDocIdTable.DocId).ToList();
            if (docDetails.Count() > 0)
            {
                var docDetailShifts = docDetails.GroupBy(t => t.ShiftId).Select(g => g.FirstOrDefault())
                                .OrderBy(d => d.ShiftId).ToList();
                ViewBag.AreaName = docDetails.First().AreaName;
                dtlShifts = docDetailShifts;
            }
            return View(dtlShifts);
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
