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
    public class InspectDocIdTableController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocIdTable
        public async Task<ActionResult> Index()
        {
            return View(await db.InspectDocIdTable.ToListAsync());
        }

        // GET: InspectDocIdTable/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Create
        public ActionResult Create()
        {
            // Get inspect area list.
            var inspectAreas = db.InspectArea.Where(a => a.AreaStatus == true).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach(var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");
            //
            // Set default value.
            InspectDocIdTable insp = new InspectDocIdTable();
            insp.ApplyDate = DateTime.Now;
            return View(insp);
        }

        // POST: InspectDocIdTable/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ApplyDate,AreaId,AreaName")] InspectDocIdTable inspectDocIdTable)
        {
            if (ModelState.IsValid)
            {
                db.InspectDocIdTable.Add(inspectDocIdTable);
                //await db.SaveChangesAsync();
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            // Get inspect area list.
            var inspectAreas = db.InspectArea.Where(a => a.AreaStatus == true).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");

            return View(inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // POST: InspectDocIdTable/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ApplyDate,CloseDate,AreaId,AreaName")] InspectDocIdTable inspectDocIdTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDocIdTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inspectDocIdTable);
        }

        // GET: InspectDocIdTable/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            return View(inspectDocIdTable);
        }

        // POST: InspectDocIdTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            db.InspectDocIdTable.Remove(inspectDocIdTable);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get Inspect DocId by insert values.
        /// </summary>
        /// <returns></returns>
        private string GetDocId(int AreaId, DateTime ApplyDate)
        {
            string docId = null;


            return docId;
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
