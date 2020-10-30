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
    public class InspectItemController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectItem
        public ActionResult Index(int? areaId = null, int? shiftId = null, int? classId = null)
        {
            if (areaId != null)
            {
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", areaId);
            }
            else
            {
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            }
            ViewBag.SelectShiftId = shiftId;
            ViewBag.SelectClassId = classId;
            return View();
        }
       
        // GET: Admin/InspectItem/Create
        public ActionResult Create(int? AreaId, int? ShiftId, int? ClassId)
        {
            if (AreaId == null || ShiftId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var insItems = db.InspectItem.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId && i.ClassId == ClassId)
                                         .OrderByDescending(i => i.ItemId).FirstOrDefault();
            // Set default values.
            InspectItem inspectItem = new InspectItem();
            inspectItem.AreaId = AreaId.Value;
            inspectItem.ShiftId = ShiftId.Value;
            inspectItem.ClassId = ClassId.Value;
            if (insItems != null)
            {
                inspectItem.ItemId = insItems.ItemId + 1;
            }
            else
            {
                inspectItem.ItemId = 1;
            }
            //
            var sia = db.ShiftsInAreas.Find(AreaId, ShiftId);
            ViewBag.AreaName = sia.InspectArea.AreaName;
            ViewBag.ShiftName = sia.InspectShift.ShiftName;
            ViewBag.ClassName = db.InspectClass.Find(AreaId, ShiftId, ClassId).ClassName;
            //
            return View(inspectItem);
        }

        // POST: Admin/InspectItem/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,ShiftId,ClassId,ItemId,ItemName,ItemStatus")] InspectItem inspectItem)
        {
            if (ModelState.IsValid)
            {
                int lastOrder = 0;
                var insItems = db.InspectItem.Where(i => i.AreaId == inspectItem.AreaId && i.ShiftId == inspectItem.ShiftId 
                                                      && i.ClassId == inspectItem.ClassId);
                if (insItems.Count() > 0)
                {
                    lastOrder = insItems.OrderByDescending(i => i.ItemOrder).FirstOrDefault().ItemOrder;
                }
                //
                inspectItem.ItemOrder = lastOrder + 1;
                inspectItem.Rtp = WebSecurity.CurrentUserId;
                inspectItem.Rtt = DateTime.Now;
                db.InspectItem.Add(inspectItem);
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectItem.AreaId);
                return RedirectToAction("Index", new { areaId = inspectItem.AreaId, shiftId = inspectItem.ShiftId, classId = inspectItem.ClassId });
            }

            return View(inspectItem);
        }

        // GET: Admin/InspectItem/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? ShiftId, int? ClassId, int? ItemId)
        {
            if (AreaId == null || ShiftId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectItem inspectItem = await db.InspectItem.FindAsync(AreaId, ShiftId, ClassId, ItemId);
            if (inspectItem == null)
            {
                return HttpNotFound();
            }
            return View(inspectItem);
        }

        // POST: Admin/InspectItem/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,ShiftId,ClassId,ItemId,ItemName,ItemStatus,ItemOrder")] InspectItem inspectItem)
        {
            if (ModelState.IsValid)
            {
                inspectItem.Rtp = WebSecurity.CurrentUserId;
                inspectItem.Rtt = DateTime.Now;
                db.Entry(inspectItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName", inspectItem.AreaId);
                return RedirectToAction("Index", new { areaId = inspectItem.AreaId, shiftId = inspectItem.ShiftId, classId = inspectItem.ClassId });
            }

            return View(inspectItem);
        }

        // GET: Admin/InspectItem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectItem inspectItem = await db.InspectItem.FindAsync(id);
            if (inspectItem == null)
            {
                return HttpNotFound();
            }
            return View(inspectItem);
        }

        // POST: Admin/InspectItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            InspectItem inspectItem = await db.InspectItem.FindAsync(id);
            db.InspectItem.Remove(inspectItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/InspectItem/GetItemList/5
        public ActionResult GetItemList(int? AreaId, int? ShiftId, int? ClassId)
        {
            if (AreaId == null || ShiftId == null || ClassId == null)
            {
                throw new Exception("區域、班別或類別不正確!");
            }
            var inspectItems = db.InspectItem.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId && i.ClassId == ClassId)
                                             .OrderBy(i => i.ItemOrder).ToList();
            foreach (var item in inspectItems)
            {
                var user = db.AppUsers.Find(item.Rtp);
                if (user != null)
                {
                    item.RtpName = user.FullName + "(" + user.UserName + ")";   // Get Rtp's name.
                }
            }
            ViewData["AId"] = AreaId;
            ViewData["SId"] = ShiftId;
            ViewData["CId"] = ClassId;

            return PartialView("ItemList", inspectItems);
        }

        // POST: Admin/InspectItem/ItemList/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult ItemList(int? AreaId, int? ShiftId, int? ClassId)
        {
            if (AreaId == null || ShiftId == null || ClassId == null)
            {
                throw new Exception("區域、班別或類別不正確!");
            }
            var inspectItems = db.InspectItem.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId && i.ClassId == ClassId)
                                             .OrderBy(i => i.ItemOrder).ToList();
            foreach (var item in inspectItems)
            {
                var user = db.AppUsers.Find(item.Rtp);
                if (user != null)
                {
                    item.RtpName = user.FullName + "(" + user.UserName + ")";   // Get Rtp's name.
                }
            }
            ViewData["AId"] = AreaId;
            ViewData["SId"] = ShiftId;
            ViewData["CId"] = ClassId;

            return PartialView(inspectItems);
        }

        // POST: Admin/InspectItem/SetItemOrder/5
        [HttpPost]
        public ActionResult SetItemOrder(int oldIndex, int newIndex, int areaId, int shiftId, int classId)
        {
            if (oldIndex < newIndex)
            {
                var currItem = db.InspectItem.SingleOrDefault(i => i.ItemOrder == oldIndex &&
                                                                   i.AreaId == areaId && i.ShiftId == shiftId && i.ClassId == classId);
                if (currItem == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ItemList = db.InspectItem.Where(i => i.ItemOrder > oldIndex && i.ItemOrder <= newIndex)
                                               .Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                           i.ClassId == classId && i.ItemId != currItem.ItemId).ToList();

                foreach (var item in ItemList)
                {
                    item.ItemOrder--;
                }

                currItem.ItemOrder = newIndex;
                db.SaveChanges();
            }
            else
            {
                var currItem = db.InspectItem.SingleOrDefault(i => i.ItemOrder == oldIndex &&
                                                                   i.AreaId == areaId && i.ShiftId == shiftId && i.ClassId == classId);
                if (currItem == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ItemList = db.InspectItem.Where(i => i.ItemOrder >= newIndex && i.ItemOrder < oldIndex)
                                               .Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                           i.ClassId == classId && i.ItemId != currItem.ItemId).ToList();

                foreach (var item in ItemList)
                {
                    item.ItemOrder++;
                }

                currItem.ItemOrder = newIndex;
                db.SaveChanges();
            }
            return RedirectToAction("GetItemList", new { AreaId = areaId, ShiftId = shiftId, ClassId = classId });
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
