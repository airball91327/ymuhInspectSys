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
    public class InspectShiftController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectShift
        public async Task<ActionResult> Index()
        {
            var inspectShifts = await db.InspectShift.ToListAsync();
            foreach (var area in inspectShifts)
            {
                var user = db.AppUsers.Find(area.Rtp);
                if (user != null)
                {
                    area.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            return View(inspectShifts);
        }

        // GET: Admin/InspectShift/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/InspectShift/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ShiftId,ShiftName")] InspectShift inspectShift)
        {
            if (ModelState.IsValid)
            {
                inspectShift.Rtp = WebSecurity.CurrentUserId;
                inspectShift.Rtt = DateTime.Now;
                db.InspectShift.Add(inspectShift);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(inspectShift);
        }

        // GET: Admin/InspectShift/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectShift inspectShift = await db.InspectShift.FindAsync(id);
            if (inspectShift == null)
            {
                return HttpNotFound();
            }
            return View(inspectShift);
        }

        // POST: Admin/InspectShift/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ShiftId,ShiftName")] InspectShift inspectShift)
        {
            if (ModelState.IsValid)
            {
                inspectShift.Rtp = WebSecurity.CurrentUserId;
                inspectShift.Rtt = DateTime.Now;
                db.Entry(inspectShift).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inspectShift);
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
