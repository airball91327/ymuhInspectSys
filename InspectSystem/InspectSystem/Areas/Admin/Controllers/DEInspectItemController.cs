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
    public class DEInspectItemController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectItem
        public ActionResult Index(int? areaId = null, int? cycleId = null, int? classId = null)
        {
            if (areaId != null)
            {
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", areaId);
            }
            else
            {
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            }
            ViewBag.SelectCycleId = cycleId;
            ViewBag.SelectClassId = classId;
            return View();
        }

        // GET: Admin/DEInspectItem/Create
        public ActionResult Create(int? AreaId, int? CycleId, int? ClassId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var insItems = db.DEInspectItem.Where(i => i.AreaId == AreaId && i.CycleId == CycleId && i.ClassId == ClassId)
                                           .OrderByDescending(i => i.ItemId).FirstOrDefault();
            // Set default values.
            DEInspectItem inspectItem = new DEInspectItem();
            inspectItem.AreaId = AreaId.Value;
            inspectItem.CycleId = CycleId.Value;
            inspectItem.ClassId = ClassId.Value;
            inspectItem.ItemStatus = true;
            if (insItems != null)
            {
                inspectItem.ItemId = insItems.ItemId + 1;
            }
            else
            {
                inspectItem.ItemId = 1;
            }
            //
            var cia = db.DECyclesInAreas.Find(AreaId, CycleId);
            ViewBag.AreaName = cia.DEInspectArea.AreaName;
            ViewBag.CycleName = cia.DEInspectCycle.CycleName;
            ViewBag.ClassName = db.DEInspectClass.Find(AreaId, CycleId, ClassId).ClassName;
            //
            return View(inspectItem);
        }

        // POST: Admin/DEInspectItem/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,CycleId,ClassId,ItemId,ItemName,ItemStatus")] DEInspectItem inspectItem)
        {
            if (ModelState.IsValid)
            {
                int lastOrder = 0;
                var insItems = db.DEInspectItem.Where(i => i.AreaId == inspectItem.AreaId && i.CycleId == inspectItem.CycleId 
                                                        && i.ClassId == inspectItem.ClassId);
                if (insItems.Count() > 0)
                {
                    lastOrder = insItems.OrderByDescending(i => i.ItemOrder).FirstOrDefault().ItemOrder;
                }
                //
                inspectItem.ItemOrder = lastOrder + 1;
                inspectItem.Rtp = WebSecurity.CurrentUserId;
                inspectItem.Rtt = DateTime.Now;
                db.DEInspectItem.Add(inspectItem);
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectItem.AreaId);
                return RedirectToAction("Index", new { areaId = inspectItem.AreaId, cycleId = inspectItem.CycleId, classId = inspectItem.ClassId });
            }

            return View(inspectItem);
        }

        // GET: Admin/DEInspectItem/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? CycleId, int? ClassId, int? ItemId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectItem inspectItem = await db.DEInspectItem.FindAsync(AreaId, CycleId, ClassId, ItemId);
            if (inspectItem == null)
            {
                return HttpNotFound();
            }
            return View(inspectItem);
        }

        // POST: Admin/DEInspectItem/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,CycleId,ClassId,ItemId,ItemName,ItemStatus,ItemOrder")] DEInspectItem inspectItem)
        {
            if (ModelState.IsValid)
            {
                inspectItem.Rtp = WebSecurity.CurrentUserId;
                inspectItem.Rtt = DateTime.Now;
                db.Entry(inspectItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName", inspectItem.AreaId);
                return RedirectToAction("Index", new { areaId = inspectItem.AreaId, shiftId = inspectItem.CycleId, classId = inspectItem.ClassId });
            }

            return View(inspectItem);
        }

        // GET: Admin/DEInspectItem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectItem inspectItem = await db.DEInspectItem.FindAsync(id);
            if (inspectItem == null)
            {
                return HttpNotFound();
            }
            return View(inspectItem);
        }

        // POST: Admin/DEInspectItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DEInspectItem inspectItem = await db.DEInspectItem.FindAsync(id);
            db.DEInspectItem.Remove(inspectItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/DEInspectItem/GetItemList/5
        public ActionResult GetItemList(int? AreaId, int? CycleId, int? ClassId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                throw new Exception("區域、週期或類別不正確!");
            }
            var inspectItems = db.DEInspectItem.Where(i => i.AreaId == AreaId && i.CycleId == CycleId && i.ClassId == ClassId)
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
            ViewData["CYCId"] = CycleId;
            ViewData["CId"] = ClassId;

            return PartialView("ItemList", inspectItems);
        }

        // POST: Admin/DEInspectItem/ItemList/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult ItemList(int? AreaId, int? CycleId, int? ClassId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                throw new Exception("區域、週期或類別不正確!");
            }
            var inspectItems = db.DEInspectItem.Where(i => i.AreaId == AreaId && i.CycleId == CycleId && i.ClassId == ClassId)
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
            ViewData["CYCId"] = CycleId;
            ViewData["CId"] = ClassId;

            return PartialView(inspectItems);
        }

        // POST: Admin/DEInspectItem/SetItemOrder/5
        [HttpPost]
        public ActionResult SetItemOrder(int oldIndex, int newIndex, int areaId, int cycleId, int classId)
        {
            if (oldIndex < newIndex)
            {
                var currItem = db.DEInspectItem.SingleOrDefault(i => i.ItemOrder == oldIndex &&
                                                                     i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId);
                if (currItem == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ItemList = db.DEInspectItem.Where(i => i.ItemOrder > oldIndex && i.ItemOrder <= newIndex)
                                               .Where(i => i.AreaId == areaId && i.CycleId == cycleId &&
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
                var currItem = db.DEInspectItem.SingleOrDefault(i => i.ItemOrder == oldIndex &&
                                                                     i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId);
                if (currItem == null)
                {
                    var msg = "排序錯誤";
                    return Json(msg);
                }

                var ItemList = db.DEInspectItem.Where(i => i.ItemOrder >= newIndex && i.ItemOrder < oldIndex)
                                               .Where(i => i.AreaId == areaId && i.CycleId == cycleId &&
                                                           i.ClassId == classId && i.ItemId != currItem.ItemId).ToList();

                foreach (var item in ItemList)
                {
                    item.ItemOrder++;
                }

                currItem.ItemOrder = newIndex;
                db.SaveChanges();
            }
            return RedirectToAction("GetItemList", new { AreaId = areaId, CycleId = cycleId, ClassId = classId });
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
