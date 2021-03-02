using InspectSystem.Models;
using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class DEInspectDocDetailTempController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: DEInspectDocDetailTemp
        public async Task<ActionResult> Index()
        {
            var inspectDocDetail = db.DEInspectDocDetail.Include(i => i.DEInspectDocs);
            return View(await inspectDocDetail.ToListAsync());
        }

        // Get: DEInspectDocDetailTemp/Edit/5
        public ActionResult Edit(string docId)
        {
            var docDetailTemp = db.DEInspectDocDetailTemp.Where(t => t.DocId == docId).ToList();
            var inspectDoc = db.DEInspectDoc.Find(docId);
            //
            if (docDetailTemp.Count() > 0)
            {
                ViewBag.ClassName = docDetailTemp.First().ClassName;
                // Get items and fields from DocDetailTemp list. 
                ViewData["itemsOfDocDetailTemps"] = docDetailTemp.GroupBy(i => i.ItemId)
                                                                 .Select(g => g.FirstOrDefault())
                                                                 .OrderBy(s => s.ItemOrder).ToList();
                ViewData["fieldsOfDocDetailTemps"] = docDetailTemp.ToList();
            }

            DEInspectDocDetailVModel inspectDocDetailViewModel = new DEInspectDocDetailVModel()
            {
                InspectDocDetailTemp = docDetailTemp
            };

            return PartialView(inspectDocDetailViewModel);
        }

        // Get: DEInspectDocDetailTemp/Views/5
        public ActionResult Views(string docId)
        {
            var docDetailTemp = db.DEInspectDocDetailTemp.Where(t => t.DocId == docId).ToList();
            var inspectDoc = db.DEInspectDoc.Find(docId);
            //
            if (docDetailTemp.Count() > 0)
            {
                ViewBag.ClassName = docDetailTemp.First().ClassName;
                // Get items and fields from DocDetailTemp list. 
                ViewData["itemsOfDocDetailTemps"] = docDetailTemp.GroupBy(i => i.ItemId)
                                                                 .Select(g => g.FirstOrDefault())
                                                                 .OrderBy(s => s.ItemOrder).ToList();
                ViewData["fieldsOfDocDetailTemps"] = docDetailTemp.ToList();
            }

            DEInspectDocDetailVModel inspectDocDetailViewModel = new DEInspectDocDetailVModel()
            {
                InspectDocDetailTemp = docDetailTemp
            };

            return PartialView(inspectDocDetailViewModel);
        }

        // POST: DEInspectDocDetailTemp/TempSave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TempSave(List<DEInspectDocDetailTemp> inspectDocDetailTemp)
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

    }
}