﻿using System;
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
            ViewBag.FlowStatusId = new SelectList(db.InspectFlowStatus, "FlowStatusId", "FlowStatusName");
            return View();
        }

        // GET: Search/GetDocs         
        public JsonResult GetDocs(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
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
                var searchList = db.InspectDoc.Where(i => i.DocID >= fromDoc && i.DocID <= toDoc);

                /* 查詢區域 */
                if (areaId != null)
                {
                    searchList = searchList.Where(r => r.AreaID == areaId);
                }
                /* 查詢文件狀態 */
                if (flowStatusId != null)
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
                if (sortColName == "AreaName")
                {
                    sortColName = "AreaID";
                }
                else if (sortColName == "FlowStatusName")
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

    }
}