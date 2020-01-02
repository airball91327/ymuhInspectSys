using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;


namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocSearchController : Controller
    {

        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocSearch
        public ActionResult Index()
        {
            ViewBag.AreaID = new SelectList(db.InspectAreas, "AreaID", "AreaName");
            ViewBag.FlowStatusID = new SelectList(db.InspectFlowStatusTable, "FlowStatusID", "FlowStatusName");
            return View();
        }

        // GET: InspectDocSearch/GetData          
        public JsonResult GetData(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  DateTime startDate, DateTime endDate, int? areaId, int? flowStatusId)
                                  //↑為表單的查詢條件
        {
            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //jQuery DataTable的Column index
            string col_index = Request.QueryString["order[0][column]"];
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "Date" : Request.QueryString[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(Request.QueryString["order[0][dir]"]) ? "asc" : Request.QueryString["order[0][dir]"];//防呆

            try
            {
                /* 查詢日期 */
                int fromDate = System.Convert.ToInt32(startDate.ToString("yyyyMMdd"));
                int toDate = System.Convert.ToInt32(endDate.ToString("yyyyMMdd"));
                int fromDoc, toDoc;     // Set doc search range.
                if(fromDate > toDate)
                {
                    fromDoc = ( toDate * 100 ) + 1;
                    toDoc = ( fromDate * 100 ) + 99;
                }
                else
                {
                    fromDoc = (fromDate * 100) + 1;
                    toDoc = (toDate * 100) + 99;
                }
                var searchList = db.InspectDocs.Where(i => i.DocID >= fromDoc && i.DocID <= toDoc);

                /* 查詢區域 */
                if (areaId != null)
                {
                    searchList = searchList.Where(r => r.AreaID == areaId);
                }
                /* 查詢文件狀態 */
                if(flowStatusId != null)
                {
                    searchList = searchList.Where(r => r.FlowStatusID == flowStatusId);
                }

                var resultList = searchList.AsEnumerable().Select(s => new
                {
                    AreaName = s.InspectAreas.AreaName,
                    FlowStatusName = s.InspectFlowStatusTable.FlowStatusName,
                    Date = s.Date.ToString("yyyy/MM/dd"),       // ToString() is not supported in Linq to Entities, 
                    WorkerID = s.WorkerID,                      // need to change type to IEnumerable by using AsEnumerable(),
                    WorkerName = s.WorkerName,                  // and then can use ToString(), 
                    CheckerID = s.CheckerID,                    // because AsEnumerable() is Linq to Objects.
                    CheckerName = s.CheckerName,
                    DocID = s.DocID,
                    AreaID = s.AreaID,
                    FlowStatusID = s.FlowStatusID
                }).ToList();

                // Deal DataTable sorting. 
                if(sortColName == "AreaName")
                {
                    sortColName = "AreaID";
                }
                else if(sortColName == "FlowStatusName")
                {
                    sortColName = "FlowStatusID";
                }
                resultList = resultList.AsEnumerable().OrderBy($@"{sortColName} {asc_desc}").ToList();

                recordsTotal = resultList.Count();//查詢後的總筆數

                //回傳Json資料
                var returnObj =
                    new
                    {
                      draw = draw,
                      recordsTotal = recordsTotal,
                      recordsFiltered = recordsTotal,
                      data = resultList.Skip(start ?? 0).Take(length)   //分頁後的資料 
                    };
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: InspectDocSearch/DocDetailsIndex
        public ActionResult DocDetailsIndex(int docID)
        {
            /* Set variables from DB. */
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocID == docID).ToList();
            int length = docID.ToString().Length;
            int areaID = System.Convert.ToInt32(docID.ToString().Substring(length - 2));

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

        // GET: InspectDocSearch/DocTempIndex
        public ActionResult DocTempIndex(int docID)
        {
            var DocDetailList = db.InspectDocDetailsTemporary.Where(i => i.DocID == docID).ToList();
            int length = docID.ToString().Length;
            int areaID = System.Convert.ToInt32(docID.ToString().Substring(length - 2));

            if (DocDetailList.Count == 0)
            {
                TempData["SaveMsg"] = "尚未有資料儲存";
                return RedirectToAction("Index", new { AreaID = areaID });
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