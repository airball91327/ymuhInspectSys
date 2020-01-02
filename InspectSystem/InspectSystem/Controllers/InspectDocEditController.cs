using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocEditController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocEdit
        public ActionResult Index(int docID)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            var theEditDoc = db.InspectDocs.Find(docID);
            int areaID = theEditDoc.AreaID;
            ViewBag.AreaID = areaID;
            ViewBag.AreaName = theEditDoc.AreaName;
            ViewBag.DocID = docID;

            /* Find Classes from DocDetails and set values to List<ClassesOfAreas> ClassList. */
            var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassID).Select(g => g.FirstOrDefault()).ToList();
            List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
            foreach (var itemClass in ClassesOfDocTemp)
            {
                var addClass = db.ClassesOfAreas.Where(c => c.AreaID == areaID && c.ClassID == itemClass.ClassID).FirstOrDefault();
                ClassList.Add(addClass);
            }
            var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

            /* Count errors for every class, and set count result to "CountErrors". */
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassID == item.ClassID &&
                                                           d.IsFunctional == "n");
                item.CountErrors = toFindErrors.Count();
            }
            return View(ClassesOfAreas.ToList());
        }

        // GET: InspectDocEdit/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;

            /* Find the doc details. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetails.GroupBy(i => i.ItemID)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetails.ToList();

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails,
            };

            return PartialView(inspectDocDetailsViewModels);
        }

        // POST: InspectDocEdit/SaveData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveData(List<InspectDocDetails> inspectDocDetails)
        {
            var areaID = inspectDocDetails.First().AreaID;
            int docID = inspectDocDetails.First().DocID;

            if (ModelState.IsValid)
            {
                foreach (var item in inspectDocDetails)
                {
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "資料修改完成";
                return RedirectToAction("Index", new { DocID = docID });
            }
            TempData["SaveMsg"] = "資料修改失敗";
            return RedirectToAction("Index", new { DocID = docID });
        }

        //GET: InspectDocEdit/CheckValue
        /* Use ajax to check the min and max value for fields. */
        public ActionResult CheckValue(int DocID, int ClassID, int ItemID, int FieldID, string Value)
        {
            /* Get the min and max value for the check field. */
            var SearchDoc = db.InspectDocDetails.Find(DocID, ClassID, ItemID, FieldID);
            var FieldDataType = SearchDoc.DataType;
            float MaxValue = System.Convert.ToSingle(SearchDoc.MaxValue);
            float MinValue = System.Convert.ToSingle(SearchDoc.MinValue);

            /* Only float type will check. */
            string msg = "";
            if (FieldDataType == "float")
            {
                /* Check the input string can be convert to float. */
                if (Single.TryParse(Value, out float inputValue))
                {
                    // Check max and min value, and if doesn't set the min or max value, return nothing.
                    if (inputValue >= MaxValue && MaxValue != 0)
                    {
                        msg = "<span style='color:red'>大於正常數值</span>";
                    }
                    else if (inputValue <= MinValue && MinValue != 0)
                    {
                        msg = "<span style='color:red'>小於正常數值</span>";
                    }
                    else if (MinValue == 0 && MaxValue == 0)
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

        // GET: InspectDocDetails/DocDetails
        public ActionResult DocDetails(int docID)
        {
            var theEditDoc = db.InspectDocs.Find(docID);
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            int areaID = theEditDoc.AreaID;

            ViewBag.AreaID = areaID;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocID = docID;

            /* Find Classes from DocDetails and set values to List<ClassesOfAreas> ClassList. */
            var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassID).Select(g => g.FirstOrDefault()).ToList();
            List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
            foreach (var itemClass in ClassesOfDocTemp)
            {
                var addClass = db.ClassesOfAreas.Where(c => c.AreaID == areaID && c.ClassID == itemClass.ClassID).FirstOrDefault();
                ClassList.Add(addClass);
            }
            var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

            /* Count errors for every class, and set count result to "CountErrors". */
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassID == item.ClassID &&
                                                           d.IsFunctional == "n");
                item.CountErrors = toFindErrors.Count();
            }
            return View(ClassesOfAreas.ToList());
        }

    }
}