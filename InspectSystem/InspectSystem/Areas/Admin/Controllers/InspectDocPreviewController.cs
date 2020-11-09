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
    [Authorize(Roles = "Admin")]
    public class InspectDocPreviewController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectDocPreview
        public async Task<ActionResult> Index()
        {
            // Get inspect area list.
            var inspectAreas = await db.InspectArea.Where(a => a.AreaStatus == true).ToListAsync();
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

        // POST: Admin/InspectDocPreview/Index/5
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var areaId = fc["AreaId"];
            var shiftId = fc["ShiftId"];
            return RedirectToAction("Preview", new { AreaId = areaId, ShiftId = shiftId });
        }

        // GET: Admin/InspectDocPreview/Preview/5
        public ActionResult Preview(int? AreaId, int? ShiftId)
        {
            if (AreaId == null || ShiftId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var shiftsInAreas = db.ShiftsInAreas.Include(s => s.InspectArea).Include(s => s.InspectShift)
                                                .Where(s => s.AreaId == AreaId && s.ShiftId == ShiftId).ToList();
            // Set variables.
            var shiftId = ShiftId;
            var previewClasses = db.InspectClass.Where(c => c.AreaId == AreaId && c.ShiftId == ShiftId)
                                                       .OrderBy(c => c.ClassOrder);
            var shiftName = shiftsInAreas.FirstOrDefault().InspectShift.ShiftName;
            var areaName = shiftsInAreas.FirstOrDefault().InspectArea.AreaName;
            List<InspectClassVModel> inspectClassVs = new List<InspectClassVModel>();
            InspectClassVModel classVModel;
            //
            foreach (var item in previewClasses)
            {
                // Insert values to classVModel.
                classVModel = new InspectClassVModel();
                classVModel.DocId = "00000000";
                classVModel.AreaId = item.AreaId;
                classVModel.ShiftId = item.ShiftId;
                classVModel.ClassId = item.ClassId;
                classVModel.ClassName = item.ClassName;
                classVModel.ClassOrder = item.ClassOrder;
                classVModel.IsSaved = false;
                classVModel.CountErrors = 0;
                inspectClassVs.Add(classVModel);
            }
            //
            ViewBag.Header = areaName + "【" + shiftName + "】";
            return View(inspectClassVs);
        }

        // Get: Admin/InspectDocPreview/GetClassContent/5
        public ActionResult GetClassContent(string areaId, string shiftId, string classId)
        {
            int iAreaId = Convert.ToInt32(areaId);
            int iShiftId = Convert.ToInt32(shiftId);
            int iClassId = Convert.ToInt32(classId);
            ViewBag.ClassName = db.InspectClass.Find(iAreaId, iShiftId, iClassId).ClassName;
            // Get preview items and fields. 
            ViewData["itemsPreview"] = db.InspectItem.Where(i => i.AreaId == iAreaId && i.ShiftId == iShiftId && i.ClassId == iClassId)
                                                     .OrderBy(i => i.ItemOrder).ToList();
            ViewData["fieldsPreview"] = db.InspectField.Where(i => i.AreaId == iAreaId && i.ShiftId == iShiftId && i.ClassId == iClassId)
                                                       .ToList();
            ViewData["dropdownsPreview"] = db.InspectFieldDropDown.Where(i => i.AreaId == iAreaId && i.ShiftId == iShiftId && i.ClassId == iClassId)
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
