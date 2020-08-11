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
            var inspectDocs = db.InspectDocs.Include(i => i.InspectFlowStatusTable);
            var editingDocs = inspectDocs.Where(i => i.FlowStatusId == 3); // 編輯中文件
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
            var workersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaId);
            var workerList = workersOfArea.Select(w => new
            {
                EngId = w.MemberId,
                EngName = w.InspectMembers.MemberName
            });
            ViewBag.EngId = new SelectList(workerList, "EngId", "EngName", inspectDocs.EngId);
            return View(inspectDocs);
        }

        // POST: InspectDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,Date,EndTime,AreaId,EngId,EngName,CheckerId,CheckerName,FlowStatusId")] InspectDocs inspectDocs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            /* Find members of the area. */
            var workersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaId);
            var workerList = workersOfArea.Select(w => new
            {
                EngId = w.MemberId,
                EngName = w.InspectMembers.MemberName
            });
            ViewBag.EngId = new SelectList(workerList, "EngId", "EngName", inspectDocs.EngId);
            return View(inspectDocs);
        }

        // GET: InspectDocs/Create
        public ActionResult Create()
        {
            /* Set default selected area and the members of area. */
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName");
            var membersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == db.InspectAreas.FirstOrDefault().AreaId);
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
            var EngId = System.Convert.ToInt32(Request.Form["MemberID"]);
            var findAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaId == inspectDocs.AreaId).First();
            var findEngName = db.InspectMembers.Find(EngId).MemberName;
            string date = inspectDocs.ApplyDate.ToString("yyyyMMdd");
            int DocId = System.Convert.ToInt32(date) * 100 + inspectDocs.AreaId;

            /* Check doc is exist or not. */
            var isDocExist = db.InspectDocs.Find(DocId);
            if(isDocExist == null)
            {
                /* Set doc details.*/
                inspectDocs.DocId = DocId;
                inspectDocs.AreaName = db.InspectAreas.Find(inspectDocs.AreaId).AreaName;
                inspectDocs.EngId = EngId;
                inspectDocs.EngName = findEngName;
                inspectDocs.CheckerId = findAreaChecker.CheckerId;
                inspectDocs.CheckerName = findAreaChecker.CheckerName;
                inspectDocs.FlowStatusId = 3;        // Default flow status:"編輯中"

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
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName");
            var membersOfArea = db.InspectMemberAreas.Where(i => i.AreaId == inspectDocs.AreaId);
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
