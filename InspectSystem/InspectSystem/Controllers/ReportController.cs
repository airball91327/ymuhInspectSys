using InspectSystem.Models;
using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: Report
        public ActionResult Index()
        {
            ViewBag.AreaId = new SelectList(db.DEInspectArea, "AreaId", "AreaName");
            return View();
        }

        // GET: Report/Index2
        public ActionResult Index2(ReportQryVModel qry)
        {
            int cycleId = qry.CycleId;
            var cycle = db.DEInspectCycle.Find(cycleId);
            TempData["query"] = qry;
            //
            if (cycle.CycleName.Contains("每日"))
            {
                return RedirectToAction("GetDaliyReportWord");
            }
            else
            {
                return RedirectToAction("GetMonthReportWord");
            }
        }

        public ActionResult GetDaliyReportWord()
        {
            ReportQryVModel qry = TempData["query"] as ReportQryVModel;
            var resultDetails = GetDaliyReport(qry);
            var resultList = resultDetails.Join(db.DEInspectDoc, dtl => dtl.DocId, d => d.DocId,
                                          (dtl, d) => new
                                          {
                                              detail = dtl,
                                              doc = d
                                          }).OrderBy(d => d.doc.ApplyDate).ToList();
            //
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
                //
                myDoc = myDoc.Replace("field_1", firstData.detail.ItemName);
                myDoc = myDoc.Replace("field_2", firstData.detail.ItemName);
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
            DateTime applyDateFrom = qry.ApplyDateFrom;
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
            docDetails = docDetails.Where(d => d.doc.ApplyDate.Month == applyDateFrom.Month);

            queryResult = docDetails.Select(d => d.detail).ToList();
            return queryResult;
        }

    }
}