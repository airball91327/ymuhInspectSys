using InspectSystem.Models;
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
            int areaId = qry.AreaId;
            int cycleId = qry.CycleId;
            int classId = qry.ClassId;
            DateTime applyDateFrom = qry.ApplyDateFrom;
            DateTime applyDateTo = qry.ApplyDateTo.AddDays(1).AddSeconds(-1);
            var cycle = db.DEInspectCycle.Find(cycleId);
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

    }
}