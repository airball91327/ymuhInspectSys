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

        // POST: InspectDocDetailTemp/TempSave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TempSave(List<InspectDocDetailTemp> inspectDocDetailTemp)
        {
            try
            {
                foreach (var item in inspectDocDetailTemp)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();

                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
            catch (Exception e)
            {
                return new JsonResult
                {
                    Data = new { success = false, error = "暫存失敗!" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
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
