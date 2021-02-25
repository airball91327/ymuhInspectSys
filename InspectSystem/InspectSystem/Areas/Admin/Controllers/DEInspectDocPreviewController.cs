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
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DEInspectDocPreviewController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectDocPreview
        public async Task<ActionResult> Index()
        {
            // Get inspect area list.
            var inspectAreas = await db.DEInspectArea.Where(a => a.AreaStatus == true).ToListAsync();
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
            return View();
        }

        // POST: Admin/DEInspectDocPreview/Index/5
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var areaId = fc["AreaId"];
            var cycleId = fc["CycleId"];
            var classId = fc["ClassId"];
            return RedirectToAction("Preview", new { AreaId = areaId, CycleId = cycleId, ClassId = classId });
        }

        // GET: Admin/DEInspectDocPreview/Preview/5
        public ActionResult Preview(int? AreaId, int? CycleId, int? ClassId)
        {
            if (AreaId == null || CycleId == null || ClassId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cyclesInAreas = db.DECyclesInAreas.Include(s => s.DEInspectArea).Include(s => s.DEInspectCycle)
                                                  .Where(s => s.Status == true)
                                                  .Where(s => s.AreaId == AreaId && s.CycleId == CycleId).ToList();
            // Set variables.
            var cycleId = CycleId;
            var previewClass = db.DEInspectClass.Where(c => c.AreaId == AreaId && c.CycleId == CycleId && c.ClassId == ClassId).FirstOrDefault();
            var cycleName = cyclesInAreas.FirstOrDefault().DEInspectCycle.CycleName;
            var areaName = cyclesInAreas.FirstOrDefault().DEInspectArea.AreaName;
            var className = "";
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            //
            if (previewClass != null)
            {
                className = previewClass.ClassName;
                // Insert values to classVModel.
                classVModel.DocId = "00000000";
                classVModel.AreaId = previewClass.AreaId;
                classVModel.CycleId = previewClass.CycleId;
                classVModel.ClassId = previewClass.ClassId;
                classVModel.IsSaved = false;
                classVModel.CountErrors = 0;
            }
            //
            ViewBag.Header = areaName + "【" + cycleName + "】"+ "巡檢單預覽";
            return View(classVModel);
        }

        // Get: Admin/DEInspectDocPreview/GetClassContent/5
        public ActionResult GetClassContent(string areaId, string cycleId, string classId)
        {
            int iAreaId = Convert.ToInt32(areaId);
            int iCycleId = Convert.ToInt32(cycleId);
            int iClassId = Convert.ToInt32(classId);
            ViewBag.ClassName = db.DEInspectClass.Find(iAreaId, iCycleId, iClassId).ClassName;
            // Get preview items and fields. 
            ViewData["itemsPreview"] = db.DEInspectItem.Where(i => i.AreaId == iAreaId && i.CycleId == iCycleId && i.ClassId == iClassId)
                                                       .Where(i => i.ItemStatus == true)
                                                       .OrderBy(i => i.ItemOrder).ToList();
            ViewData["fieldsPreview"] = db.DEInspectField.Where(i => i.AreaId == iAreaId && i.CycleId == iCycleId && i.ClassId == iClassId)
                                                         .Where(i => i.FieldStatus == true)                                         
                                                         .ToList();
            ViewData["dropdownsPreview"] = db.DEInspectFieldDropDown.Where(i => i.AreaId == iAreaId && i.CycleId == iCycleId && i.ClassId == iClassId)
                                                                    .ToList();
            return PartialView();
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
