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

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DEInspectDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;

        // GET: Admin/DEInspectDocDetail
        public async Task<ActionResult> Index()
        {
            return View();
        }

        // GET: Admin/DEInspectDocDetail/Index2
        public ActionResult Index2(DEInspectDocQryVModel qry, int page = 1)
        {
            // query variables.
            string docid = qry.DocId;
            //
            var qryResult = db.DEInspectDoc.Join(db.AppUsers, f => f.EngId, u => u.Id,
                                        (f, u) => new
                                        {
                                            inspectDoc = f,
                                            eng = u
                                        });
            // query conditions.
            if (!string.IsNullOrEmpty(docid))   //案件編號
            {
                docid = docid.Trim();
                qryResult = qryResult.Where(d => d.inspectDoc.DocId == docid);
            }
            //
            List<DEInspectDocVModel> returnList = new List<DEInspectDocVModel>();
            DEInspectDocVModel docVModel;
            foreach (var item in qryResult.ToList())
            {
                docVModel = new DEInspectDocVModel();
                docVModel.ApplyDate = item.inspectDoc.ApplyDate;
                docVModel.AreaId = item.inspectDoc.AreaId;
                docVModel.AreaName = item.inspectDoc.AreaName;
                docVModel.CheckerId = item.inspectDoc.CheckerId;
                docVModel.CheckerName = item.inspectDoc.CheckerName;
                docVModel.ClassId = item.inspectDoc.ClassId;
                docVModel.ClassName = item.inspectDoc.ClassName;
                docVModel.CloseDate = item.inspectDoc.CloseDate;
                docVModel.CycleId = item.inspectDoc.CycleId;
                docVModel.CycleName = item.inspectDoc.CycleName;
                docVModel.DocId = item.inspectDoc.DocId;
                docVModel.EndTime = item.inspectDoc.EndTime;
                docVModel.EngId = item.inspectDoc.EngId;
                docVModel.EngName = item.inspectDoc.EngName;
                docVModel.EngUserName = item.eng.UserName;
                returnList.Add(docVModel);
            }
            var pageCount = returnList.ToPagedList(page, pageSize).PageCount;
            pageCount = pageCount == 0 ? 1 : pageCount; // If no page.
            if (returnList.ToPagedList(page, pageSize).Count <= 0)  //If the page has no items.
                return PartialView("List", returnList.ToPagedList(pageCount, pageSize));
            return PartialView("List", returnList.ToPagedList(page, pageSize));
        }

        // GET: Admin/DEInspectDocDetail/Edit/5
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
            // Insert values to classVModel.
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            classVModel.DocId = DEInspectDoc.DocId;
            classVModel.AreaId = DEInspectDoc.AreaId;
            classVModel.CycleId = DEInspectDoc.CycleId;
            classVModel.ClassId = DEInspectDoc.ClassId;
            //
            ViewBag.Header = DEInspectDoc.AreaName + "【" + DEInspectDoc.CycleName + "】" + "巡檢單";
            return View(classVModel);
        }

        // GET: Admin/DEInspectDocDetail/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDocDetail dEInspectDocDetail = await db.DEInspectDocDetail.FindAsync(id);
            if (dEInspectDocDetail == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDocDetail);
        }

        // POST: Admin/DEInspectDocDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DEInspectDocDetail dEInspectDocDetail = await db.DEInspectDocDetail.FindAsync(id);
            db.DEInspectDocDetail.Remove(dEInspectDocDetail);
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
