using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Areas.Mobile.Controllers
{
    [Authorize]
    public class InspectDocEditController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // Select class for worker to view DocDetails.
        // GET: Mobile/InspectDocEdit
        public ActionResult Index(int DocId)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocId == DocId).ToList();
            var theEditDoc = db.InspectDocs.Find(DocId);
            int areaID = theEditDoc.AreaId;
            ViewBag.AreaId = areaID;
            ViewBag.AreaName = theEditDoc.AreaName;
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

        // GET: Mobile/InspectDocEdit/ClassContentOfAreaEdit
        public ActionResult ClassContentOfAreaEdit(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaId = db.ClassesOfAreas.Find(ACID).AreaId;
            ViewBag.DocId = DocId;
            ViewBag.ACID = ACID;

            /* Find the doc details. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassId;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocId == DocId &&
                                                                    i.ClassId == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetails.GroupBy(i => i.ItemId)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetails.ToList();

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails,
            };

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocChecker/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaId = db.ClassesOfAreas.Find(ACID).AreaId;
            ViewBag.DocId = DocId;
            ViewBag.ACID = ACID;

            /* Find the data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassId;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocId == DocId &&
                                                                    i.ClassId == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetails.GroupBy(i => i.ItemId)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetails.ToList();

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails.ToList(),
            };

            return View(inspectDocDetailsViewModels);
        }

        // POST: Mobile/InspectDocEdit/SaveData
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveData(List<InspectDocDetails> inspectDocDetails)
        {
            var areaID = inspectDocDetails.First().AreaId;
            int DocId = inspectDocDetails.First().DocId;

            if (ModelState.IsValid)
            {
                foreach (var item in inspectDocDetails)
                {
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                TempData["SaveMsg"] = "資料修改完成";
                return RedirectToAction("Index", new { area = "Mobile", DocId = DocId });
            }
            TempData["SaveMsg"] = "資料修改失敗";
            return RedirectToAction("Index", new { area = "Mobile", DocId = DocId });
        }

        // GET: Mobile/InspectDocEdit/GetFlowList
        public ActionResult GetFlowList(int DocId)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocId == DocId).OrderBy(i => i.StepId);
            var findDoc = db.InspectDocs.Find(DocId);

            foreach (var item in flowList)
            {
                if (item.StepOwnerId == item.EngId)
                {
                    item.StepOwnerName = findDoc.EngName;
                }
                else if (item.StepOwnerId == item.CheckerId)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return View(flowList.ToList());
        }

    }
}