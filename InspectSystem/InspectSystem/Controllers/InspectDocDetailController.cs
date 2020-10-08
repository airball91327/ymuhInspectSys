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
    public class InspectDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocDetail
        public async Task<ActionResult> Index()
        {
            var inspectDocDetail = db.InspectDocDetail.Include(i => i.InspectDocs);
            return View(await inspectDocDetail.ToListAsync());
        }

        // Get: InspectDocDetail/GetShiftViews/5
        public ActionResult GetShiftViews(string docId, string shiftId)
        {
            int iShiftId = Convert.ToInt32(shiftId);
            // Get inspect DocDetail list.
            var docDetail = db.InspectDocDetail.Where(t => t.DocId == docId && t.ShiftId == iShiftId).ToList();
            if (docDetail.Count() > 0)
            {
                ViewBag.ShiftName = docDetail.First().ShiftName;
                // Get classes, items and fields from DocDetail list. 
                ViewData["classesOfDocDetails"] = docDetail.GroupBy(i => i.ClassId)
                                                 .Select(g => g.FirstOrDefault())
                                                 .OrderBy(s => s.ClassOrder).ToList();
                ViewData["itemsOfDocDetails"] = docDetail.GroupBy(i => i.ItemId)
                                                                 .Select(g => g.FirstOrDefault())
                                                                 .OrderBy(s => s.ItemOrder).ToList();
                ViewData["fieldsOfDocDetails"] = docDetail.ToList();
            }

            InspectDocDetailViewModel inspectDocDetailViewModel = new InspectDocDetailViewModel()
            {
                InspectDocDetail = docDetail
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
