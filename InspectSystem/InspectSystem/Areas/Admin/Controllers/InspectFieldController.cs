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
    public class InspectFieldController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectField
        public async Task<ActionResult> Index()
        {
            var inspectField = db.InspectField.Include(i => i.InspectItem);
            return View(await inspectField.ToListAsync());
        }

        // GET: Admin/InspectField/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.InspectItem, "AreaId", "ItemName");
            return View();
        }

        // POST: Admin/InspectField/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AreaId,ShiftId,ClassId,ItemId,FieldId,FieldName,DataType,UnitOfData,MinValue,MaxValue,FieldStatus,IsRequired,ShowPastValue,Rtp,Rtt")] InspectField inspectField)
        {
            if (ModelState.IsValid)
            {
                db.InspectField.Add(inspectField);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.InspectItem, "AreaId", "ItemName", inspectField.AreaId);
            return View(inspectField);
        }

        // GET: Admin/InspectField/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectField inspectField = await db.InspectField.FindAsync(id);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.InspectItem, "AreaId", "ItemName", inspectField.AreaId);
            return View(inspectField);
        }

        // POST: Admin/InspectField/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaId,ShiftId,ClassId,ItemId,FieldId,FieldName,DataType,UnitOfData,MinValue,MaxValue,FieldStatus,IsRequired,ShowPastValue,Rtp,Rtt")] InspectField inspectField)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectField).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.InspectItem, "AreaId", "ItemName", inspectField.AreaId);
            return View(inspectField);
        }

        // GET: Admin/InspectField/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectField inspectField = await db.InspectField.FindAsync(id);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            return View(inspectField);
        }

        // POST: Admin/InspectField/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            InspectField inspectField = await db.InspectField.FindAsync(id);
            db.InspectField.Remove(inspectField);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/InspectField/GetFieldList/5
        public async Task<ActionResult> GetFieldList(int? AreaId, int? ShiftId, int? ClassId, int? ItemId)
        {
            var inspectFields = db.InspectField.Include(i => i.InspectItem)
                                               .Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId && 
                                                           i.ClassId == ClassId && i.ItemId == ItemId);
            return PartialView(await inspectFields.ToListAsync());
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
