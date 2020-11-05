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
using InspectSystem.Filters;
using WebMatrix.WebData;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectClassController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectClass
        public ActionResult Index(int? areaId = null, int ? shiftId = null)
        {
            //
            if (areaId != null)
            {
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", areaId);
            }
            else
            {
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            }
            ViewBag.SelectShiftId = shiftId;
            return View();
        }

        // GET: Admin/InspectClass/Create
        public ActionResult Create(int? AreaId, int? ShiftId)
        {
            if (AreaId == null || ShiftId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var insClass = db.InspectClass.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId)
                                          .OrderByDescending(i => i.ClassId).FirstOrDefault();
            // Set default values.
            InspectClass inspectClass = new InspectClass();
            inspectClass.AreaId = AreaId.Value;
            inspectClass.ShiftId = ShiftId.Value;
            inspectClass.ClassStatus = true;
            if (insClass != null)
            {
                inspectClass.ClassId = insClass.ClassId + 1;
            }
            else
            {
                inspectClass.ClassId = 1;
            }
            //
            var sia = db.ShiftsInAreas.Find(AreaId, ShiftId);
            ViewBag.AreaName = sia.InspectArea.AreaName;
            ViewBag.ShiftName = sia.InspectShift.ShiftName;
            //
            return View(inspectClass);
        }

        // POST: Admin/InspectClass/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,ShiftId,ClassId,ClassName,ClassStatus")] InspectClass inspectClass)
        {
            if (ModelState.IsValid)
            {
                int lastOrder = 0;
                var insClasses = db.InspectClass.Where(i => i.AreaId == inspectClass.AreaId && i.ShiftId == inspectClass.ShiftId);
                if (insClasses.Count() > 0)
                {
                    lastOrder = insClasses.OrderByDescending(i => i.ClassOrder).FirstOrDefault().ClassOrder;
                }
                //
                inspectClass.ClassOrder = lastOrder + 1;
                inspectClass.Rtp = WebSecurity.CurrentUserId;
                inspectClass.Rtt = DateTime.Now;
                db.InspectClass.Add(inspectClass);
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectClass.AreaId);
                return RedirectToAction("Index", new { areaId = inspectClass.AreaId, shiftId = inspectClass.ShiftId });
            }

            return View(inspectClass);
        }

        // GET: Admin/InspectClass/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? ShiftId, int? ClassId)
        {
            if (AreaId == null || ShiftId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectClass inspectClass = await db.InspectClass.FindAsync(AreaId, ShiftId, ClassId);
            if (inspectClass == null)
            {
                return HttpNotFound();
            }
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
                inspectClass.Rtp = WebSecurity.CurrentUserId;
                inspectClass.Rtt = DateTime.Now;
                db.Entry(inspectClass).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectClass.AreaId);
                return RedirectToAction("Index", new { areaId = inspectClass.AreaId, shiftId = inspectClass.ShiftId });
            }

            return View(inspectClass);
        }

        // GET: Admin/InspectClass/GetClassList/5
        public async Task<ActionResult> GetClassList(int? AreaId, int? ShiftId)
        {
            if (AreaId == null || ShiftId == null)
            {
                throw new Exception("區域或班別不正確!");
            }
            var inspectClass = await db.InspectClass.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId)
                                                    .OrderBy(i => i.ClassOrder).ToListAsync();
            foreach (var cls in inspectClass)
            {
                var user = db.AppUsers.Find(cls.Rtp);
                if (user != null)
                {
                    cls.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            ViewData["AId"] = AreaId;
            ViewData["SId"] = ShiftId;

            return PartialView("ClassList", inspectClass);
        }

        // POST: Admin/InspectClass/ClassList/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult ClassList(int? AreaId, int? ShiftId)
        {
            if (AreaId == null || ShiftId == null)
            {
                throw new Exception("區域或班別不正確!");
            }
            var inspectClass = db.InspectClass.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId)
                                                    .OrderBy(i => i.ClassOrder).ToList();
            foreach (var cls in inspectClass)
            {
                var user = db.AppUsers.Find(cls.Rtp);
                if (user != null)
                {
                    cls.RtpName = user.FullName + "(" + user.UserName + ")";
                }
            }
            ViewData["AId"] = AreaId;
            ViewData["SId"] = ShiftId;

            return PartialView(inspectClass);
        }

        // POST: Admin/InspectClass/SetClassOrder/5
        [HttpPost]
        public ActionResult SetClassOrder(int oldIndex, int newIndex, int areaId, int shiftId)
        {
            if (oldIndex < newIndex)
            {
                var currClass = db.InspectClass.SingleOrDefault(i => i.ClassOrder == oldIndex &&
                                                                     i.AreaId == areaId && i.ShiftId == shiftId);
                if (currClass == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ClassList = db.InspectClass.Where(i => i.ClassOrder > oldIndex && i.ClassOrder <= newIndex)
                                               .Where(i => i.AreaId == areaId && i.ShiftId == shiftId && 
                                                           i.ClassId != currClass.ClassId).ToList();

                foreach (var item in ClassList)
                {
                    item.ClassOrder--;
                }

                currClass.ClassOrder = newIndex;
                db.SaveChanges();
            }
            else
            {
                var currClass = db.InspectClass.SingleOrDefault(i => i.ClassOrder == oldIndex &&
                                                                     i.AreaId == areaId && i.ShiftId == shiftId);
                if (currClass == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ClassList = db.InspectClass.Where(i => i.ClassOrder >= newIndex && i.ClassOrder < oldIndex)
                                               .Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                           i.ClassId != currClass.ClassId).ToList();

                foreach (var item in ClassList)
                {
                    item.ClassOrder++;
                }

                currClass.ClassOrder = newIndex;
                db.SaveChanges();
            }
            return RedirectToAction("GetClassList", new { AreaId = areaId, ShiftId = shiftId });
        }

        // POST: Admin/InspectClass/GetShifts/5
        [HttpPost]
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

        // POST: Admin/InspectClass/GetClasses/5
        [HttpPost]
        public JsonResult GetClasses(int AreaId, int ShiftId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var clses = db.InspectClass.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId);
            clses.OrderBy(s => s.ClassOrder).ToList()
                .ForEach(c => {
                    list.Add(new SelectListItem
                    {
                        Text = c.ClassName,
                        Value = c.ClassId.ToString()
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
