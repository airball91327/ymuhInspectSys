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
    public class DEInspectDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: DEInspectDocDetail
        public async Task<ActionResult> Index()
        {
            var inspectDocDetail = db.DEInspectDocDetail.Include(i => i.DEInspectDocs);
            return View(await inspectDocDetail.ToListAsync());
        }

        // Get: DEInspectDocDetail/Views/5
        public ActionResult Views(string docId)
        {
            var docDetail = db.DEInspectDocDetail.Where(t => t.DocId == docId).ToList();
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
                InspectDocDetail = docDetail,
                InspectItems = items,
                InspectFields = fields,
                InspectFieldDropDowns = dropdowns
            };

            return PartialView(inspectDocDetailViewModel);
        }

        /// <summary>
        /// Check the field input value is in the set range or not.
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="shiftId"></param>
        /// <param name="classId"></param>
        /// <param name="itemId"></param>
        /// <param name="fieldId"></param>
        /// <param name="value"></param>
        /// <returns>string of the html to display the message.</returns>
        //GET: DEInspectDocDetail/CheckValue
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
