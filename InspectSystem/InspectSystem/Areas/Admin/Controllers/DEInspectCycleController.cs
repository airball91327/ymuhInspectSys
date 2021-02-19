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
    public class DEInspectCycleController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectCycle
        public async Task<ActionResult> Index()
        {
            var inspectCycles = await db.DEInspectCycle.ToListAsync();
            foreach (var item in inspectCycles)
            {
                var user = db.AppUsers.Find(item.Rtp);
                if (user != null)
                {
                    item.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            return View(inspectCycles);
        }

        // GET: Admin/DEInspectCycle/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DEInspectCycle/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CycleId,CycleName")] DEInspectCycle inspectCycle)
        {
            if (ModelState.IsValid)
            {
                inspectCycle.Rtp = WebSecurity.CurrentUserId;
                inspectCycle.Rtt = DateTime.Now;
                db.DEInspectCycle.Add(inspectCycle);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(inspectCycle);
        }

        // GET: Admin/DEInspectCycle/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectCycle inspectCycle = await db.DEInspectCycle.FindAsync(id);
            if (inspectCycle == null)
            {
                return HttpNotFound();
            }
            return View(inspectCycle);
        }

        // POST: Admin/DEInspectCycle/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CycleId,CycleName")] DEInspectCycle inspectCycle)
        {
            if (ModelState.IsValid)
            {
                inspectCycle.Rtp = WebSecurity.CurrentUserId;
                inspectCycle.Rtt = DateTime.Now;
                db.Entry(inspectCycle).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inspectCycle);
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
