using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocTempViewController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocTempView/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;

            /* Find the data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetailsTemp = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetailsTemp.GroupBy(i => i.ItemID)
                                                             .Select(g => g.FirstOrDefault())
                                                             .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetailsTemp.ToList();

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetailsTemporary = inspectDocDetailsTemp.ToList(),
            };

            return PartialView(inspectDocDetailsViewModels);
        }
    }
}