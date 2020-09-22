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
    public class VentilatorDocsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: VentilatorDocs
        public ActionResult Index()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "未結案", Value = "未結案" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["DOC_STATUS"] = new SelectList(listItem, "Value", "Text", "未結案");
            //
            var ventilatorDoc = db.VentilatorDoc.Include(v => v.VentilatorStatus);
            return PartialView(ventilatorDoc.ToList());
        }

        // POST: VentilatorDocs
        [HttpPost]
        public ActionResult Index(VentilatorQryVModel qryVModel)
        {
            var ventilatorDoc = db.VentilatorDoc.Include(v => v.VentilatorStatus);
            return PartialView(ventilatorDoc.ToList());
        }

        // GET: VentilatorDocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentilatorDoc ventilatorDoc = db.VentilatorDoc.Find(id);
            if (ventilatorDoc == null)
            {
                return HttpNotFound();
            }
            return View(ventilatorDoc);
        }

        // GET: VentilatorDocs/Create
        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.VentilatorStatus, "StatusId", "StatusName");
            return View();
        }

        // POST: VentilatorDocs/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocId,StatusId,ApplyDate,CloseDate,EngId,EngName,CheckerId,CheckerName,AssetNo,AssetName,DocStatus")] VentilatorDoc ventilatorDoc)
        {
            if (ModelState.IsValid)
            {
                db.VentilatorDoc.Add(ventilatorDoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.VentilatorStatus, "StatusId", "StatusName", ventilatorDoc.StatusId);
            return View(ventilatorDoc);
        }

        // GET: VentilatorDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentilatorDoc ventilatorDoc = db.VentilatorDoc.Find(id);
            if (ventilatorDoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.VentilatorStatus, "StatusId", "StatusName", ventilatorDoc.StatusId);
            return View(ventilatorDoc);
        }

        // POST: VentilatorDocs/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,StatusId,ApplyDate,CloseDate,EngId,EngName,CheckerId,CheckerName,AssetNo,AssetName,DocStatus")] VentilatorDoc ventilatorDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ventilatorDoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.VentilatorStatus, "StatusId", "StatusName", ventilatorDoc.StatusId);
            return View(ventilatorDoc);
        }

        // GET: VentilatorDocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentilatorDoc ventilatorDoc = db.VentilatorDoc.Find(id);
            if (ventilatorDoc == null)
            {
                return HttpNotFound();
            }
            return View(ventilatorDoc);
        }

        // POST: VentilatorDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VentilatorDoc ventilatorDoc = db.VentilatorDoc.Find(id);
            db.VentilatorDoc.Remove(ventilatorDoc);
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
