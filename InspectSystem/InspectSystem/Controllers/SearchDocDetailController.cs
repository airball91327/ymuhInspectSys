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

namespace InspectSystem.Controllers
{
    public class SearchDocDetailController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: SearchDocDetail
        public ActionResult Index()
        {
            ViewBag.AreaID = new SelectList(db.InspectArea, "AreaId", "AreaName");
            return View();
        }

        // GET: SearchDocDetail/GetData          
        public JsonResult GetData(int? draw, int? start, int length,    //←此三個為DataTables自動傳遞參數
                                  DateTime startDate, DateTime endDate, int areaId, int shiftId,
                                  int classId, int itemId, int fieldId)
        {
            //查詢&排序後的總筆數
            int recordsTotal = 0;
            //jQuery DataTable的Column index
            string col_index = Request.QueryString["order[0][column]"];
            //排序資料行名稱
            string sortColName = string.IsNullOrEmpty(col_index) ? "Date" : Request.QueryString[$@"columns[{col_index}][data]"];
            //升冪或降冪
            string asc_desc = string.IsNullOrEmpty(Request.QueryString["order[0][dir]"]) ? "asc" : Request.QueryString["order[0][dir]"];//防呆

            //DateTime applyDateFrom = DateTime.Now;
            //DateTime applyDateTo = DateTime.Now;
            ///* Dealing search by date. */
            //if (startDate != null && endDate != null)// If 2 date inputs have been insert, compare 2 dates.
            //{
            //    DateTime date1 = DateTime.Parse(startDate);
            //    DateTime date2 = DateTime.Parse(endDate);
            //    int result = DateTime.Compare(date1, date2);
            //    if (result < 0)
            //    {
            //        applyDateFrom = date1.Date;
            //        applyDateTo = date2.Date;
            //    }
            //    else if (result == 0)
            //    {
            //        applyDateFrom = date1.Date;
            //        applyDateTo = date1.Date;
            //    }
            //    else
            //    {
            //        applyDateFrom = date2.Date;
            //        applyDateTo = date1.Date;
            //    }
            //}
            //else if (startDate == null && endDate != null)
            //{
            //    applyDateFrom = DateTime.Parse(endDate);
            //    applyDateTo = DateTime.Parse(endDate);
            //}
            //else if (startDate != null && endDate == null)
            //{
            //    applyDateFrom = DateTime.Parse(startDate);
            //    applyDateTo = DateTime.Parse(startDate);
            //}

            ///* 查詢日期 */
            var searchList = db.InspectDocDetail.ToList();

            ///* 查詢區域、類別 */
            //searchList = searchList.Where(s => s.AreaId == areaId &&
            //                                    s.ClassId == classId);
            ///* 查詢項目 */
            //if (itemId != 0)
            //{
            //    searchList = searchList.Where(s => s.ItemID == itemId);
            //}

            ///* 查詢欄位 */
            //if (fieldId != 0)
            //{
            //    searchList = searchList.Where(s => s.FieldID == fieldId);
            //}            

            /* 處理儲存正常或不正常的欄位，把Value拿來顯示是否正常. */
            foreach (var item in searchList)
            {
                if (item.DataType == "boolean")
                {
                    if (item.IsFunctional == "y")
                    {
                        item.Value = "正常";
                    }
                    else
                    {
                        item.Value = "不正常";
                    }
                }
            }

            var resultList = searchList.Select(s => new
            {
                ApplyDate = s.InspectDocs.ApplyDate.ToString("yyyy/MM/dd"),   // ToString() is not supported in Linq to Entities, 
                AreaName = s.AreaName,                                        // need to change type to IEnumerable by using AsEnumerable(),
                ClassName = s.ClassName,                                      // and then can use ToString(), 
                ItemName = s.ItemName,                                        // because AsEnumerable() is Linq to Objects.
                FieldName = s.FieldName,
                Value = s.Value,
                UnitOfData = s.UnitOfData,
                DocId = s.DocId,
                AreaId = s.AreaId
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

        // POST: SearchDocDetail/GetClasses
        [HttpPost]
        public JsonResult GetClasses(int AreaId, int shiftId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            db.InspectClass.Where(c => c.AreaId == AreaId && c.ShiftId == shiftId)
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

        // POST: SearchDocDetail/GetItems
        [HttpPost]
        public JsonResult GetItems(int AreaId,int shiftId, string ClassId)
        {
            int classId = System.Convert.ToInt32(ClassId);
            List<SelectListItem> list = new List<SelectListItem>();
            db.InspectItem.Where(i => i.AreaId == AreaId && i.ShiftId == shiftId && i.ClassId == classId)
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

        // POST: SearchDocDetail/GetFields
        [HttpPost]
        public JsonResult GetFields(int AreaId, int shiftId, string ClassId, string ItemId)
        {
            int classId = System.Convert.ToInt32(ClassId);
            int itemId = System.Convert.ToInt32(ItemId);
            List<SelectListItem> list = new List<SelectListItem>();
            db.InspectField.Where(i => i.AreaId == AreaId && i.ShiftId == shiftId && i.ClassId == classId && i.ItemId == itemId).ToList()
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

        // GET: SearchDocDetail/ExportToExcel
        public ActionResult ExportToExcel(DateTime startDate, DateTime endDate, int areaId, int classId, int itemId, int fieldId)
        {
            /* 查詢日期 */
            var searchList = db.InspectDocDetail.ToList();
            //var searchList = db.InspectDocDetail.Where(i => i.DocID >= fromDoc && i.DocID <= toDoc);

            ///* 查詢區域、類別 */
            //searchList = searchList.Where(s => s.AreaID == areaId &&
            //                                   s.ClassID == classId);
            ///* 查詢項目 */
            //if (itemId != 0)
            //{
            //    searchList = searchList.Where(s => s.ItemID == itemId);
            //}

            ///* 查詢欄位 */
            //if (fieldId != 0)
            //{
            //    searchList = searchList.Where(s => s.FieldID == fieldId);
            //}

            //ClosedXML的用法 先new一個Excel Workbook
            using (XLWorkbook workbook = new XLWorkbook())
            {
                //取得要塞入Excel內的資料
                var data = searchList.Select(c => new {
                    c.DocId,
                    c.AreaName,
                    c.ClassName,
                    c.ItemName,
                    c.FieldName,
                    c.UnitOfData,
                    c.Value,
                    c.IsFunctional,
                    c.ErrorDescription
                });

                //一個wrokbook內至少會有一個worksheet,並將資料Insert至這個位於A1這個位置上
                var ws = workbook.Worksheets.Add("sheet1", 1);

                //Title
                ws.Cell(1, 1).Value = "表單編號";
                ws.Cell(1, 2).Value = "區域名稱";
                ws.Cell(1, 3).Value = "類別名稱";
                ws.Cell(1, 4).Value = "項目名稱";
                ws.Cell(1, 5).Value = "欄位名稱";
                ws.Cell(1, 6).Value = "單位";
                ws.Cell(1, 7).Value = "數值";
                ws.Cell(1, 8).Value = "是否正常";
                ws.Cell(1, 9).Value = "備註說明";

                //如果是要塞入Query後的資料該資料一定要變成是data.AsEnumerable()
                ws.Cell(2, 1).InsertData(data);

                //因為是用Query的方式,這個地方要用串流的方式來存檔
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    //請注意 一定要加入這行,不然Excel會是空檔
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    //注意Excel的ContentType,是要用這個"application/vnd.ms-excel"
                    string fileName = "數值搜尋_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                    return this.File(memoryStream.ToArray(), "application/vnd.ms-excel", fileName);
                }
            }
        }
    }
}