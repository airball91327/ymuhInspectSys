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
    public class InspectMembersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectMembers
        public ActionResult Index()
        {
            var inspectMemberAreas = db.InspectMemberAreas.Include(i => i.InspectAreas)
                                                          .Include(i => i.InspectMembers)
                                                          .OrderBy(i => i.AreaId);
            var inspectMembers = db.InspectMembers.OrderBy(i => i.Department);
            InspectMembersViewModel inspectMembersViewModel = new InspectMembersViewModel()
            {
                InspectMemberAreas = inspectMemberAreas.ToList(),
                InspectMembers = inspectMembers.ToList()
            };
            ViewBag.CountAreas = db.InspectAreas.Count();
            return View(inspectMembersViewModel);
        }

        // GET: InspectMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Create/5
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /* New a InspectMemberAreas. */
            InspectMemberAreas inspectMemberAreas = new InspectMemberAreas();
            inspectMemberAreas.MemberId = db.InspectMembers.Find(id).MemberId;
            inspectMemberAreas.InspectMembers = db.InspectMembers.Find(id);

            /* Find the AreaIDs which is not already exist. */
            var q = ( from a in db.InspectAreas select a.AreaID )
                    .Except( from m in db.InspectMemberAreas where m.MemberId == id select m.AreaId );
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach(var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaID", "AreaName");
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas)
        {
            if (ModelState.IsValid)
            {
                db.InspectMemberAreas.Add(inspectMemberAreas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            /* Find the AreaIDs which is not already exist. */
            var id = inspectMemberAreas.MemberId;
            var q = (from a in db.InspectAreas select a.AreaID)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id select m.AreaId);
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaID", "AreaName");
            inspectMemberAreas.InspectMembers = db.InspectMembers.Find(id);
            return View(inspectMemberAreas);
        }

        // Get: InspectMembers/EditList/5
        public ActionResult EditList(int? id)
        {
            var inspectMemberAreas = db.InspectMemberAreas.Where(i => i.MemberId == id);
            if(inspectMemberAreas.Count() == 1)
            {
                return RedirectToAction("Edit", new { id = id, areaID = inspectMemberAreas.First().AreaId });
            }
            return View(inspectMemberAreas.ToList());
        }

        // GET: InspectMembers/Edit/5
        public ActionResult Edit(int? id, int? areaID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }

            /* Find the AreaIDs which is not already exist and selected. */
            var q = (from a in db.InspectAreas select a.AreaID)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id && 
                                                                  m.AreaId != areaID select m.AreaId);
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaID", "AreaName", areaID);
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas, int originAreaID)
        {
            if (ModelState.IsValid)
            {
                if(inspectMemberAreas.AreaId == originAreaID)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    /* Add a InspectMemberAreas. */
                    db.InspectMemberAreas.Add(inspectMemberAreas);
                    /* Delete old area. */
                    InspectMemberAreas deleteMemberAreas = db.InspectMemberAreas.Find(inspectMemberAreas.MemberId, originAreaID);
                    db.InspectMemberAreas.Remove(deleteMemberAreas);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            /* Find the AreaIDs which is not already exist. */
            var id = inspectMemberAreas.MemberId;
            int areaID = originAreaID;
            var q = (from a in db.InspectAreas select a.AreaID)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id &&
                                                                  m.AreaId != areaID select m.AreaId);
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaID", "AreaName", areaID);
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Delete/5
        public ActionResult Delete(int? id, int? areaID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int areaID)
        {
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            db.InspectMemberAreas.Remove(inspectMemberAreas);
            db.SaveChanges();
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
