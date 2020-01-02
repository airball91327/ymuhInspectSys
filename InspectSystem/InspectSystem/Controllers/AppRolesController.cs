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
    [Authorize]
    public class AppRolesController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AppRoles
        public ActionResult Index()
        {
            return View(db.AppRoles.ToList());
        }

        // GET: MedEngMgt/AppRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppRoles appRoles = db.AppRoles.Find(id);
            if (appRoles == null)
            {
                return HttpNotFound();
            }
            return View(appRoles);
        }

        // GET: MedEngMgt/AppRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/AppRoles/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoleId,RoleName,Description")] AppRoles appRoles)
        {
            if (ModelState.IsValid)
            {
                db.AppRoles.Add(appRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appRoles);
        }

        // GET: MedEngMgt/AppRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppRoles appRoles = db.AppRoles.Find(id);
            if (appRoles == null)
            {
                return HttpNotFound();
            }
            return View(appRoles);
        }

        // POST: MedEngMgt/AppRoles/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoleId,RoleName,Description")] AppRoles appRoles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appRoles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appRoles);
        }

        // GET: MedEngMgt/AppRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppRoles appRoles = db.AppRoles.Find(id);
            if (appRoles == null)
            {
                return HttpNotFound();
            }
            return View(appRoles);
        }

        // POST: MedEngMgt/AppRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppRoles appRoles = db.AppRoles.Find(id);
            db.AppRoles.Remove(appRoles);
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
