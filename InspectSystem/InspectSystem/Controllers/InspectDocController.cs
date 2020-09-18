using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    public class InspectDocController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDoc
        public async Task<ActionResult> Index()
        {
            var inspectDoc = db.InspectDoc.Include(i => i.InspectDocIdTable);
            return View(await inspectDoc.ToListAsync());
        }

        // GET: InspectDoc/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDoc inspectDoc = await db.InspectDoc.FindAsync(id);
            if (inspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(inspectDoc);
        }

        // GET: InspectDoc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InspectDoc/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DocId,ShiftId,ApplyDate,EndTime,CloseDate,EngId,EngName,CheckerId,CheckerName")] InspectDoc inspectDoc)
        {
            if (ModelState.IsValid)
            {
                db.InspectDoc.Add(inspectDoc);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DocId = new SelectList(db.InspectDocIdTable, "DocId", "AreaName", inspectDoc.DocId);
            return View(inspectDoc);
        }

        // GET: InspectDoc/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDoc inspectDoc = await db.InspectDoc.FindAsync(id);
            if (inspectDoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocId = new SelectList(db.InspectDocIdTable, "DocId", "AreaName", inspectDoc.DocId);
            return View(inspectDoc);
        }

        // POST: InspectDoc/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ShiftId,ApplyDate,EndTime,CloseDate,EngId,EngName,CheckerId,CheckerName")] InspectDoc inspectDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDoc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DocId = new SelectList(db.InspectDocIdTable, "DocId", "AreaName", inspectDoc.DocId);
            return View(inspectDoc);
        }

        // GET: InspectDoc/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDoc inspectDoc = await db.InspectDoc.FindAsync(id);
            if (inspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(inspectDoc);
        }

        // POST: InspectDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            InspectDoc inspectDoc = await db.InspectDoc.FindAsync(id);
            db.InspectDoc.Remove(inspectDoc);
            await db.SaveChangesAsync();
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
