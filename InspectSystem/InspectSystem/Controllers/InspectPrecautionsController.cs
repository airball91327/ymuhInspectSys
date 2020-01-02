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
    public class InspectPrecautionsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectPrecautions
        public ActionResult Index()
        {
            var inspectPrecautions = db.InspectPrecautions.Include(i => i.InspectAreas)
                                                          .OrderBy(i => i.AreaID);
            return View(inspectPrecautions.ToList());
        }

        // GET: InspectPrecautions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = db.InspectPrecautions.Find(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // GET: InspectPrecautions/Create
        public ActionResult Create()
        {
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            return View();
        }

        // POST: InspectPrecautions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrecautionID,AreaID,Content")] InspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.InspectPrecautions.Add(inspectPrecautions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectPrecautions.AreaID);
            return View(inspectPrecautions);
        }

        // GET: InspectPrecautions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = db.InspectPrecautions.Find(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectPrecautions.AreaID);
            return View(inspectPrecautions);
        }

        // POST: InspectPrecautions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrecautionID,AreaID,Content")] InspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectPrecautions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectPrecautions.AreaID);
            return View(inspectPrecautions);
        }

        // GET: InspectPrecautions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = db.InspectPrecautions.Find(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // POST: InspectPrecautions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectPrecautions inspectPrecautions = db.InspectPrecautions.Find(id);
            db.InspectPrecautions.Remove(inspectPrecautions);
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
