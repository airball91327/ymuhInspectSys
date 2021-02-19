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
    public class DEInspectAreaController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectArea
        public async Task<ActionResult> Index()
        {
            var inspectAreas = await db.DEInspectArea.ToListAsync();
            foreach(var area in inspectAreas)
            {
                var user = db.AppUsers.Find(area.Rtp);
                if (user != null)
                {
                    area.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            return View(inspectAreas);
        }

        // GET: Admin/DEInspectArea/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DEInspectArea/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,AreaName,AreaStatus")] DEInspectArea inspectArea)
        {
            if (ModelState.IsValid)
            {
                inspectArea.Rtp = WebSecurity.CurrentUserId;
                inspectArea.Rtt = DateTime.Now;
                db.DEInspectArea.Add(inspectArea);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(inspectArea);
        }

        // GET: Admin/DEInspectArea/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectArea inspectArea = await db.DEInspectArea.FindAsync(id);
            if (inspectArea == null)
            {
                return HttpNotFound();
            }
            return View(inspectArea);
        }

        // POST: Admin/DEInspectArea/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,AreaName,AreaStatus")] DEInspectArea inspectArea)
        {
            if (ModelState.IsValid)
            {
                inspectArea.Rtp = WebSecurity.CurrentUserId;
                inspectArea.Rtt = DateTime.Now;
                db.Entry(inspectArea).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inspectArea);
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
