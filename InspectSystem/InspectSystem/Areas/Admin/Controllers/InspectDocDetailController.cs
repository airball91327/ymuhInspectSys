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
using X.PagedList;
using InspectSystem.Extensions;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;

        // GET: Admin/InspectDocDetail
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: Admin/InspectDocDetail/Index2
        public async Task<ActionResult> Index2(InspectDocQryVModel qry, int page = 1)
        {
            string docId = qry.DocId;
            //
            var inspectDocs = db.InspectDoc.Join(db.InspectShift, d => d.ShiftId, s => s.ShiftId,
                                           (d, s) => new 
                                           { 
                                               doc = d,
                                               shift = s
                                           }).AsQueryable();
            if (!string.IsNullOrEmpty(docId))
            {
                inspectDocs = inspectDocs.Where(d => d.doc.DocId == docId);
            }
            foreach(var item in inspectDocs)
            {
                item.doc.ShiftName = item.shift.ShiftName;
            }
            var returnList = inspectDocs.Select(d => d.doc).ToList();
            var pageCount = returnList.ToPagedList(page, pageSize).PageCount;
            pageCount = pageCount == 0 ? 1 : pageCount; // If no page.
            if (returnList.ToPagedList(page, pageSize).Count <= 0)  //If the page has no items.
                return PartialView("DocList", returnList.ToPagedList(pageCount, pageSize));
            return PartialView("DocList", returnList.ToPagedList(page, pageSize));
        }

        // GET: Admin/InspectDocDetail/Edit/5
        public async Task<ActionResult> Edit(string docid, string shiftid)
        {
            if (docid == null || shiftid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(docid);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            // Set variables.
            var shiftId = Convert.ToInt32(shiftid);
            var docStatusId = inspectDocIdTable.DocStatusId;
            var docDetails = db.InspectDocDetail.Where(d => d.DocId == docid && d.ShiftId == shiftId).ToList();
            var docDetailsClasses = docDetails.GroupBy(t => t.ClassId).Select(g => g.FirstOrDefault())
                                              .OrderBy(d => d.ClassOrder);
            var shiftName = "";
            if (docDetails.Count > 0)
            {
                shiftName = docDetails.First().ShiftName;
            }
            var areaName = inspectDocIdTable.AreaName;
            List<InspectClassVModel> inspectClassVs = new List<InspectClassVModel>();
            InspectClassVModel classVModel;
            //
            foreach (var item in docDetailsClasses)
            {
                // Get class error fields.
                var classErrors = docDetails.Where(d => d.ClassId == item.ClassId &&
                                                        d.IsFunctional == "N").ToList();
                // Get details of class.
                var findDocTemps = docDetails.Where(d => d.ClassId == item.ClassId);
                // Insert values to classVModel.
                classVModel = new InspectClassVModel();
                classVModel.DocId = item.DocId;
                classVModel.AreaId = item.AreaId;
                classVModel.ShiftId = item.ShiftId;
                classVModel.ClassId = item.ClassId;
                classVModel.ClassName = item.ClassName;
                classVModel.ClassOrder = item.ClassOrder;
                classVModel.IsSaved = true;
                classVModel.CountErrors = classErrors.Count();
                inspectClassVs.Add(classVModel);
            }
            //
            var notes = new InspectSystem.Controllers.InspectDocController().GetDocNotes(docid);
            if (notes != null)
            {
                ViewBag.Notes = notes;
            }
            //
            ViewBag.Header = areaName + "【" + shiftName + "】";
            return View(inspectClassVs);
        }

        // Get: Admin/InspectDocDetail/GetClassContentEdit/5
        public ActionResult GetClassContentEdit(string docId, string shiftId, string classId)
        {
            int iShiftId = Convert.ToInt32(shiftId);
            int iClassId = Convert.ToInt32(classId);
            // Get inspect DocDetailTemp list.
            var docDetail = db.InspectDocDetail.Where(t => t.DocId == docId && t.ShiftId == iShiftId &&
                                                           t.ClassId == iClassId).ToList();
            if (docDetail.Count() > 0)
            {
                ViewBag.ClassName = docDetail.First().ClassName;
                // Get items and fields from DocDetailTemp list. 
                ViewData["itemsByDocDetails"] = docDetail.GroupBy(i => i.ItemId)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
                ViewData["fieldsByDocDetails"] = docDetail.ToList();
            }

            InspectDocDetailViewModel inspectDocDetailViewModel = new InspectDocDetailViewModel()
            {
                InspectDocDetail = docDetail
            };
            return PartialView(inspectDocDetailViewModel);
        }


        // POST: Admin/InspectDocDetail/SaveDetails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDetails(List<InspectDocDetail> inspectDocDetail)
        {
            try
            {
                foreach (var item in inspectDocDetail)
                {
                    InspectDocDetail oriObj = db.InspectDocDetail.Find(item.DocId, item.ShiftId, item.ClassId, item.ItemId, item.FieldId);
                    db.Entry(oriObj).State = EntityState.Detached;
                    //
                    db.Entry(item).State = EntityState.Modified;
                    // Save log.
                    var logAction2 = oriObj.EnumeratePropertyDifferences<InspectDocDetail>(item);
                    if (logAction2.Count() > 0)
                    {
                        string logClass = "巡檢系統";
                        string logAction = "一般巡檢 > 後台數值編輯 > " + "(" + item.DocId + ")" + " ";
                        var result = new SystemLogsController().SaveLog(logClass, logAction, logAction2);
                    }
                }
                db.SaveChanges();

                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
            catch (Exception e)
            {
                return new JsonResult
                {
                    Data = new { success = false, error = "儲存失敗!" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
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
