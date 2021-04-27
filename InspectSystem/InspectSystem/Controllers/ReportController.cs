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
                var temp = resultList.Where(r => r.detail.ItemName.Contains("單位名稱")).FirstOrDefault();
                var dpt = temp != null ? temp.detail.Value : "";
                var ClassName = firstData.detail.ClassName.ToString();
                var year = (firstData.doc.ApplyDate.Year - 1911).ToString();
                var month = firstData.doc.ApplyDate.Month.ToString();
                var managerFlow = db.DEInspectDocFlow.Where(df => df.DocId == firstData.doc.DocId).OrderByDescending(d => d.StepId)
                                                     .Where(df => df.Cls.Contains("單位主管")).FirstOrDefault();
                //將抓到的資料，替換掉我們剛剛設的文字
                myDoc = myDoc.Replace("Location", location);
                myDoc = myDoc.Replace("Department", dpt);
                myDoc = myDoc.Replace("Year", year);
                myDoc = myDoc.Replace("Month", month);
                myDoc = myDoc.Replace("Classname", ClassName);
                if (managerFlow != null)
                {
                    var manager = db.AppUsers.Find(managerFlow.Rtp);
                    myDoc = myDoc.Replace("Manager", manager.FullName);
                }
                //替換欄位名稱
                var itemNames = resultList.Where(r => r.detail.DocId == firstData.doc.DocId)
                                          .Select(r => new
                                          {
                                              itemId = r.detail.ItemId,
                                              itemOrder = r.detail.ItemOrder,
                                              itemName = r.detail.ItemName
                                          })
                                          .OrderBy(r => r.itemOrder)
                                          .Where(r => r.itemName != "異常狀況之危害因素及嚴重性分析" && r.itemName != "異常狀況改善措施" &&
                                                      r.itemName != "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)" &&
                                                      r.itemName != "單位名稱")
                                          .Select(r => r.itemName).Distinct().ToList();
                int cloumns = 13;
                for (int i = 0; i < cloumns; i++)
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
                    //填入資料 欄位數值
                    string targetCol = "";
                    for (int j = 1; j <= 31; j++)
                    {
                        string replaceValue = "";
                        int qryDay = j;
                        var detailsOfDay = resultList.Where(r => r.doc.ApplyDate.Day == qryDay).Select(r => r.detail).ToList();
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
                        if (detailsOfDay.Count() > i)
                        {
                            if (detailsOfDay[i].ItemName != "異常狀況之危害因素及嚴重性分析" && detailsOfDay[i].ItemName != "異常狀況改善措施" &&
                                detailsOfDay[i].ItemName != "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)" &&
                                detailsOfDay[i].ItemName != "單位名稱") 
                            {
                                var value = detailsOfDay[i].IsFunctional;
                                if (value == "合格")
                                    replaceValue = "✔";
                                else if (value == "不合格")
                                    replaceValue = "✗";
                                else if (value == "已改善")
                                    replaceValue = "❍";
                                else if (value == "未開機")
                                    replaceValue = "△";
                                else
                                    replaceValue = "NA";
                            }
                        }
                        myDoc = myDoc.Replace(targetCol, replaceValue);
                    }
                }
                //填入檢查者
                for (int i = 1; i <= 31; i++)
                {
                    int qryDay = i;
                    var docOfDay = resultList.Where(r => r.doc.ApplyDate.Day == qryDay).Select(r => r.doc).ToList();
                    string targetCol = "";
                    string replaceValue = "";
                    if (i < 10)
                    {
                        targetCol = "ENG0" + i.ToString();
                    }
                    else
                    {
                        targetCol = "ENG" + i.ToString();
                    }
                    if (docOfDay.Count() > 0)
                    {
                        replaceValue = docOfDay.FirstOrDefault().EngName;
                    }
                    myDoc = myDoc.Replace(targetCol, replaceValue);
                }
                //填入異常狀況之危害因素及嚴重性分析等文字
                string analysisText = "", improveText = "", trackText = "";
                var temp1 = resultList.Where(r => r.detail.ItemName == "異常狀況之危害因素及嚴重性分析").ToList();
                var temp2 = resultList.Where(r => r.detail.ItemName == "異常狀況改善措施").ToList();
                var temp3 = resultList.Where(r => r.detail.ItemName == "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)").ToList();
                foreach(var item in temp1)
                {
                    if (item.detail.Value.Trim() != "無" && !string.IsNullOrEmpty(item.detail.Value))
                    {
                        analysisText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                    }
                }
                foreach (var item in temp2)
                {
                    if (item.detail.Value.Trim() != "無" && !string.IsNullOrEmpty(item.detail.Value))
                    {
                        improveText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                    }
                }
                foreach (var item in temp3)
                {
                    if (!string.IsNullOrEmpty(item.detail.Value))
                    {
                        if (item.detail.Value.Trim() != "無")
                        {
                            trackText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                        }
                    }
                }
                myDoc = myDoc.Replace("analysisText", analysisText);
                myDoc = myDoc.Replace("improveText", improveText);
                myDoc = myDoc.Replace("trackText", trackText);
            }
            else
            {
                //無資料時，所有欄位清空
                myDoc = myDoc.Replace("Location", "");
                myDoc = myDoc.Replace("Department", "");
                myDoc = myDoc.Replace("Year", "");
                myDoc = myDoc.Replace("Month", "");
                myDoc = myDoc.Replace("Classname", "");
                myDoc = myDoc.Replace("analysisText", "");
                myDoc = myDoc.Replace("improveText", "");
                myDoc = myDoc.Replace("trackText", "");
                for (int i = 0; i < 13; i++)
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
                    myDoc = myDoc.Replace(targetS, replaceS);
                    //填入資料 欄位數值
                    string targetCol = "";
                    for (int j = 1; j <= 31; j++)
                    {
                        string replaceValue = "";
                        int qryDay = j;
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
                //檢查者
                for (int i = 1; i <= 31; i++)
                {
                    string targetCol = "";
                    string replaceValue = "";
                    if (i < 10)
                    {
                        targetCol = "ENG0" + i.ToString();
                    }
                    else
                    {
                        targetCol = "ENG" + i.ToString();
                    }
                    myDoc = myDoc.Replace(targetCol, replaceValue);
                }
            }
            //寫出
            Response.Write(myDoc);
            Response.End();

            return View();
        }

        public ActionResult DownloadMonthReport(string docId)
        {
            var resultDetails = db.DEInspectDocDetail.Where(dtl => dtl.DocId == docId);
            var resultList = resultDetails.Join(db.DEInspectDoc, dtl => dtl.DocId, d => d.DocId,
                                          (dtl, d) => new
                                          {
                                              detail = dtl,
                                              doc = d
                                          }).OrderBy(d => d.doc.ApplyDate).ToList();
            var docFlows = db.DEInspectDocFlow.Join(db.AppUsers, df => df.Rtp, u => u.Id,
                                          (df, u) => new
                                          {
                                              flow = df,
                                              rtpuser = u
                                          }).Where(df => df.flow.DocId == docId).ToList();
            //
            string fileName = "月報表_" + DateTime.Now.ToString("yyyyMMdd");
            if (resultList.Count() > 0)
            {
                var cycleName = resultList.FirstOrDefault().doc.CycleName;
                fileName = cycleName + "報表_" + DateTime.Now.ToString("yyyyMMdd");
            }
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".doc"); //fileName是word的檔名
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");     //編碼utf-8
            Response.ContentType = "application/vnd.ms-word";                         //讓瀏覽器知道是word檔
            Response.Charset = "";

            string myDoc = "";
            StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/ReportSamples/通用表單_月.xml"));//檔案放在App_Data裡面

            myDoc = sr.ReadToEnd();      //從頭讀到尾

            sr.Close();

            //下面開始抓資料
            if (resultList.Count() > 0)
            {
                var firstData = resultList.FirstOrDefault();
                var location = firstData.detail.AreaName;
                var cycleName = firstData.doc.CycleName;
                var temp = resultList.Where(r => r.detail.ItemName.Contains("單位名稱")).FirstOrDefault();
                var dpt = temp != null ? temp.detail.Value : "";
                var ClassName = firstData.detail.ClassName.ToString();
                var year = (firstData.doc.ApplyDate.Year - 1911).ToString();
                var month = firstData.doc.ApplyDate.Month.ToString();
                var day = firstData.doc.ApplyDate.Day.ToString();
                var engineerName = firstData.doc.EngName;
                var managerName = "";
                if (docFlows.Count() > 0)
                {
                    docFlows = docFlows.OrderByDescending(df => df.flow.StepId).ToList();
                    var lastFlow = docFlows.Where(df => df.flow.Cls.Contains("單位主管")).FirstOrDefault();
                    managerName = lastFlow != null ? lastFlow.rtpuser.FullName : "";
                }
                //將抓到的資料，替換掉我們剛剛設的文字
                myDoc = myDoc.Replace("Location", location);
                myDoc = myDoc.Replace("Cycle", cycleName);
                myDoc = myDoc.Replace("Department", dpt);
                myDoc = myDoc.Replace("Year", year);
                myDoc = myDoc.Replace("Month", month);
                myDoc = myDoc.Replace("Classname", ClassName);
                myDoc = myDoc.Replace("Day", day);
                myDoc = myDoc.Replace("Engineer", engineerName);
                myDoc = myDoc.Replace("Manager", managerName);
                //替換欄位名稱
                var itemNames = resultList.Where(r => r.detail.DocId == firstData.doc.DocId)
                                          .Select(r => new
                                          {
                                              itemId = r.detail.ItemId,
                                              itemOrder = r.detail.ItemOrder,
                                              itemName = r.detail.ItemName
                                          })
                                          .Where(r => r.itemName != "異常狀況之危害因素及嚴重性分析" && r.itemName != "異常狀況改善措施" &&
                                                      r.itemName != "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)" &&
                                                      r.itemName != "單位名稱")
                                          .OrderBy(r => r.itemOrder)
                                          .Select(r => r.itemName).Distinct().ToList();
                int cloumns = 14;
                for (int i = 0; i < cloumns; i++)
                {
                    // Replace 項目名稱
                    string targetS = "", targetS2 = "";
                    string replaceS = "", replaceS2 = "";
                    string targetCol1 = "", targetCol2 = "", targetCol3 = "", targetCol4 = "";
                    string replaceVal1 = "", replaceVal2 = "", replaceVal3 = "", replaceVal4 = "";
                    if (i < 10)
                    {
                        targetS = "field_0" + i.ToString();
                        targetS2 = "no_0" + i.ToString();
                        targetCol1 = "check1_0" + i.ToString();
                        targetCol2 = "check2_0" + i.ToString();
                        targetCol3 = "value1_0" + i.ToString();
                        targetCol4 = "value2_0" + i.ToString();
                    }
                    else
                    {
                        targetS = "field_" + i.ToString();
                        targetS2 = "no_" + i.ToString();
                        targetCol1 = "check1_" + i.ToString();
                        targetCol2 = "check2_" + i.ToString();
                        targetCol3 = "value1_" + i.ToString();
                        targetCol4 = "value2_" + i.ToString();
                    }
                    if (itemNames.Count() > i)
                    {
                        if (itemNames[i] != "異常狀況之危害因素及嚴重性分析" && itemNames[i] != "異常狀況改善措施" &&
                            itemNames[i] != "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)" &&
                            itemNames[i] != "單位名稱") 
                        {
                            replaceS = itemNames[i];
                            replaceS2 = (i + 1).ToString();
                            var targetDetails = resultList.Where(r => r.detail.ItemName == itemNames[i]).ToList();
                            var val1 = targetDetails.Where(r => r.detail.FieldName == "檢查方法").FirstOrDefault();
                            var val2 = targetDetails.Where(r => r.detail.FieldName == "判定基準").FirstOrDefault();
                            var val3 = targetDetails.Where(r => r.detail.FieldName == "檢查結果或量測值").FirstOrDefault();
                            var val4 = targetDetails.Where(r => r.detail.FieldName == "是否合格").FirstOrDefault();
                            replaceVal1 = val1 != null ? val1.detail.FieldDescription : "";
                            replaceVal2 = val2 != null ? val2.detail.FieldDescription : "";
                            replaceVal3 = val3 != null ? val3.detail.Value : "";
                            replaceVal4 = val4 != null ? val4.detail.Value : "";
                        }
                    }
                    myDoc = myDoc.Replace(targetS, replaceS);
                    myDoc = myDoc.Replace(targetS2, replaceS2);
                    myDoc = myDoc.Replace(targetCol1, replaceVal1);
                    myDoc = myDoc.Replace(targetCol2, replaceVal2);
                    myDoc = myDoc.Replace(targetCol3, replaceVal3);
                    myDoc = myDoc.Replace(targetCol4, replaceVal4);
                    //填入異常狀況之危害因素及嚴重性分析等文字
                    string analysisText = "", improveText = "", trackText = "";
                    var temp1 = resultList.Where(r => r.detail.ItemName == "異常狀況之危害因素及嚴重性分析").ToList();
                    var temp2 = resultList.Where(r => r.detail.ItemName == "異常狀況改善措施").ToList();
                    var temp3 = resultList.Where(r => r.detail.ItemName == "異常狀況改善措施追蹤:(定期檢討改善措施之合宜性，本表並依規定須保存三年)").ToList();
                    foreach (var item in temp1)
                    {
                        if (item.detail.Value.Trim() != "無" && !string.IsNullOrEmpty(item.detail.Value))
                        {
                            analysisText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                        }
                    }
                    foreach (var item in temp2)
                    {
                        if (item.detail.Value.Trim() != "無" && !string.IsNullOrEmpty(item.detail.Value))
                        {
                            improveText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                        }
                    }
                    foreach (var item in temp3)
                    {
                        if (item.detail.Value.Trim() != "無" && !string.IsNullOrEmpty(item.detail.Value))
                        {
                            trackText += item.doc.ApplyDate.ToString("MM/dd") + item.detail.Value.ToString() + " ";
                        }
                    }
                    myDoc = myDoc.Replace("analysisText", analysisText);
                    myDoc = myDoc.Replace("improveText", improveText);
                    myDoc = myDoc.Replace("trackText", trackText);
                }
            }
            else
            {
                //無資料時，所有欄位清空
                myDoc = myDoc.Replace("Location", "");
                myDoc = myDoc.Replace("Cycle", "");
                myDoc = myDoc.Replace("Department", "");
                myDoc = myDoc.Replace("Year", "");
                myDoc = myDoc.Replace("Month", "");
                myDoc = myDoc.Replace("Classname", "");
                myDoc = myDoc.Replace("Day", "");
                myDoc = myDoc.Replace("engineer", "");
                myDoc = myDoc.Replace("analysisText", "");
                myDoc = myDoc.Replace("improveText", "");
                myDoc = myDoc.Replace("trackText", "");
                for (int i = 0; i < 13; i++)
                {
                    string targetS = "", targetS2 = "";
                    string targetCol1 = "", targetCol2 = "", targetCol3 = "", targetCol4 = "";
                    if (i < 10)
                    {
                        targetS = "field_0" + i.ToString();
                        targetS2 = "no_0" + i.ToString();
                        targetCol1 = "check1_0" + i.ToString();
                        targetCol2 = "check2_0" + i.ToString();
                        targetCol3 = "value1_0" + i.ToString();
                        targetCol4 = "value2_0" + i.ToString();
                    }
                    else
                    {
                        targetS = "field_" + i.ToString();
                        targetS2 = "no_" + i.ToString();
                        targetCol1 = "check1_" + i.ToString();
                        targetCol2 = "check2_" + i.ToString();
                        targetCol3 = "value1_" + i.ToString();
                        targetCol4 = "value2_" + i.ToString();
                    }
                    myDoc = myDoc.Replace(targetS, "");
                    myDoc = myDoc.Replace(targetS2, "");
                    myDoc = myDoc.Replace(targetCol1, "");
                    myDoc = myDoc.Replace(targetCol2, "");
                    myDoc = myDoc.Replace(targetCol3, "");
                    myDoc = myDoc.Replace(targetCol4, "");
                }
            }
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