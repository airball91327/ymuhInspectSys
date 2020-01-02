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
        public ActionResult Index(int docID)
        {
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            var theEditDoc = db.InspectDocs.Find(docID);
            int areaID = theEditDoc.AreaID;
            ViewBag.AreaID = areaID;
            ViewBag.AreaName = theEditDoc.InspectAreas.AreaName;
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

        // GET: Mobile/InspectDocEdit/ClassContentOfAreaEdit
        public ActionResult ClassContentOfAreaEdit(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaID = db.ClassesOfAreas.Find(ACID).AreaID;
            ViewBag.DocID = docID;
            ViewBag.ACID = ACID;

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

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocChecker/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.AreaID = db.ClassesOfAreas.Find(ACID).AreaID;
            ViewBag.DocID = docID;
            ViewBag.ACID = ACID;

            /* Find the data. */
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
                InspectDocDetails = inspectDocDetails.ToList(),
            };

            return View(inspectDocDetailsViewModels);
        }

        // POST: Mobile/InspectDocEdit/SaveData
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
                return RedirectToAction("Index", new { area = "Mobile", DocID = docID });
            }
            TempData["SaveMsg"] = "資料修改失敗";
            return RedirectToAction("Index", new { area = "Mobile", DocID = docID });
        }

        // GET: Mobile/InspectDocEdit/GetFlowList
        public ActionResult GetFlowList(int docID)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocID == docID).OrderBy(i => i.StepID);
            var findDoc = db.InspectDocs.Find(docID);

            foreach (var item in flowList)
            {
                if (item.StepOwnerID == item.WorkerID)
                {
                    item.StepOwnerName = findDoc.WorkerName;
                }
                else if (item.StepOwnerID == item.CheckerID)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return View(flowList.ToList());
        }

    }
}