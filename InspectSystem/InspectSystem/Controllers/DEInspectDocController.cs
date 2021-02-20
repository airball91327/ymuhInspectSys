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
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Controllers
{
    public class DEInspectDocController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: DEInspectDoc
        public async Task<ActionResult> Index()
        {
            return View(await db.DEInspectDoc.ToListAsync());
        }

        // GET: DEInspectDoc/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (dEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDoc);
        }

        // GET: DEInspectDoc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DEInspectDoc/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DocId,AreaId,CycleId,ClassId,ApplyDate,EndTime,CloseDate,EngId,EngName,CheckerId,CheckerName,Note")] DEInspectDoc dEInspectDoc)
        {
            if (ModelState.IsValid)
            {
                db.DEInspectDoc.Add(dEInspectDoc);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dEInspectDoc);
        }

        // GET: DEInspectDoc/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (dEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDoc);
        }

        // POST: DEInspectDoc/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,AreaId,CycleId,ClassId,ApplyDate,EndTime,CloseDate,EngId,EngName,CheckerId,CheckerName,Note")] DEInspectDoc dEInspectDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEInspectDoc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dEInspectDoc);
        }

        // GET: DEInspectDoc/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (dEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDoc);
        }

        // POST: DEInspectDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            db.DEInspectDoc.Remove(dEInspectDoc);
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
