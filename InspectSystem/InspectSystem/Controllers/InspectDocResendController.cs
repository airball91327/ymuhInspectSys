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
using System.Web.Security;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocResendController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocResend
        public ActionResult Index(int docID)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            var theEditDoc = db.InspectDocs.Find(docID);
            int areaID = theEditDoc.AreaID;
            ViewBag.AreaID = areaID;
            ViewBag.AreaName = theEditDoc.InspectAreas.AreaName;
            ViewBag.DocID = docID;

            // If the temp data has been deleted.
            if (DocDetailList.Count == 0)
            {
                /* New DocDetailsTemp and add to DB, when first into system. */
                int firstACID = areaID * 100 + 1;
                int nextACID = firstACID + 100;

                var allItems = db.InspectItems.Where(i => i.ACID >= firstACID &&
                                                          i.ACID < nextACID &&
                                                          i.ItemStatus == true);
                var allFields = db.InspectFields.Where(i => i.ACID >= firstACID &&
                                                            i.ACID < nextACID &&
                                                            i.FieldStatus == true);
                var insertFields =
                    from f in allFields
                    join i in allItems on f.ACID equals i.ACID
                    where f.ItemID == i.ItemID
                    select new
                    {
                        f.ClassesOfAreas.AreaID,
                        f.ClassesOfAreas.InspectAreas.AreaName,
                        f.ClassesOfAreas.ClassID,
                        f.ClassesOfAreas.InspectClasses.ClassName,
                        f.ItemID,
                        i.ItemName,
                        i.ItemOrder,
                        f.FieldID,
                        f.FieldName,
                        f.UnitOfData,
                        f.DataType,
                        f.MinValue,
                        f.MaxValue,
                        f.IsRequired
                    };
                //int countFields = insertFields.ToList().Count; // For Debug
                var inspectDocDetailsTemporary = new List<InspectDocDetailsTemporary>();
                foreach (var item in insertFields)
                {
                    string isFunctional = null; // Set default value.

                    string dropDownItems = null;
                    /* If field is dropdown, set dropdownlist items to string and save to DB. */
                    if (item.DataType == "dropdownlist")
                    {
                        int itemACID = (item.AreaID) * 100 + item.ClassID;
                        var itemDropDownList = db.InspectFieldDropDown.Where(i => i.ACID == itemACID &&
                                                                              i.ItemID == item.ItemID &&
                                                                              i.FieldID == item.FieldID).ToList();
                        foreach (var dropItem in itemDropDownList)
                        {
                            dropDownItems += dropItem.Value.ToString() + ";";
                        }
                    }

                    inspectDocDetailsTemporary.Add(new InspectDocDetailsTemporary()
                    {
                        DocID = docID,
                        AreaID = item.AreaID,
                        AreaName = item.AreaName,
                        ClassID = item.ClassID,
                        ClassName = item.ClassName,
                        ItemID = item.ItemID,
                        ItemName = item.ItemName,
                        FieldID = item.FieldID,
                        FieldName = item.FieldName,
                        UnitOfData = item.UnitOfData,
                        IsFunctional = isFunctional,
                        ItemOrder = item.ItemOrder,
                        DataType = item.DataType,
                        MinValue = item.MinValue,
                        MaxValue = item.MaxValue,
                        IsRequired = item.IsRequired,
                        DropDownItems = dropDownItems
                    });
                }
                /* Insert data to DocTemp DB. */
                foreach (var item in inspectDocDetailsTemporary)
                {
                    db.InspectDocDetailsTemporary.Add(item);
                }
                db.SaveChanges();
            }

            /* Find Classes from DocDetailsTemp and set values to List<ClassesOfAreas> ClassList. */
            DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassID).Select(g => g.FirstOrDefault()).ToList();
            List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
            foreach (var itemClass in ClassesOfDocTemp)
            {
                var addClass = db.ClassesOfAreas.Where(c => c.AreaID == areaID && c.ClassID == itemClass.ClassID).FirstOrDefault();
                ClassList.Add(addClass);
            }
            var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

            /* Check all class is saved or not. */
            foreach (var item in ClassesOfAreas)
            {
                var findDocTemps = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                            i.ClassID == item.ClassID);
                /* If find temp save of the class, set IsSaved to true. */
                if (findDocTemps.Count() != 0)
                {
                    Boolean isDataCompleted = true;
                    /* Check are all the required fields having data. */
                    foreach (var tempItem in findDocTemps)
                    {
                        // If required field has no data or isFunctional didn't selected.
                        if (tempItem.IsRequired == true && tempItem.DataType != "boolean" && tempItem.Value == null)
                        {
                            isDataCompleted = false;
                            break;
                        }
                        else if (tempItem.DataType == "boolean" && tempItem.IsFunctional == null)
                        {
                            isDataCompleted = false;
                            break;
                        }
                    }
                    if (isDataCompleted == true)
                    {
                        item.IsSaved = true;
                    }
                }
                else
                {
                    item.IsSaved = false;
                }
            }

            var isAllSaved = ClassesOfAreas.Where(c => c.IsSaved == false).ToList();
            if (isAllSaved.Count() == 0)
            {
                ViewBag.AllSaved = "true";
            }
            else
            {
                ViewBag.AllSaved = "false";
            }
            /* Count errors for every class, and set count result to "CountErrors". */
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassID == item.ClassID &&
                                                           d.IsFunctional == "n");
                item.CountErrors = toFindErrors.Count();
            }

            return View(ClassesOfAreas.ToList());
        }

        // POST: InspectDocResend/TempSave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TempSave(List<InspectDocDetailsTemporary> inspectDocDetailsTemporary)
        {
            var areaID = inspectDocDetailsTemporary.First().AreaID;
            var docID = inspectDocDetailsTemporary.First().DocID;
            var classID = inspectDocDetailsTemporary.First().ClassID;

            if (ModelState.IsValid)
            {
                var findTemp = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                        i.ClassID == classID);
                /* If can't find temp data, insert data to database. */
                if (findTemp.Any() == false)
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.InspectDocDetailsTemporary.Add(item);
                    }
                }
                else
                {
                    foreach (var item in inspectDocDetailsTemporary)
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "暫存完成";
                return RedirectToAction("Index", new { DocID = docID });
            }
            TempData["SaveMsg"] = "暫存失敗";
            return RedirectToAction("Index", new { DocID = docID });
        }

        // GET: InspectDocResend/DocDetails
        public ActionResult DocDetails(int docID)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            int length = docID.ToString().Length;
            int areaID = System.Convert.ToInt32(docID.ToString().Substring(length - 2));

            if (DocDetailList.Count == 0)
            {
                TempData["SaveMsg"] = "尚未有資料儲存";
                return RedirectToAction("Index", new { DocID = docID });
            }
            else
            {
                ViewBag.AreaID = DocDetailList.First().AreaID;
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
}