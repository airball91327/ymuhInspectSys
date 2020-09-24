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

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocDetailTempController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocDetailTemp
        public async Task<ActionResult> Index()
        {
            var inspectDocDetailTemp = db.InspectDocDetailTemp.Include(i => i.InspectDocs);
            return View(await inspectDocDetailTemp.ToListAsync());
        }

        // GET: InspectDocDetailTemp/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetailTemp inspectDocDetailTemp = await db.InspectDocDetailTemp.FindAsync(id);
            if (inspectDocDetailTemp == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetailTemp);
        }

        // GET: InspectDocDetailTemp/Create
        public ActionResult Create()
        {
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName");
            return View();
        }

        // POST: InspectDocDetailTemp/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DocId,ShiftId,ClassId,ItemId,FieldId,AreaId,AreaName,ShiftName,ClassName,ClassOrder,ItemName,ItemOrder,FieldName,DataType,UnitOfData,MinValue,MaxValue,IsRequired,Value,IsFunctional,ErrorDescription,RepairDocId,DropDownItems")] InspectDocDetailTemp inspectDocDetailTemp)
        {
            if (ModelState.IsValid)
            {
                db.InspectDocDetailTemp.Add(inspectDocDetailTemp);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetailTemp.DocId);
            return View(inspectDocDetailTemp);
        }

        // GET: InspectDocDetailTemp/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetailTemp inspectDocDetailTemp = await db.InspectDocDetailTemp.FindAsync(id);
            if (inspectDocDetailTemp == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetailTemp.DocId);
            return View(inspectDocDetailTemp);
        }

        // POST: InspectDocDetailTemp/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ShiftId,ClassId,ItemId,FieldId,AreaId,AreaName,ShiftName,ClassName,ClassOrder,ItemName,ItemOrder,FieldName,DataType,UnitOfData,MinValue,MaxValue,IsRequired,Value,IsFunctional,ErrorDescription,RepairDocId,DropDownItems")] InspectDocDetailTemp inspectDocDetailTemp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocDetailTemp).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetailTemp.DocId);
            return View(inspectDocDetailTemp);
        }

        // GET: InspectDocDetailTemp/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetailTemp inspectDocDetailTemp = await db.InspectDocDetailTemp.FindAsync(id);
            if (inspectDocDetailTemp == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetailTemp);
        }

        // POST: InspectDocDetailTemp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            InspectDocDetailTemp inspectDocDetailTemp = await db.InspectDocDetailTemp.FindAsync(id);
            db.InspectDocDetailTemp.Remove(inspectDocDetailTemp);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Get: InspectDocDetailTemp/GetClassContents/5
        public ActionResult GetClassContents(string docId, string shiftId, string classId)
        {
            int iShiftId = Convert.ToInt32(shiftId);
            int iClassId = Convert.ToInt32(classId);
            // Get inspect DocDetailTemp list.
            var docDetailTemp = db.InspectDocDetailTemp.Where(t => t.DocId == docId && t.ShiftId == iShiftId &&
                                                                   t.ClassId == iClassId).ToList();
            if (docDetailTemp.Count() > 0)
            {
                ViewBag.ClassName = docDetailTemp.First().ClassName;
                // Get items and fields from DocDetailTemp list. 
                ViewData["itemsByDocDetailTemps"] = docDetailTemp.GroupBy(i => i.ItemId)
                                                                 .Select(g => g.FirstOrDefault())
                                                                 .OrderBy(s => s.ItemOrder).ToList();
                ViewData["fieldsByDocDetailTemps"] = docDetailTemp.ToList();
            }

            InspectDocDetailViewModel inspectDocDetailViewModel = new InspectDocDetailViewModel()
            {
                InspectDocDetailTemp = docDetailTemp
            };

            return PartialView(inspectDocDetailViewModel);
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
