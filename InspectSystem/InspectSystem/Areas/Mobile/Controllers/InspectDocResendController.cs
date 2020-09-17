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

namespace InspectSystem.Areas.Mobile.Controllers
{
    [Authorize]
    public class InspectDocResendController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // Select class for worker to view DocDetailsTemp.
        // GET: Mobile/InspectDocResend
        public ActionResult Index(int DocId)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId).ToList();
            var theEditDoc = db.InspectDocs.Find(DocId);
            int areaID = theEditDoc.AreaId;
            ViewBag.AreaId = areaID;
            ViewBag.AreaName = theEditDoc.AreaName;
            ViewBag.DocId = DocId;

            // If the temp data has been deleted.
            if(DocDetailList.Count == 0)
            {
                /* New DocDetailsTemp and add to DB, when first into system. */

                var allItems = db.InspectItems.Where(i => i.AreaId >= areaID &&
                                                          i.ItemStatus == true);
                var allFields = db.InspectFields.Where(i => i.AreaId >= areaID &&
                                                            i.FieldStatus == true);
                var insertFields =
                    from f in allFields
                    join i in allItems on f.AreaId equals i.AreaId
                    where f.ItemId == i.ItemId
                    select new
                    {
                        f.AreaId,
                        f.InspectAreas.AreaName,
                        f.ClassId,
                        f.InspectClasses.ClassName,
                        f.ItemId,
                        i.ItemName,
                        i.ItemOrder,
                        f.FieldId,
                        f.FieldName,
                        f.UnitOfData,
                        f.DataType,
                        f.MinValue,
                        f.MaxValue,
                        f.IsRequired
                    };
                //int countFields = insertFields.ToList().Count; // For Debug
                var inspectDocDetailsTemporary = new List<InspectDocDetailTemp>();
                foreach (var item in insertFields)
                {
                    string isFunctional = null; // Set default value.

                    string dropDownItems = null;
                    /* If field is dropdown, set dropdownlist items to string and save to DB. */
                    if (item.DataType == "dropdownlist")
                    {
                        int itemACID = (item.AreaId) * 100 + item.ClassId;
                        var itemDropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == item.AreaId &&
                                                                                  i.ClassId == item.ClassId &&
                                                                                  i.ItemId == item.ItemId &&
                                                                                  i.FieldId == item.FieldId).ToList();
                        foreach (var dropItem in itemDropDownList)
                        {
                            dropDownItems += dropItem.Value.ToString() + ";";
                        }
                    }

                    inspectDocDetailsTemporary.Add(new InspectDocDetailTemp()
                    {
                        DocId = DocId,
                        AreaId = item.AreaId,
                        AreaName = item.AreaName,
                        ClassId = item.ClassId,
                        ClassName = item.ClassName,
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        FieldId = item.FieldId,
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
            DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId).ToList();
            var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassId).Select(g => g.FirstOrDefault()).ToList();
            List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
            foreach (var itemClass in ClassesOfDocTemp)
            {
                var addClass = db.ClassesOfAreas.Where(c => c.AreaId == areaID && c.ClassId == itemClass.ClassId).FirstOrDefault();
                ClassList.Add(addClass);
            }
            var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

            /* Check all class is saved or not. */
            foreach (var item in ClassesOfAreas)
            {
                var findDocTemps = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId &&
                                                                            i.ClassId == item.ClassId);
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
                var toFindErrors = DocDetailList.Where(d => d.ClassId == item.ClassId &&
                                                           d.IsFunctional == "n");
                item.CountErrors = toFindErrors.Count();
            }

            return View(ClassesOfAreas.ToList());
        }

        // GET: Mobile/InspectDocResend/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaId = db.ClassesOfAreas.Find(ACID).AreaId;
            ViewBag.DocId = DocId;

            /* Find the temp data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassId;
            var inspectDocDetailsTemp = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId &&
                                                                                 i.ClassId == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetailsTemp.GroupBy(i => i.ItemId)
                                                             .Select(g => g.FirstOrDefault())
                                                             .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetailsTemp.ToList();

            InspectDocDetailViewModels inspectDocDetailsViewModels = new InspectDocDetailViewModels()
            {
                InspectDocDetailsTemporary = inspectDocDetailsTemp.ToList(),
            };

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocResend/ClassContentOfAreaEdit
        public ActionResult ClassContentOfAreaEdit(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaId = db.ClassesOfAreas.Find(ACID).AreaId;
            ViewBag.DocId = DocId;

            /* Find the doc details. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassId;
            var inspectDocDetailsTemp = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId &&
                                                                                 i.ClassId == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetailsTemp.GroupBy(i => i.ItemId)
                                                             .Select(g => g.FirstOrDefault())
                                                             .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetailsTemp.ToList();


            InspectDocDetailViewModels inspectDocDetailsViewModels = new InspectDocDetailViewModels()
            {
                InspectDocDetailsTemporary = inspectDocDetailsTemp,
            };

            return View(inspectDocDetailsViewModels);
        }

        // POST: Mobile/InspectDocResend/TempSave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TempSave(List<InspectDocDetailTemp> inspectDocDetailsTemporary)
        {
            var areaID = inspectDocDetailsTemporary.First().AreaId;
            var DocId = inspectDocDetailsTemporary.First().DocId;
            var classID = inspectDocDetailsTemporary.First().ClassId;

            if (ModelState.IsValid)
            {
                var findTemp = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId &&
                                                                        i.ClassId == classID);
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
                return RedirectToAction("Index", new { Area = "Mobile", DocId = DocId });
            }
            TempData["SaveMsg"] = "暫存失敗";
            return RedirectToAction("Index", new { Area = "Mobile", DocId = DocId });
        }

        // Select Class for worker to View DocTemp.
        // GET: Mobile/InspectDocResend/DocDetails
        public ActionResult DocDetails(int DocId)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId).ToList();
            int length = DocId.ToString().Length;
            int areaID = System.Convert.ToInt32(DocId.ToString().Substring(length - 2));

            if (DocDetailList.Count == 0)
            {
                TempData["SaveMsg"] = "尚未有資料儲存";
                return RedirectToAction("Index", new { Area = "Mobile", DocId = DocId });
            }
            else
            {
                ViewBag.AreaId = DocDetailList.First().AreaId;
                ViewBag.AreaName = DocDetailList.First().AreaName;
                ViewBag.DocId = DocId;

                /* Find Classes from DocDetails and set values to List<ClassesOfAreas> ClassList. */
                var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassId).Select(g => g.FirstOrDefault()).ToList();
                List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
                foreach (var itemClass in ClassesOfDocTemp)
                {
                    var addClass = db.ClassesOfAreas.Where(c => c.AreaId == areaID && c.ClassId == itemClass.ClassId).FirstOrDefault();
                    ClassList.Add(addClass);
                }
                var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

                /* Count errors for every class, and set count result to "CountErrors". */
                foreach (var item in ClassesOfAreas)
                {
                    var toFindErrors = DocDetailList.Where(d => d.ClassId == item.ClassId &&
                                                               d.IsFunctional == "n");
                    item.CountErrors = toFindErrors.Count();
                }

                return View(ClassesOfAreas.ToList());
            }
        }

        // GET: Mobile/InspectDocResend/FlowDocEdit
        public ActionResult FlowDocEdit(int DocId)
        {

            var findDoc = db.InspectDocs.Find(DocId);
            int areaID = findDoc.AreaId;
            int CheckerId = db.InspectAreaCheckers.Where(i => i.AreaId == areaID).First().CheckerId;
            var areaCheckers = db.InspectAreaCheckers.ToList();
            var areaCheckerNames = areaCheckers.GroupBy(a => a.CheckerName).Select(g => g.First()).ToList();

            SelectList areaCheckerSelectList = new SelectList(areaCheckerNames, "CheckerId", "CheckerName", CheckerId);
            ViewBag.AreaCheckerNames = areaCheckerSelectList;

            /* Set value to new first DocFlow. */
            InspectDocFlow DocFlow = new InspectDocFlow()
            {
                DocId = DocId,
                StepId = 1,
                StepOwnerId = findDoc.EngId,
                EngId = findDoc.EngId,
                CheckerId = findDoc.CheckerId,
                Opinions = "",
                FlowStatusId = 0,
                EditorId = findDoc.EngId,
                EditorName = findDoc.EngName,
                EditTime = null,
                InspectDocs = db.InspectDocs.Find(DocId)
            };
            return View(DocFlow);
        }

        // POST: Mobile/InspectDocResend/SendDocToChecker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendDocToChecker(InspectDocFlow inspectDocFlow)
        {
            /* Declear variables, and search data from DB. */
            int DocId = inspectDocFlow.DocId;
            var DocDetailTempList = db.InspectDocDetailsTemporary.Where(i => i.DocId == DocId).ToList();
            var areaID = DocDetailTempList.First().AreaId;
            var classID = DocDetailTempList.First().ClassId;
            List<InspectDocDetail> inspectDocDetails = new List<InspectDocDetail>();
            var findDocDetails = db.InspectDocDetails.Where(i => i.DocId == DocId);
            var findDoc = db.InspectDocs.Find(DocId);
            var CheckerId = System.Convert.ToInt32(Request.Form["AreaCheckerNames"]);

            /* Save all temp details to database. */
            /* Copy temp data to inspectDocDetails list. */
            foreach (var item in DocDetailTempList)
            {
                inspectDocDetails.Add(new InspectDocDetail()
                {
                    DocId = item.DocId,
                    AreaId = item.AreaId,
                    AreaName = item.AreaName,
                    ClassId = item.ClassId,
                    ClassName = item.ClassName,
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    FieldId = item.FieldId,
                    FieldName = item.FieldName,
                    UnitOfData = item.UnitOfData,
                    Value = item.Value,
                    IsFunctional = item.IsFunctional,
                    ErrorDescription = item.ErrorDescription,
                    RepairDocId = item.RepairDocId,
                    ItemOrder = item.ItemOrder,
                    DataType = item.DataType,
                    MinValue = item.MinValue,
                    MaxValue = item.MaxValue,
                    IsRequired = item.IsRequired,
                    DropDownItems = item.DropDownItems
                });
            }

            if (ModelState.IsValid)
            {
                if (findDocDetails.Count() == 0)
                {
                    foreach (var item in inspectDocDetails)
                    {
                        db.InspectDocDetails.Add(item);
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (var item in inspectDocDetails)
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                TempData["SaveMsg"] = "資料儲存失敗";
                return RedirectToAction("Index", new { Area = "Mobile", DocId = DocId });
            }

            /* Change flow status to "Checking" for this doc. */
            findDoc.FlowStatusId = 1;
            findDoc.EndTime = DateTime.UtcNow.AddHours(08);
            findDoc.CheckerId = CheckerId;
            findDoc.CheckerName = db.InspectAreaCheckers.Where(i => i.CheckerId == CheckerId).First().CheckerName;

            /* Set edit time and CheckerId for doc flow. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDocFlow.CheckerId = CheckerId;

            /* The next flow for checker. */
            InspectDocFlow NextDocFlow = new InspectDocFlow()
            {
                DocId = DocId,
                StepId = inspectDocFlow.StepId + 1,
                StepOwnerId = findDoc.CheckerId,
                EngId = findDoc.EngId,
                CheckerId = findDoc.CheckerId,
                Opinions = "",
                FlowStatusId = 1,
                EditorId = 0,
                EditorName = "",
                EditTime = null,
            };

            if (ModelState.IsValid)
            {
                db.InspectDocFlows.Add(inspectDocFlow);
                db.InspectDocFlows.Add(NextDocFlow);
                db.Entry(findDoc).State = EntityState.Modified;
                db.SaveChanges();
                var msg = "資料已傳送";
                return RedirectToAction("AfterSendDoc", new { Area = "Mobile", Msg = msg });
            }
            else
            {
                TempData["SaveMsg"] = "資料傳送失敗";
                return RedirectToAction("Index", new { Area = "Mobile", DocId = DocId });
            }
        }

        // GET: Mobile/InspectDocResend/AfterSendDoc
        public ActionResult AfterSendDoc(string Msg)
        {
            ViewBag.msg = Msg;
            return View();
        }

    }
}