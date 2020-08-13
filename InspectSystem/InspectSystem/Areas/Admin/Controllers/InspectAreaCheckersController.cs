using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using InspectSystem.Models;
using WebMatrix.WebData;

namespace InspectSystem.Areas.Admin.Controllers
{
    public class InspectAreaCheckersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectAreaCheckers
        public ActionResult Index()
        {
            var inspectAreaCheckers = db.InspectAreaCheckers.Include(i => i.InspectAreas).OrderBy(i => i.AreaId);
            return View(inspectAreaCheckers.ToList());
        }

        // GET: Admin/InspectAreaCheckers/Edit/5
        public ActionResult Edit(int? areaCheckerId)
        {
            if (areaCheckerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerId);
            if (inspectAreaChecker == null)
            {
                return HttpNotFound();
            }
            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            AppUser u;
            s = Roles.GetUsersInRole("MedMgr").ToList();
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (!string.IsNullOrEmpty(u.DptId))
                {
                    li = new ListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["CheckerId"] = new SelectList(list, "Value", "Text", "");
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName", inspectAreaChecker.AreaId);
            return View(inspectAreaChecker);
        }

        // POST: Admin/InspectAreaCheckers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AreaCheckerId,AreaId,CheckerId,CheckerName,Email")] InspectAreaChecker inspectAreaChecker)
        {
            if (ModelState.IsValid)
            {
                var newAreaCheckerId = (inspectAreaChecker.CheckerId) * 100 + (inspectAreaChecker.AreaId);
                if (newAreaCheckerId == inspectAreaChecker.AreaCheckerId) // Did not change area or checker.
                {
                    db.Entry(inspectAreaChecker).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var isNewIdExist = db.InspectAreaCheckers.Find(newAreaCheckerId);
                    if (isNewIdExist == null)
                    {
                        db.Entry(inspectAreaChecker).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "此項資料已存在!");
                        ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName", inspectAreaChecker.AreaId);
                        return View(inspectAreaChecker);
                    }
                }

            }
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName", inspectAreaChecker.AreaId);
            return View(inspectAreaChecker);
        }

        // GET: Admin/InspectAreaCheckers/Create
        public ActionResult Create()
        {
            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            AppUser u;
            s = Roles.GetUsersInRole("MedMgr").ToList();
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (!string.IsNullOrEmpty(u.DptId))
                {
                    li = new ListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["CheckerId"] = new SelectList(list, "Value", "Text", list.First().Value);
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName");
            return View();
        }

        // POST: Admin/InspectAreaCheckers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AreaCheckerId,AreaId,CheckerId,CheckerName,Email")] InspectAreaChecker inspectAreaChecker)
        {
            inspectAreaChecker.AreaCheckerId = (inspectAreaChecker.CheckerId) * 100 + (inspectAreaChecker.AreaId);
            if (ModelState.IsValid)
            {
                var isDataExist = db.InspectAreaCheckers.Find(inspectAreaChecker.AreaCheckerId);
                if (isDataExist != null)
                {
                    ModelState.AddModelError("", "此項資料已存在!");
                    ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName", inspectAreaChecker.AreaId);
                    return View(inspectAreaChecker);
                }
                db.InspectAreaCheckers.Add(inspectAreaChecker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.InspectAreas, "AreaId", "AreaName", inspectAreaChecker.AreaId);
            return View(inspectAreaChecker);
        }

        // GET: Admin/InspectAreaCheckers/Delete/5
        public ActionResult Delete(int? areaCheckerId)
        {
            if (areaCheckerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerId);
            if (inspectAreaChecker == null)
            {
                return HttpNotFound();
            }
            return View(inspectAreaChecker);
        }

        // POST: Admin/InspectAreaCheckers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? areaCheckerId)
        {
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerId);
            var isLastAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaId == inspectAreaChecker.AreaId).Count();
            if (isLastAreaChecker <= 1)
            {
                ModelState.AddModelError("", "區域唯一的主管無法刪除!");
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