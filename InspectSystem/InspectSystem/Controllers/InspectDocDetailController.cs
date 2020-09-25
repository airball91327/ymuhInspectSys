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
    public class InspectDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocDetail
        public async Task<ActionResult> Index()
        {
            var inspectDocDetail = db.InspectDocDetail.Include(i => i.InspectDocs);
            return View(await inspectDocDetail.ToListAsync());
        }

        // GET: InspectDocDetail/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetail inspectDocDetail = await db.InspectDocDetail.FindAsync(id);
            if (inspectDocDetail == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetail);
        }

        // GET: InspectDocDetail/Create
        public ActionResult Create()
        {
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName");
            return View();
        }

        // POST: InspectDocDetail/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DocId,ShiftId,ClassId,ItemId,FieldId,AreaId,AreaName,ShiftName,ClassName,ClassOrder,ItemName,ItemOrder,FieldName,DataType,UnitOfData,MinValue,MaxValue,IsRequired,Value,IsFunctional,ErrorDescription,RepairDocId,DropDownItems")] InspectDocDetail inspectDocDetail)
        {
            if (ModelState.IsValid)
            {
                db.InspectDocDetail.Add(inspectDocDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetail.DocId);
            return View(inspectDocDetail);
        }

        // GET: InspectDocDetail/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetail inspectDocDetail = await db.InspectDocDetail.FindAsync(id);
            if (inspectDocDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetail.DocId);
            return View(inspectDocDetail);
        }

        // POST: InspectDocDetail/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ShiftId,ClassId,ItemId,FieldId,AreaId,AreaName,ShiftName,ClassName,ClassOrder,ItemName,ItemOrder,FieldName,DataType,UnitOfData,MinValue,MaxValue,IsRequired,Value,IsFunctional,ErrorDescription,RepairDocId,DropDownItems")] InspectDocDetail inspectDocDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DocId = new SelectList(db.InspectDoc, "DocId", "EngName", inspectDocDetail.DocId);
            return View(inspectDocDetail);
        }

        // GET: InspectDocDetail/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocDetail inspectDocDetail = await db.InspectDocDetail.FindAsync(id);
            if (inspectDocDetail == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocDetail);
        }

        // POST: InspectDocDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            InspectDocDetail inspectDocDetail = await db.InspectDocDetail.FindAsync(id);
            db.InspectDocDetail.Remove(inspectDocDetail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //GET: InspectDocDetail/CheckValue
        public ActionResult CheckValue(string docId, int shiftId, int classId, int itemId, int fieldId, string value)
        {
            /* Get the min and max value for the check field. */
            var searchField = db.InspectDocDetailTemp.Find(docId, shiftId, classId, itemId, fieldId);
            var fieldDataType = searchField.DataType;
            float maxValue = System.Convert.ToSingle(searchField.MaxValue);
            float minValue = System.Convert.ToSingle(searchField.MinValue);

            /* Only float type will check. */
            string msg = "";
            if (fieldDataType == "float")
            {
                /* Check the input string can be convert to float. */
                if (float.TryParse(value, out float inputValue))
                {
                    // Check max and min value, and if doesn't set the min or max value, return nothing.
                    if (inputValue >= maxValue && minValue != 0)
                    {
                        msg = "<span style='color:red'>大於正常數值</span>";
                    }
                    else if (inputValue <= minValue && minValue != 0)
                    {
                        msg = "<span style='color:red'>小於正常數值</span>";
                    }
                    else if (minValue == 0 && maxValue == 0) // If min and max both set to 0, not check the value.
                    {
                        msg = "";
                    }
                    else
                    {
                        msg = "";
                    }
                }
                else
                {
                    msg = "<span style='color:red'>請輸入數字</span>";
                }
            }
            else
            {
                msg = "";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
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
