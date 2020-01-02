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
    public class InspectAreaCheckersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectAreaCheckers
        public ActionResult Index()
        {
            var inspectAreaCheckers = db.InspectAreaCheckers.Include(i => i.InspectAreas).OrderBy(i => i.AreaID);
            return View(inspectAreaCheckers.ToList());
        }

        // GET: InspectAreaCheckers/Edit/5
        public ActionResult Edit(int? areaCheckerID)
        {
            if (areaCheckerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerID);
            if (inspectAreaChecker == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
            return View(inspectAreaChecker);
        }

        // POST: InspectAreaCheckers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AreaCheckerID,AreaID,CheckerID,CheckerName,Email")] InspectAreaChecker inspectAreaChecker)
        {
            if (ModelState.IsValid)
            {
                var newAreaCheckerID = (inspectAreaChecker.CheckerID) * 100 +(inspectAreaChecker.AreaID);
                if( newAreaCheckerID == inspectAreaChecker.AreaCheckerID ) // Did not change area or checker.
                {
                    db.Entry(inspectAreaChecker).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var isNewIdExist = db.InspectAreaCheckers.Find(newAreaCheckerID);
                    if(isNewIdExist == null)
                    {
                        db.Entry(inspectAreaChecker).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "此項資料已存在");
                        ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
                        return View(inspectAreaChecker);
                    }
                }
                
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
            return View(inspectAreaChecker);
        }

        // GET: InspectAreaCheckers/Create
        public ActionResult Create()
        {
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            return View();
        }

        // POST: InspectAreaCheckers/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AreaCheckerID,AreaID,CheckerID,CheckerName,Email")] InspectAreaChecker inspectAreaChecker)
        {
            inspectAreaChecker.AreaCheckerID = (inspectAreaChecker.CheckerID) * 100 + (inspectAreaChecker.AreaID);
            if (ModelState.IsValid)
            {
                var isDataExist = db.InspectAreaCheckers.Find(inspectAreaChecker.AreaCheckerID);
                if( isDataExist != null )
                {
                    ModelState.AddModelError("", "此項資料已存在");
                    ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
                    return View(inspectAreaChecker);
                }
                db.InspectAreaCheckers.Add(inspectAreaChecker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaID);
            return View(inspectAreaChecker);
        }

        // GET: InspectAreaCheckers/Delete/5
        public ActionResult Delete(int? areaCheckerID)
        {
            if (areaCheckerID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerID);
            if (inspectAreaChecker == null)
            {
                return HttpNotFound();
            }
            return View(inspectAreaChecker);
        }

        // POST: InspectAreaCheckers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? areaCheckerID)
        {
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerID);
            var isLastAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaID == inspectAreaChecker.AreaID).Count();
            if(isLastAreaChecker <= 1)
            {
                ModelState.AddModelError("", "區域唯一的主管無法刪除");
                return View(inspectAreaChecker);
            }
            db.InspectAreaCheckers.Remove(inspectAreaChecker);
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
