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
    public class SearchController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        // GET: Search/InspectDocIndex
        public ActionResult InspectDocIndex()
        {
            ViewBag.AreaId = new SelectList(db.InspectArea, "AreaId", "AreaName");
            ViewBag.DocStatusId = new SelectList(db.InspectDocStatus, "DocStatusId", "DocStatusDes");
            return View();
        }

        // POST: Search/GetDocs         
        [HttpPost]
        public JsonResult GetDocs(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  FormCollection form)
        {
            var qryStartDate = form["startDate"];
            var qryEndDate = form["endDate"];
            var qryAreaId = form["AreaId"];
            var qryDocStatusId = form["DocStatusId"];

            DateTime applyDateFrom = DateTime.Now;
            DateTime applyDateTo = DateTime.Now;
            /* Dealing search by date. */
            if (!string.IsNullOrEmpty(qryStartDate) && !string.IsNullOrEmpty(qryEndDate))// If 2 date inputs have been insert, compare 2 dates.
            {
                DateTime date1 = DateTime.Parse(qryStartDate);
                DateTime date2 = DateTime.Parse(qryEndDate);
                int result = DateTime.Compare(date1, date2);
                if (result < 0)
                {
                    applyDateFrom = date1.Date;
                    applyDateTo = date2.Date;
                }
                else if (result == 0)
                {
                    applyDateFrom = date1.Date;
                    applyDateTo = date1.Date;
                }
                else
                {
                    applyDateFrom = date2.Date;
                    applyDateTo = date1.Date;
                }
            }
            else if (string.IsNullOrEmpty(qryStartDate) && !string.IsNullOrEmpty(qryEndDate))
            {
                applyDateFrom = DateTime.Parse(qryEndDate);
                applyDateTo = DateTime.Parse(qryEndDate);
            }
            else if (!string.IsNullOrEmpty(qryStartDate) && string.IsNullOrEmpty(qryEndDate))
            {
                applyDateFrom = DateTime.Parse(qryStartDate);
                applyDateTo = DateTime.Parse(qryStartDate);
            }

            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //jQuery DataTable的Column index
            string col_index = form["order[0][column]"];
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "ApplyDate" : form[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(form["order[0][dir]"]) ? "asc" : form["order[0][dir]"];//防呆

            var searchList = db.InspectDocIdTable.Include(i => i.InspectDoc).Include(i => i.InspectDocFlow)
                                                 .Include(i => i.InspectDocStatus);
            /* 查詢日期 */
            if (string.IsNullOrEmpty(qryStartDate) == false || string.IsNullOrEmpty(qryEndDate) == false)
            {
                searchList = searchList.Where(v => v.ApplyDate >= applyDateFrom && v.ApplyDate <= applyDateTo);
            }
            if (!string.IsNullOrEmpty(qryAreaId))  /* 查詢區域 */
            {
                var areaid = Convert.ToInt32(qryAreaId);
                searchList = searchList.Where(r => r.AreaId == areaid);
            }
            if (!string.IsNullOrEmpty(qryDocStatusId))  /* 查詢文件狀態 */
            {
                searchList = searchList.Where(r => r.DocStatusId == qryDocStatusId);
            }

            var resultList = searchList.ToList().Select(s => new InspectDocIdTableVModel()
            {
                DocId = s.DocId,
                ApplyDate = s.ApplyDate.ToString("yyyy/MM/dd"),
                CloseDate = s.CloseDate.HasValue == false ? null : s.CloseDate.Value.ToString("yyyy/MM/dd"),
                AreaId = s.AreaId,
                AreaName = s.AreaName,
                ShiftId = s.ShiftId,
                ShiftName = db.InspectShift.Find(s.ShiftId).ShiftName,
                DocStatusId = s.DocStatusId,
                DocStatusDes = s.InspectDocStatus.DocStatusDes,
                FlowStatusId = s.InspectDocFlow.OrderByDescending(f => f.StepId).FirstOrDefault().FlowStatusId,
                FlowStatusDes = s.InspectDocFlow.OrderByDescending(f => f.StepId).FirstOrDefault().InspectFlowStatus.FlowStatusDes,
                EngUserName = db.AppUsers.Find(s.InspectDoc.OrderByDescending(d => d.ShiftId).FirstOrDefault().EngId).UserName,
                EngFullName = s.InspectDoc.OrderByDescending(d => d.ShiftId).FirstOrDefault().EngName
            }).ToList();

            // Deal DataTable sorting. 
            if (sortColName == "AreaName")
            {
                sortColName = "AreaId";
            }
            else if (sortColName == "FlowStatusDes")
            {
                sortColName = "FlowStatusId";
            }
            else if (sortColName == "DocStatusDes")
            {
                sortColName = "DocStatusId";
            }
            else if (sortColName == "ShiftName")
            {
                sortColName = "ShiftId";
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

    }
}