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
            var areaId = inspectDoc.AreaId;
            var cycleId = inspectDoc.CycleId;
            var classId = inspectDoc.ClassId;
            // Get items and fields. 
            var items = db.DEInspectItem.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                        .Where(i => i.ItemStatus == true)
                                        .OrderBy(i => i.ItemOrder).ToList();
            var fields = db.DEInspectField.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                          .Where(i => i.FieldStatus == true)
                                          .ToList();
            var dropdowns = db.DEInspectFieldDropDown.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                                     .ToList();
            ViewBag.ClassName = db.DEInspectClass.Find(areaId, cycleId, classId).ClassName;
            //
            DEInspectDocDetailVModel inspectDocDetailViewModel = new DEInspectDocDetailVModel()
            {
                InspectDocDetailTemp = docDetailTemp,
                InspectItems = items,
                InspectFields = fields,
                InspectFieldDropDowns = dropdowns
            };

            return PartialView(inspectDocDetailViewModel);
        }

        // Get: DEInspectDocDetailTemp/Views/5
        public ActionResult Views(string docId)
        {
            var docDetailTemp = db.DEInspectDocDetailTemp.Where(t => t.DocId == docId).ToList();
            var inspectDoc = db.DEInspectDoc.Find(docId);
            //
            var areaId = inspectDoc.AreaId;
            var cycleId = inspectDoc.CycleId;
            var classId = inspectDoc.ClassId;
            // Get items and fields. 
            var items = db.DEInspectItem.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                        .Where(i => i.ItemStatus == true)
                                        .OrderBy(i => i.ItemOrder).ToList();
            var fields = db.DEInspectField.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                          .Where(i => i.FieldStatus == true)
                                          .ToList();
            var dropdowns = db.DEInspectFieldDropDown.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                                     .ToList();
            ViewBag.ClassName = db.DEInspectClass.Find(areaId, cycleId, classId).ClassName;
            //
            DEInspectDocDetailVModel inspectDocDetailViewModel = new DEInspectDocDetailVModel()
            {
                InspectDocDetailTemp = docDetailTemp,
                InspectItems = items,
                InspectFields = fields,
                InspectFieldDropDowns = dropdowns
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