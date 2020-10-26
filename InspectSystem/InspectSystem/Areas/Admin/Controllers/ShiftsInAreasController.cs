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

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShiftsInAreasController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/ShiftsInAreas
        public async Task<ActionResult> Index()
        {
            var shiftsInAreas = await db.ShiftsInAreas.Include(s => s.InspectArea).Include(s => s.InspectShift).ToListAsync();
            foreach (var s in shiftsInAreas)
            {
                var user = db.AppUsers.Find(s.Rtp);
                if (user != null)
                {
                    s.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            return View(shiftsInAreas);
        }

        // GET: Admin/ShiftsInAreas/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            ViewBag.ShiftId = new SelectList(db.InspectShift, "ShiftId", "ShiftName");
            return View();
        }

        // POST: Admin/ShiftsInAreas/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,ShiftId,Status")] ShiftsInAreas shiftsInAreas)
        {
            var isDataExist = db.ShiftsInAreas.Find(shiftsInAreas.AreaId, shiftsInAreas.ShiftId);
            if (isDataExist != null)
            {
                ModelState.AddModelError("", "該區域已有此班別!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    shiftsInAreas.Rtp = WebSecurity.CurrentUserId;
                    shiftsInAreas.Rtt = DateTime.Now;
                    db.ShiftsInAreas.Add(shiftsInAreas);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", shiftsInAreas.AreaId);
            ViewBag.ShiftId = new SelectList(db.InspectShift, "ShiftId", "ShiftName", shiftsInAreas.ShiftId);
            return View(shiftsInAreas);
        }

        // GET: Admin/ShiftsInAreas/Edit/5
        public async Task<ActionResult> Edit(int? areaId, int? shiftId)
        {
            if (areaId == null || shiftId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShiftsInAreas shiftsInAreas = await db.ShiftsInAreas.FindAsync(areaId, shiftId);
            if (shiftsInAreas == null)
            {
                return HttpNotFound();
            }

            return View(shiftsInAreas);
        }

        // POST: Admin/ShiftsInAreas/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,ShiftId,Status")] ShiftsInAreas shiftsInAreas)
        {
            if (ModelState.IsValid)
            {
                shiftsInAreas.Rtp = WebSecurity.CurrentUserId;
                shiftsInAreas.Rtt = DateTime.Now;
                db.Entry(shiftsInAreas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", shiftsInAreas.AreaId);
            ViewBag.ShiftId = new SelectList(db.InspectShift, "ShiftId", "ShiftName", shiftsInAreas.ShiftId);
            return View(shiftsInAreas);
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
