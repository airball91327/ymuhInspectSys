using InspectSystem.Models;
using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;
        // GET: Report
        public ActionResult Index(string reportType)
        {
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            if (reportType == "每日")
            {
                return View("DaliyReportIndex");
            }
            else
            {
                return View("MonthReportIndex");
            }
        }

        // GET: Report/Index2
        public ActionResult Index2(ReportQryVModel qry, int page = 1)
        {
            var qryDetails = GetMonthReport(qry);
            var docIds = qryDetails.Select(dtl => dtl.DocId).Distinct();
            var qryResult = docIds.Join(db.DEInspectDoc, id => id, d => d.DocId,
                                  (id, d) => new
                                  {
                                      doc = d
                                  })
                                  .Join(db.AppUsers, d => d.doc.EngId, u => u.Id,
                                  (d, u) => new
                                  {
                                      doc = d.doc,
                                      eng = u
                                  }).ToList();
            //
            List<DEInspectDocVModel> returnList = new List<DEInspectDocVModel>();
            DEInspectDocVModel docVModel;
            foreach (var item in qryResult.ToList())
            {
                docVModel = new DEInspectDocVModel();
                docVModel.ApplyDate = item.doc.ApplyDate;
                docVModel.AreaId = item.doc.AreaId;
                docVModel.AreaName = item.doc.AreaName;
                docVModel.CheckerId = item.doc.CheckerId;
                docVModel.CheckerName = item.doc.CheckerName;
                docVModel.ClassId = item.doc.ClassId;
                docVModel.ClassName = item.doc.ClassName;
                docVModel.CloseDate = item.doc.CloseDate;
                docVModel.CycleId = item.doc.CycleId;
                docVModel.CycleName = item.doc.CycleName;
                docVModel.DocId = item.doc.DocId;
                docVModel.EndTime = item.doc.EndTime;
                docVModel.EngId = item.doc.EngId;
                docVModel.EngName = item.doc.EngName;
                docVModel.EngUserName = item.eng.UserName;
                returnList.Add(docVModel);
            }

            var pageCount = returnList.ToPagedList(page, pageSize).PageCount;
            pageCount = pageCount == 0 ? 1 : pageCount; // If no page.
            if (returnList.ToPagedList(page, pageSize).Count <= 0)  //If the page has no items.
                return PartialView("DocList", returnList.ToPagedList(pageCount, pageSize));
            return PartialView("DocList", returnList.ToPagedList(page, pageSize));
        }

        public ActionResult DownloadDaliyReport(ReportQryVModel qry)
        {
            var resultDetails = GetDaliyReport(qry);
            var resultList = resultDetails.Join(db.DEInspectDoc, dtl => dtl.DocId, d => d.DocId,
                                          (dtl, d) => new
                                          {
                                              detail = dtl,
                                              doc = d
                                          }).OrderBy(d => d.doc.ApplyDate).ToList();
            //
            string fileName = "日報表_" + DateTime.Now.ToString("yyyyMMdd");
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".doc"); //fileName是word的檔名
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");     //編碼utf-8
            Response.ContentType = "application/vnd.ms-word";                         //讓瀏覽器知道是word檔
            Response.Charset = "";

            string myDoc = "";
            StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/ReportSamples/通用表單_日.xml"));//檔案放在App_Data裡面

            myDoc = sr.ReadToEnd();      //從頭讀到尾

            sr.Close();

            //下面開始抓資料
            if (resultList.Count() > 0)
            {
                var firstData = resultList.FirstOrDefault();
                var location = firstData.detail.AreaName;
                var dpt = firstData.detail.ClassName.Split('-').GetValue(0).ToString();
                var ClassName = firstData.detail.ClassName.Split('-').GetValue(1).ToString();
                var year = (firstData.doc.ApplyDate.Year - 1911).ToString();
                var month = firstData.doc.ApplyDate.Month.ToString();
                //將抓到的資料，替換掉我們剛剛設的文字
                myDoc = myDoc.Replace("Location", location);
                myDoc = myDoc.Replace("Department", dpt);
                myDoc = myDoc.Replace("Year", year);
                myDoc = myDoc.Replace("Month", month);
                myDoc = myDoc.Replace("Classname", ClassName);
                //替換欄位名稱
                var itemNames = resultList.Where(r => r.detail.DocId == firstData.doc.DocId)
                                          .Select(r => new
                                          {
                                              itemId = r.detail.ItemId,
                                              itemOrder = r.detail.ItemOrder,
                                              itemName = r.detail.ItemName
                                          })
                                          .OrderBy(r => r.itemOrder)
                                          .Select(r => r.itemName).ToList();
                for(int i = 0; i < 13; i++)
                {
                    // Replace 項目名稱
                    string targetS = "";
                    string replaceS = "";
                    if (i < 10)
                    {
                        targetS = "field_0" + i.ToString();
                    }
                    else
                    {
                        targetS = "field_" + i.ToString();
                    }
                    if (itemNames.Count() > i) 
                    {
                        replaceS = itemNames[i];
                    }                    
                    myDoc = myDoc.Replace(targetS, replaceS);
                    // replace欄位數值
                    string targetCol = "";
                    string replaceValue = "A";
                    for (int j = 1; j <= 31; j++)
                    {
                        if (j < 10)
                        {
                            targetCol = "col0" + j.ToString();
                        }
                        else
                        {
                            targetCol = "col" + j.ToString();
                        }
                        if (i < 10)
                        {
                            targetCol += "0" + i.ToString();
                        }
                        else
                        {
                            targetCol += i.ToString();
                        }
                        myDoc = myDoc.Replace(targetCol, replaceValue);
                    }
                }
                //填入資料
            }
            //寫出
            Response.Write(myDoc);
            Response.End();

            return View();
        }

        public ActionResult GetMonthReportWord()
        {
            string fileName = "日報表";
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".doc"); //fileName是word的檔名
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");     //編碼utf-8
            Response.ContentType = "application/vnd.ms-word";                         //讓瀏覽器知道是word檔
            Response.Charset = "";

            string myDoc = "";
            StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/ReportSamples/通用表單_日.xml"));//檔案放在App_Data裡面

            myDoc = sr.ReadToEnd();      //從頭讀到尾

            sr.Close();

            //下面開始抓資料

            //if (asset != null)
            //{
            //    //將抓到的資料，替換掉我們剛剛設的文字
            //    myDoc = myDoc.Replace("UserDpt", dptName);
            //    myDoc = myDoc.Replace("AccDpt", asset.purchase.AccDptUid);
            //    myDoc = myDoc.Replace("BmedNo", asset.bmedm.BmedNo);
            //    myDoc = myDoc.Replace("EYAssetNo", asset.budget.EYAssetNo);
            //    myDoc = myDoc.Replace("assetname", asset.bmedm.ACname);
            //    myDoc = myDoc.Replace("brand", asset.purchase.Brand + "/" + asset.purchase.Type);
            //    myDoc = myDoc.Replace("uprice", asset.purchase.UPrice.ToString());
            //}
            //寫出
            Response.Write(myDoc);
            Response.End();

            return View();
        }

        public List<DEInspectDocDetail> GetDaliyReport(ReportQryVModel qry)
        {
            int areaId = qry.AreaId;
            int cycleId = qry.CycleId;
            int classId = qry.ClassId;
            DateTime? applyMonth = qry.ApplyDateFrom;
            List<DEInspectDocDetail> queryResult = new List<DEInspectDocDetail>();
            
            // Query
            var docDetails = db.DEInspectDoc.Join(db.DEInspectDocDetail, d => d.DocId, dtl => dtl.DocId,
                                                  (d, dtl) => new 
                                                  { 
                                                      doc = d,
                                                      detail = dtl
                                                  }).AsQueryable();
            // query area, cycle, class
            docDetails = docDetails.Where(d => d.detail.AreaId == areaId && d.detail.CycleId == cycleId && d.detail.ClassId == classId);
            // query the month of details
            if (applyMonth != null)
            {
                docDetails = docDetails.Where(d => d.doc.ApplyDate.Month == applyMonth.Value.Month);
            }

            queryResult = docDetails.Select(d => d.detail).ToList();
            return queryResult;
        }

        public List<DEInspectDocDetail> GetMonthReport(ReportQryVModel qry)
        {
            int areaId = qry.AreaId;
            int cycleId = qry.CycleId;
            int classId = qry.ClassId;
            DateTime? applyDateFrom = qry.ApplyDateFrom;
            DateTime? applyDateTo = qry.ApplyDateTo;
            if (applyDateTo != null)
            {
                applyDateTo = applyDateTo.Value.AddDays(1).AddSeconds(-1);
            }
            List<DEInspectDocDetail> queryResult = new List<DEInspectDocDetail>();

            // Query
            var docDetails = db.DEInspectDoc.Join(db.DEInspectDocDetail, d => d.DocId, dtl => dtl.DocId,
                                                  (d, dtl) => new
                                                  {
                                                      doc = d,
                                                      detail = dtl
                                                  }).AsQueryable();
            // query area, cycle, class
            docDetails = docDetails.Where(d => d.detail.AreaId == areaId && d.detail.CycleId == cycleId && d.detail.ClassId == classId);
            // query the month of details
            if (applyDateFrom != null)
            {
                docDetails = docDetails.Where(d => d.doc.ApplyDate >= applyDateFrom);
            }
            if (applyDateTo != null)
            {
                docDetails = docDetails.Where(d => d.doc.ApplyDate <= applyDateTo);
            }

            queryResult = docDetails.Select(d => d.detail).ToList();
            return queryResult;
        }

    }
}