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
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DEInspectClassController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectClass
        public ActionResult Index(int? areaId = null, int ? cycleId = null)
        {
            //
            if (areaId != null)
            {
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", areaId);
            }
            else
            {
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            }
            ViewBag.SelectCycleId = cycleId;
            return View();
        }

        // GET: Admin/DEInspectClass/Create
        public ActionResult Create(int? AreaId, int? CycleId)
        {
            if (AreaId == null || CycleId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var insClass = db.DEInspectClass.Where(i => i.AreaId == AreaId && i.CycleId == CycleId)
                                            .OrderByDescending(i => i.ClassId).FirstOrDefault();
            // Set default values.
            DEInspectClass inspectClass = new DEInspectClass();
            inspectClass.AreaId = AreaId.Value;
            inspectClass.CycleId = CycleId.Value;
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
            var cia = db.DECyclesInAreas.Find(AreaId, CycleId);
            ViewBag.AreaName = cia.DEInspectArea.AreaName;
            ViewBag.CycleName = cia.DEInspectCycle.CycleName;
            //
            return View(inspectClass);
        }

        // POST: Admin/DEInspectClass/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,CycleId,ClassId,ClassName,ClassStatus")] DEInspectClass inspectClass)
        {
            if (ModelState.IsValid)
            {
                int lastOrder = 0;
                var insClasses = db.DEInspectClass.Where(i => i.AreaId == inspectClass.AreaId && i.CycleId == inspectClass.CycleId);
                if (insClasses.Count() > 0)
                {
                    lastOrder = insClasses.OrderByDescending(i => i.ClassOrder).FirstOrDefault().ClassOrder;
                }
                //
                inspectClass.ClassOrder = lastOrder + 1;
                inspectClass.Rtp = WebSecurity.CurrentUserId;
                inspectClass.Rtt = DateTime.Now;
                db.DEInspectClass.Add(inspectClass);
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectClass.AreaId);
                return RedirectToAction("Index", new { areaId = inspectClass.AreaId, cycleId = inspectClass.CycleId });
            }

            return View(inspectClass);
        }

        // GET: Admin/DEInspectClass/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? CycleId, int? ClassId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectClass inspectClass = await db.DEInspectClass.FindAsync(AreaId, CycleId, ClassId);
            if (inspectClass == null)
            {
                return HttpNotFound();
            }
            return View(inspectClass);
        }

        // POST: Admin/DEInspectClass/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,CycleId,ClassId,ClassName,ClassStatus,ClassOrder")] DEInspectClass inspectClass)
        {
            if (ModelState.IsValid)
            {
                inspectClass.Rtp = WebSecurity.CurrentUserId;
                inspectClass.Rtt = DateTime.Now;
                db.Entry(inspectClass).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectClass.AreaId);
                return RedirectToAction("Index", new { areaId = inspectClass.AreaId, cycleId = inspectClass.CycleId });
            }

            return View(inspectClass);
        }

        // GET: Admin/DEInspectClass/GetClassList/5
        public async Task<ActionResult> GetClassList(int? AreaId, int? CycleId)
        {
            if (AreaId == null || CycleId == null)
            {
                throw new Exception("區域或週期不正確!");
            }
            var inspectClass = await db.DEInspectClass.Where(i => i.AreaId == AreaId && i.CycleId == CycleId)
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
            ViewData["CYCId"] = CycleId;

            return PartialView("ClassList", inspectClass);
        }

        // POST: Admin/DEInspectClass/ClassList/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult ClassList(int? AreaId, int? CycleId)
        {
            if (AreaId == null || CycleId == null)
            {
                throw new Exception("區域或週期不正確!");
            }
            var inspectClass = db.DEInspectClass.Where(i => i.AreaId == AreaId && i.CycleId == CycleId)
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
            ViewData["CYCId"] = CycleId;

            return PartialView(inspectClass);
        }

        // POST: Admin/DEInspectClass/SetClassOrder/5
        [HttpPost]
        public ActionResult SetClassOrder(int oldIndex, int newIndex, int areaId, int cycleId)
        {
            if (oldIndex < newIndex)
            {
                var currClass = db.DEInspectClass.SingleOrDefault(i => i.ClassOrder == oldIndex &&
                                                                       i.AreaId == areaId && i.CycleId == cycleId);
                if (currClass == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ClassList = db.DEInspectClass.Where(i => i.ClassOrder > oldIndex && i.ClassOrder <= newIndex)
                                                 .Where(i => i.AreaId == areaId && i.CycleId == cycleId && 
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
                var currClass = db.DEInspectClass.SingleOrDefault(i => i.ClassOrder == oldIndex &&
                                                                       i.AreaId == areaId && i.CycleId == cycleId);
                if (currClass == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ClassList = db.DEInspectClass.Where(i => i.ClassOrder >= newIndex && i.ClassOrder < oldIndex)
                                                 .Where(i => i.AreaId == areaId && i.CycleId == cycleId &&
                                                             i.ClassId != currClass.ClassId).ToList();

                foreach (var item in ClassList)
                {
                    item.ClassOrder++;
                }

                currClass.ClassOrder = newIndex;
                db.SaveChanges();
            }
            return RedirectToAction("GetClassList", new { AreaId = areaId, CycleId = cycleId });
        }

        // POST: Admin/DEInspectClass/GetCycles/5
        [HttpPost]
        public JsonResult GetCycles(int AreaId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cycles = db.DECyclesInAreas.Include(s => s.DEInspectCycle).ToList();
            cycles.Where(s => s.AreaId == AreaId).OrderBy(s => s.CycleId).ToList()
                .ForEach(c => {
                    list.Add(new SelectListItem
                    {
                        Text = c.DEInspectCycle.CycleName,
                        Value = c.CycleId.ToString()
                    });
                });
            return Json(list);
        }

        // POST: Admin/DEInspectClass/GetClasses/5
        [HttpPost]
        public JsonResult GetClasses(int AreaId, int CycleId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var clses = db.DEInspectClass.Where(i => i.AreaId == AreaId && i.CycleId == CycleId);
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
