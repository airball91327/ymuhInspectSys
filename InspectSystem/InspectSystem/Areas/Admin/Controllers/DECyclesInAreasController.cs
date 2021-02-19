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
using WebMatrix.WebData;
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DECyclesInAreasController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DECyclesInAreas
        public async Task<ActionResult> Index()
        {
            var cyclesInAreas = await db.DECyclesInAreas.Include(s => s.DEInspectArea).Include(s => s.DEInspectCycle).ToListAsync();
            foreach (var item in cyclesInAreas)
            {
                var user = db.AppUsers.Find(item.Rtp);
                if (user != null)
                {
                    item.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            return View(cyclesInAreas);
        }

        // GET: Admin/DECyclesInAreas/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            ViewBag.CycleId = new SelectList(db.DEInspectCycle, "CycleId", "CycleName");
            return View();
        }

        // POST: Admin/DECyclesInAreas/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,CycleId,Status")] DECyclesInAreas cyclesInAreas)
        {
            var isDataExist = db.DECyclesInAreas.Find(cyclesInAreas.AreaId, cyclesInAreas.CycleId);
            if (isDataExist != null)
            {
                ModelState.AddModelError("", "該區域已設定此週期!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    cyclesInAreas.Rtp = WebSecurity.CurrentUserId;
                    cyclesInAreas.Rtt = DateTime.Now;
                    db.DECyclesInAreas.Add(cyclesInAreas);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", cyclesInAreas.AreaId);
            ViewBag.CycleId = new SelectList(db.DEInspectCycle, "CycleId", "CycleName", cyclesInAreas.CycleId);
            return View(cyclesInAreas);
        }

        // GET: Admin/DECyclesInAreas/Edit/5
        public async Task<ActionResult> Edit(int? areaId, int? cycleId)
        {
            if (areaId == null || cycleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DECyclesInAreas cyclesInAreas = await db.DECyclesInAreas.FindAsync(areaId, cycleId);
            if (cyclesInAreas == null)
            {
                return HttpNotFound();
            }

            return View(cyclesInAreas);
        }

        // POST: Admin/DECyclesInAreas/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,CycleId,Status")] DECyclesInAreas cyclesInAreas)
        {
            if (ModelState.IsValid)
            {
                cyclesInAreas.Rtp = WebSecurity.CurrentUserId;
                cyclesInAreas.Rtt = DateTime.Now;
                db.Entry(cyclesInAreas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", cyclesInAreas.AreaId);
            ViewBag.CycleId = new SelectList(db.DEInspectCycle, "CycleId", "CycleName", cyclesInAreas.CycleId);
            return View(cyclesInAreas);
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
