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
    [Authorize(Roles = "Admin")]
    public class InspectClassController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectClass
        public ActionResult Index()
        {

            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            return View();
        }

        // GET: Admin/InspectClass/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClass inspectClass = await db.InspectClass.FindAsync(id);
            if (inspectClass == null)
            {
                return HttpNotFound();
            }
            return View(inspectClass);
        }

        // GET: Admin/InspectClass/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.ShiftsInAreas, "AreaId", "AreaId");
            return View();
        }

        // POST: Admin/InspectClass/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,ShiftId,ClassId,ClassName,ClassStatus,ClassOrder")] InspectClass inspectClass)
        {
            if (ModelState.IsValid)
            {
                db.InspectClass.Add(inspectClass);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.ShiftsInAreas, "AreaId", "AreaId", inspectClass.AreaId);
            return View(inspectClass);
        }

        // GET: Admin/InspectClass/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClass inspectClass = await db.InspectClass.FindAsync(id);
            if (inspectClass == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.ShiftsInAreas, "AreaId", "AreaId", inspectClass.AreaId);
            return View(inspectClass);
        }

        // POST: Admin/InspectClass/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,ShiftId,ClassId,ClassName,ClassStatus,ClassOrder")] InspectClass inspectClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectClass).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.ShiftsInAreas, "AreaId", "AreaId", inspectClass.AreaId);
            return View(inspectClass);
        }

        // GET: Admin/InspectClass/Edit/5
        public JsonResult GetShifts(int AreaId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var shifts = db.ShiftsInAreas.Include(s => s.InspectShift).ToList();
            shifts.Where(s => s.AreaId == AreaId).OrderBy(s => s.ShiftId).ToList()
                .ForEach(c => {
                    list.Add(new SelectListItem
                    {
                        Text = c.InspectShift.ShiftName,
                        Value = c.ShiftId.ToString()
                    });
                });
            return Json(list);
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
