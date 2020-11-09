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

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize]
    public class InspectPrecautionController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectPrecaution
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var inspectPrecautions = db.InspectPrecautions.Include(i => i.InspectArea).OrderBy(i => i.AreaId);
            return View(await inspectPrecautions.ToListAsync());
        }

        // GET: Admin/InspectPrecaution/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = await db.InspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // GET: Admin/InspectPrecaution/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            return View();
        }

        // POST: Admin/InspectPrecaution/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PrecautionId,AreaId,Content")] InspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.InspectPrecautions.Add(inspectPrecautions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // GET: Admin/InspectPrecaution/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = await db.InspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // POST: Admin/InspectPrecaution/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PrecautionId,AreaId,Content")] InspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectPrecautions).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // GET: Admin/InspectPrecaution/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectPrecautions inspectPrecautions = await db.InspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // POST: Admin/InspectPrecaution/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            InspectPrecautions inspectPrecautions = await db.InspectPrecautions.FindAsync(id);
            db.InspectPrecautions.Remove(inspectPrecautions);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/InspectPrecaution/AreaPrecautions/5
        public ActionResult AreaPrecautions(int areaId)
        {
            var areaPrecautions = db.InspectPrecautions.Where(i => i.AreaId == areaId);
            return PartialView(areaPrecautions.ToList());
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
