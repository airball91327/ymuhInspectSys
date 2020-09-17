using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;
using WebMatrix.WebData;

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectAreasController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectAreas
        public ActionResult Index()
        {
            return View(db.InspectAreas.ToList());
        }

        // GET: InspectAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectArea inspectAreas = db.InspectAreas.Find(id);
            if (inspectAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectAreas);
        }

        // GET: InspectAreas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InspectAreas/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AreaId,AreaName,CheckerId,CheckerName")] InspectArea inspectAreas)
        {
            if (ModelState.IsValid)
            {
                inspectAreas.Rtp = WebSecurity.CurrentUserId;
                inspectAreas.Rtt = DateTime.Now;
                inspectAreas.Status = "Y";
                db.InspectAreas.Add(inspectAreas);
                db.SaveChanges();

                var getAreaId = db.InspectAreas.ToList().Last().AreaId;
                // Insert default checker for the new area.
                InspectAreaChecker inspectAreaChecker = new InspectAreaChecker()
                {
                    AreaId = getAreaId,
                    CheckerId = inspectAreas.CheckerId,
                    CheckerName = inspectAreas.CheckerName,
                    AreaCheckerId = (inspectAreas.CheckerId) * 100 + getAreaId
                };
                db.InspectAreaCheckers.Add(inspectAreaChecker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inspectAreas);
        }

        // GET: InspectAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectArea inspectAreas = db.InspectAreas.Find(id);
            if (inspectAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectAreas);
        }

        // POST: InspectAreas/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AreaId,AreaName,Status")] InspectArea inspectAreas)
        {
            if (ModelState.IsValid)
            {
                inspectAreas.Rtp = WebSecurity.CurrentUserId;
                inspectAreas.Rtt = DateTime.Now;
                db.Entry(inspectAreas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inspectAreas);
        }

        // GET: InspectAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectArea inspectAreas = db.InspectAreas.Find(id);
            if (inspectAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectAreas);
        }

        // POST: InspectAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectArea inspectAreas = db.InspectAreas.Find(id);
            db.InspectAreas.Remove(inspectAreas);
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
