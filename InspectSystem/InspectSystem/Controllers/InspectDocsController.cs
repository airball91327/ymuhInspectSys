using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectDocsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocs
        public ActionResult Index()
        {
            var inspectDocs = db.InspectDocs.Include(i => i.InspectAreas).Include(i => i.InspectFlowStatusTable);
            var editingDocs = inspectDocs.Where(i => i.FlowStatusID == 3); // 編輯中文件
            return View(editingDocs.ToList());
        }

        // GET: InspectDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocs inspectDocs = db.InspectDocs.Find(id);
            if (inspectDocs == null)
            {
                return HttpNotFound();
            }
            /* Find members of the area. */
            var workersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaID);
            var workerList = workersOfArea.Select(w => new
            {
                WorkerID = w.MemberId,
                WorkerName = w.InspectMembers.MemberName
            });
            ViewBag.WorkerID = new SelectList(workerList, "WorkerID", "WorkerName", inspectDocs.WorkerID);
            return View(inspectDocs);
        }

        // POST: InspectDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocID,Date,EndTime,AreaID,WorkerID,WorkerName,CheckerID,CheckerName,FlowStatusID")] InspectDocs inspectDocs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            /* Find members of the area. */
            var workersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaID);
            var workerList = workersOfArea.Select(w => new
            {
                WorkerID = w.MemberId,
                WorkerName = w.InspectMembers.MemberName
            });
            ViewBag.WorkerID = new SelectList(workerList, "WorkerID", "WorkerName", inspectDocs.WorkerID);
            return View(inspectDocs);
        }

        // GET: InspectDocs/Create
        public ActionResult Create()
        {
            /* Set default selected area and the members of area. */
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            var membersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == db.InspectAreas.FirstOrDefault().AreaID);
            var defaultMembers = membersOfArea.Select(m => new
            {
                MemberId = m.MemberId,
                MemberName = m.InspectMembers.MemberName
            });
            ViewBag.MemberID = new SelectList(defaultMembers, "MemberID", "MemberName");
            return View();
        }

        // POST: InspectDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InspectDocs inspectDocs)
        {
            /* Find data from DB and set to variables. */
            var workerID = System.Convert.ToInt32(Request.Form["MemberID"]);
            var findAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaID == inspectDocs.AreaID).First();
            var findWorkerName = db.InspectMembers.Find(workerID).MemberName;
            string date = inspectDocs.Date.ToString("yyyyMMdd");
            int docID = System.Convert.ToInt32(date) * 100 + inspectDocs.AreaID;

            /* Check doc is exist or not. */
            var isDocExist = db.InspectDocs.Find(docID);
            if(isDocExist == null)
            {
                /* Set doc details.*/
                inspectDocs.DocID = docID;
                inspectDocs.AreaName = db.InspectAreas.Find(inspectDocs.AreaID).AreaName;
                inspectDocs.WorkerID = workerID;
                inspectDocs.WorkerName = findWorkerName;
                inspectDocs.CheckerID = findAreaChecker.CheckerID;
                inspectDocs.CheckerName = findAreaChecker.CheckerName;
                inspectDocs.FlowStatusID = 3;        // Default flow status:"編輯中"

                if (ModelState.IsValid)
                {
                    db.InspectDocs.Add(inspectDocs);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("","已有相同文件存在!");
            }           

            /* Set default selected area and the members of area. */
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            var membersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaID);
            var defaultMembers = membersOfArea.Select(m => new
            {
                MemberId = m.MemberId,
                MemberName = m.InspectMembers.MemberName
            });
            ViewBag.MemberID = new SelectList(defaultMembers, "MemberID", "MemberName");
            return View(inspectDocs);
        }

        // GET: InspectDocs/GetMembers
        public JsonResult GetMembers(int areaId)
        {
            var membersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == areaId);
            var members = membersOfArea.Select(m => new
            {
                MemberId = m.MemberId,
                MemberName = m.InspectMembers.MemberName
            });
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        //// GET: InspectDocs1/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    InspectDocs inspectDocs = db.InspectDocs.Find(id);
        //    if (inspectDocs == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(inspectDocs);
        //}

        //// POST: InspectDocs1/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    InspectDocs inspectDocs = db.InspectDocs.Find(id);
        //    db.InspectDocs.Remove(inspectDocs);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
