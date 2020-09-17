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
        public ActionResult ClassContentOfArea(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;

            /* Find the data. */
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

            return PartialView(inspectDocDetailsViewModels);
        }
    }
}