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

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize]
    public class DEInspectPrecautionsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectPrecautions
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var inspectPrecautions = db.DEInspectPrecautions.Include(i => i.DEInspectArea).OrderBy(i => i.AreaId);
            return View(await inspectPrecautions.ToListAsync());
        }

        // GET: Admin/DEInspectPrecautions/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectPrecautions inspectPrecautions = await db.DEInspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // GET: Admin/DEInspectPrecautions/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            return View();
        }

        // POST: Admin/DEInspectPrecautions/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PrecautionId,AreaId,Content")] DEInspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.DEInspectPrecautions.Add(inspectPrecautions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // GET: Admin/DEInspectPrecautions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectPrecautions inspectPrecautions = await db.DEInspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // POST: Admin/DEInspectPrecautions/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PrecautionId,AreaId,Content")] DEInspectPrecautions inspectPrecautions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectPrecautions).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectPrecautions.AreaId);
            return View(inspectPrecautions);
        }

        // GET: Admin/DEInspectPrecautions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectPrecautions inspectPrecautions = await db.DEInspectPrecautions.FindAsync(id);
            if (inspectPrecautions == null)
            {
                return HttpNotFound();
            }
            return View(inspectPrecautions);
        }

        // POST: Admin/DEInspectPrecautions/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DEInspectPrecautions inspectPrecautions = await db.DEInspectPrecautions.FindAsync(id);
            db.DEInspectPrecautions.Remove(inspectPrecautions);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/DEInspectPrecautions/AreaPrecautions/5
        public ActionResult AreaPrecautions(int areaId)
        {
            var areaPrecautions = db.DEInspectPrecautions.Where(i => i.AreaId == areaId);
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
