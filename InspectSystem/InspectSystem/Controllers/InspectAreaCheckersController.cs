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

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectAreaCheckersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectAreaCheckers
        public ActionResult Index()
        {
            var inspectAreaCheckers = db.InspectAreaCheckers.Include(i => i.InspectAreas).OrderBy(i => i.AreaId);
            return View(inspectAreaCheckers.ToList());
        }

        // GET: InspectAreaCheckers/Edit/5
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
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaId);
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
                var newAreaCheckerID = (inspectAreaChecker.CheckerId) * 100 +(inspectAreaChecker.AreaId);
                if( newAreaCheckerID == inspectAreaChecker.AreaCheckerId ) // Did not change area or checker.
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
                        ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaId);
                        return View(inspectAreaChecker);
                    }
                }
                
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaId);
            return View(inspectAreaChecker);
        }

        // GET: InspectAreaCheckers/Create
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
            inspectAreaChecker.AreaCheckerId = (inspectAreaChecker.CheckerId) * 100 + (inspectAreaChecker.AreaId);
            if (ModelState.IsValid)
            {
                var isDataExist = db.InspectAreaCheckers.Find(inspectAreaChecker.AreaCheckerId);
                if( isDataExist != null )
                {
                    ModelState.AddModelError("", "此項資料已存在");
                    ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaId);
                    return View(inspectAreaChecker);
                }
                db.InspectAreaCheckers.Add(inspectAreaChecker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName", inspectAreaChecker.AreaId);
            return View(inspectAreaChecker);
        }

        // GET: InspectAreaCheckers/Delete/5
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

        // POST: InspectAreaCheckers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? areaCheckerId)
        {
            InspectAreaChecker inspectAreaChecker = db.InspectAreaCheckers.Find(areaCheckerId);
            var isLastAreaChecker = db.InspectAreaCheckers.Where(i => i.AreaId == inspectAreaChecker.AreaId).Count();
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
