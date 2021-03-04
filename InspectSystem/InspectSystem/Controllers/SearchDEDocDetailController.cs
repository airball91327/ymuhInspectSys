using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using InspectSystem.Models;
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class SearchDEDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: SearchDEDocDetail
        public ActionResult Index()
        {
            ViewBag.AreaID = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            return View();
        }

        // POST: SearchDEDocDetail/GetData  
        [HttpPost]
        public JsonResult GetData(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  FormCollection form)
        {
            var qryStartDate = form["startDate"];
            var qryEndDate = form["endDate"];
            var qryAreaId = form["AreaId"];
            var qryCycleId = form["CycleId"];
            var qryClassId = form["ClassId"];
            var qryItemId = form["ItemId"];
            var qryFieldId = form["FieldId"];
            //
            DEDocDetailQryVModel query = new DEDocDetailQryVModel();
            query.StartDate = qryStartDate;
            query.EndDate = qryEndDate;
            query.AreaId = qryAreaId;
            query.CycleId = qryCycleId;
            query.ClassId = qryClassId;
            query.ItemId = qryItemId;
            query.FieldId = qryFieldId;

            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //jQuery DataTable的Column index
            string col_index = form["order[0][column]"];
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "ApplyDate" : form[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(form["order[0][dir]"]) ? "asc" : form["order[0][dir]"];//防呆

            var qryList = GetDocDetails(query);
            var searchList = qryList.Join(db.DEInspectDoc, dtl => dtl.DocId, d => d.DocId,
                                    (dtl, d) => new
                                    {
                                        doc = d,
                                        docdtl = dtl
                                    });
            /* 處理儲存正常或不正常的欄位，把Value拿來顯示是否正常. */
            foreach (var item in searchList)
            {
                if (item.docdtl.DataType == "boolean")
                {
                    item.docdtl.Value = item.docdtl.IsFunctional;
                }
            }

            var resultList = searchList.Select(s => new 
            {
                ApplyDate = s.doc.ApplyDate.ToString("yyyy/MM/dd"),
                AreaName = s.docdtl.AreaName,
                CycleName = s.docdtl.CycleName,
                ClassName = s.docdtl.ClassName,
                ItemName = s.docdtl.ItemName,
                FieldName = s.docdtl.FieldName,
                Value = s.docdtl.Value,
                UnitOfData = s.docdtl.UnitOfData,
                DocId = s.docdtl.DocId,
                AreaId = s.docdtl.AreaId
            }).ToList();

            // Deal DataTable sorting. 
            resultList = resultList.AsEnumerable().OrderBy($@"{sortColName} {asc_desc}").ToList();

            recordsTotal = resultList.Count();//查詢後的總筆數

            // 分頁處理
            if (length != -1) //-1為顯示所有資料
            {
                resultList = resultList.Skip(start ?? 0).Take(length).ToList();   //分頁後的資料 
            }

            //回傳Json資料
            var returnObj =
                new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = resultList
                };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        // POST: SearchDEDocDetail/GetCycles
        [HttpPost]
        public JsonResult GetCycles(int AreaId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cycles = db.DECyclesInAreas.Include(s => s.DEInspectCycle);
            cycles.Where(s => s.AreaId == AreaId).OrderBy(s => s.CycleId).ToList()
                  .ForEach(c => {
                      list.Add(new SelectListItem
                      {
                          Text = c.DEInspectCycle.CycleName,
                          Value = c.CycleId.ToString()
                      });
                  });
            return Json(list);
        }

        // POST: SearchDEDocDetail/GetClasses
        [HttpPost]
        public JsonResult GetClasses(int AreaId, int cycleId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            db.DEInspectClass.Where(c => c.AreaId == AreaId && c.CycleId == cycleId)
                             .OrderBy(c => c.ClassOrder).ToList()
                             .ForEach(c => {
                                 list.Add(new SelectListItem
                                 {
                                     Text = c.ClassName,
                                     Value = c.ClassId.ToString()
                                 });
                             });
            return Json(list);
        }

        // POST: SearchDEDocDetail/GetItems
        [HttpPost]
        public JsonResult GetItems(int AreaId,int cycleId, string ClassId)
        {
            int classId = System.Convert.ToInt32(ClassId);
            List<SelectListItem> list = new List<SelectListItem>();
            db.DEInspectItem.Where(i => i.AreaId == AreaId && i.CycleId == cycleId)
                            .Where(i => i.ClassId == classId)
                            .OrderBy(i => i.ItemOrder).ToList()
                            .ForEach(i => {
                                list.Add(new SelectListItem
                                {
                                    Text = i.ItemName,
                                    Value = i.ItemId.ToString()
                                });
                            });
            return Json(list);
        }

        // POST: SearchDEDocDetail/GetFields
        [HttpPost]
        public JsonResult GetFields(int AreaId, int cycleId, string ClassId, string ItemId)
        {
            int classId = System.Convert.ToInt32(ClassId);
            int itemId = System.Convert.ToInt32(ItemId);
            List<SelectListItem> list = new List<SelectListItem>();
            db.DEInspectField.Where(i => i.AreaId == AreaId && i.CycleId == cycleId)
                             .Where(i => i.ClassId == classId && i.ItemId == itemId)
                             .OrderBy(i => i.ItemId).ToList()
                             .ForEach(i => {
                                 if (i.FieldName != null && i.FieldName != "" && i.FieldStatus != false)   //擷取有輸入名稱、後台設定為顯示的欄位
                                 {
                                     list.Add(new SelectListItem
                                     {
                                         Text = i.FieldName,
                                         Value = i.FieldId.ToString()
                                     });
                                 }
                             });
            return Json(list);
        }

        // GET: SearchDEDocDetail/ExportToExcel
        public ActionResult ExportToExcel(DEDocDetailQryVModel qry)
        {
            var qryList = GetDocDetails(qry);
            var searchList = qryList.Join(db.DEInspectDoc, dtl => dtl.DocId, d => d.DocId,
                                    (dtl, d) => new
                                    {
                                        doc = d,
                                        docdtl = dtl
                                    });

            /* 處理儲存正常或不正常的欄位，把Value拿來顯示是否正常. */
            foreach (var item in searchList)
            {
                if (item.docdtl.DataType == "boolean")
                {
                    item.docdtl.Value = item.docdtl.IsFunctional;
                }
            }

            var resultList = searchList.Select(s => new
            {
                ApplyDate = s.doc.ApplyDate.ToString("yyyy/MM/dd"),
                AreaName = s.docdtl.AreaName,
                CycleName = s.docdtl.CycleName,
                ClassName = s.docdtl.ClassName,
                ItemName = s.docdtl.ItemName,
                FieldName = s.docdtl.FieldName,
                Value = s.docdtl.Value,
                UnitOfData = s.docdtl.UnitOfData,
                DocId = s.docdtl.DocId,
                AreaId = s.docdtl.AreaId,
                IsFunctional = s.docdtl.IsFunctional,
                ErrorDescription = s.docdtl.ErrorDescription
            }).ToList();

            //ClosedXML的用法 先new一個Excel Workbook
            using (XLWorkbook workbook = new XLWorkbook())
            {
                //取得要塞入Excel內的資料
                var data = searchList.Select(c => new
                {
                    c.docdtl.DocId,
                    c.doc.ApplyDate,
                    c.docdtl.AreaName,
                    c.docdtl.CycleName,
                    c.docdtl.ClassName,
                    c.docdtl.ItemName,
                    c.docdtl.FieldName,
                    c.docdtl.Value,
                    c.docdtl.UnitOfData,
                    c.docdtl.IsFunctional,
                    c.docdtl.ErrorDescription
                });

                //一個wrokbook內至少會有一個worksheet,並將資料Insert至這個位於A1這個位置上
                var ws = workbook.Worksheets.Add("sheet1", 1);

                //Title
                ws.Cell(1, 1).Value = "表單編號";
                ws.Cell(1, 2).Value = "申請日期";
                ws.Cell(1, 3).Value = "區域";
                ws.Cell(1, 4).Value = "週期";
                ws.Cell(1, 5).Value = "類別";
                ws.Cell(1, 6).Value = "項目";
                ws.Cell(1, 7).Value = "欄位";
                ws.Cell(1, 8).Value = "數值";
                ws.Cell(1, 9).Value = "單位";
                ws.Cell(1, 10).Value = "巡檢結果";
                ws.Cell(1, 11).Value = "備註說明";

                //如果是要塞入Query後的資料該資料一定要變成是data.AsEnumerable()
                ws.Cell(2, 1).InsertData(data);

                //因為是用Query的方式,這個地方要用串流的方式來存檔
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    //請注意 一定要加入這行,不然Excel會是空檔
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    //注意Excel的ContentType,是要用這個"application/vnd.ms-excel"
                    string fileName = "危險性設備數值搜尋_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                    return this.File(memoryStream.ToArray(), "application/vnd.ms-excel", fileName);
                }
            }
        }

        public List<DEInspectDocDetail> GetDocDetails(DEDocDetailQryVModel qry)
        {
            var qryStartDate = qry.StartDate;
            var qryEndDate = qry.EndDate;
            var qryAreaId = qry.AreaId;
            var qryCycleId = qry.CycleId;
            var qryClassId = qry.ClassId;
            var qryItemId = qry.ItemId;
            var qryFieldId = qry.FieldId;

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

            var searchList = db.DEInspectDoc.Join(db.DEInspectDocDetail, d => d.DocId, dtl => dtl.DocId,
                            (d, dtl) => new
                            {
                                doc = d,
                                docdtl = dtl
                            });

            /* 查詢日期 */
            if (string.IsNullOrEmpty(qryStartDate) == false || string.IsNullOrEmpty(qryEndDate) == false)
            {
                searchList = searchList.Where(v => v.doc.ApplyDate >= applyDateFrom && v.doc.ApplyDate <= applyDateTo);
            }
            if (!string.IsNullOrEmpty(qryAreaId))  /* 查詢區域 */
            {
                var areaid = Convert.ToInt32(qryAreaId);
                searchList = searchList.Where(r => r.doc.AreaId == areaid);
            }
            if (!string.IsNullOrEmpty(qryCycleId) && qryCycleId != "0")  /* 查詢週期 */
            {
                var cycleid = Convert.ToInt32(qryCycleId);
                searchList = searchList.Where(r => r.doc.CycleId == cycleid);
            }
            if (!string.IsNullOrEmpty(qryClassId) && qryClassId != "0")  /* 查詢類別 */
            {
                var classid = Convert.ToInt32(qryClassId);
                searchList = searchList.Where(r => r.doc.ClassId == classid);
            }
            if (!string.IsNullOrEmpty(qryItemId) && qryItemId != "0")  /* 查詢項目 */
            {
                var itemid = Convert.ToInt32(qryItemId);
                searchList = searchList.Where(r => r.docdtl.ItemId == itemid);
            }
            if (!string.IsNullOrEmpty(qryFieldId) && qryFieldId != "0")  /* 查詢欄位 */
            {
                var fieldid = Convert.ToInt32(qryFieldId);
                searchList = searchList.Where(r => r.docdtl.FieldId == fieldid);
            }

            List<DEInspectDocDetail> resultList = searchList.Select(s => s.docdtl).ToList();
            return resultList;
        }

    }
}