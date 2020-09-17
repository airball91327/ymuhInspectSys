using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using InspectSystem.Models;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace InspectSystem.Areas.Mobile.Controllers
{
    [Authorize]
    public class InspectDocCheckerController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Mobile/InspectDocChecker/DocListForChecker
        public ActionResult DocListForChecker()
        {
            int UserID = WebSecurity.CurrentUserId;
            /* Find All not completed documents. */
            var CheckingDocs = db.InspectDocs.Where(i => i.CheckerId == UserID &&
                                                         i.FlowStatusId != 2);

            return View(CheckingDocs.ToList());
        }

        // GET: Mobile/InspectDocChecker/DocListForWorker
        public ActionResult DocListForWorker()
        {
            int UserID = WebSecurity.CurrentUserId;
            /* Find all not completed or not send documents. */
            var CheckingDocs = db.InspectDocs.Where(i => i.EngId == UserID &&
                                                         i.FlowStatusId != 2 &&
                                                         i.FlowStatusId != 1);

            return View(CheckingDocs.ToList());
        }

        // Select Classes for checker to view DocDetails
        // GET: Mobile/InspectDocChecker/DocDetailsChecker
        public ActionResult DocDetailsChecker(int DocId)
        {
            /* Set variables from DB. */
            var DocDetailList = db.InspectDocDetails.Where(i => i.DocId == DocId).ToList();
            int length = DocId.ToString().Length;
            int areaID = System.Convert.ToInt32(DocId.ToString().Substring(length - 2));

            ViewBag.AreaId = DocDetailList.First().AreaId;
            ViewBag.AreaName = DocDetailList.First().AreaName;
            ViewBag.DocId = DocId;

            /* Find Classes from DocDetails and set values to List<ClassesOfAreas> ClassList. */
            var ClassesOfDocTemp = DocDetailList.GroupBy(c => c.ClassId).Select(g => g.FirstOrDefault()).ToList();
            List<ClassesOfAreas> ClassList = new List<ClassesOfAreas>();
            foreach (var itemClass in ClassesOfDocTemp)
            {
                var addClass = db.ClassesOfAreas.Where(c => c.AreaId == areaID && c.ClassId == itemClass.ClassId).FirstOrDefault();
                ClassList.Add(addClass);
            }
            var ClassesOfAreas = ClassList.OrderBy(c => c.InspectClasses.ClassOrder);

            /* Count errors for every class, and set count result to "CountErrors". */
            foreach (var item in ClassesOfAreas)
            {
                var toFindErrors = DocDetailList.Where(d => d.ClassId == item.ClassId &&
                                                           d.IsFunctional == "n");
                item.CountErrors = toFindErrors.Count();
            }
            return View(ClassesOfAreas.ToList());
        }

        // GET: Mobile/InspectDocChecker/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int DocId)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.DocId = DocId;

            /* Find the data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassId;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocId == DocId &&
                                                                    i.ClassId == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetails.GroupBy(i => i.ItemId)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetails.ToList();

            InspectDocDetailViewModels inspectDocDetailsViewModels = new InspectDocDetailViewModels()
            {
                InspectDocDetails = inspectDocDetails.ToList(),
            };

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocChecker/GetFlowList
        public ActionResult GetFlowList(int DocId)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocId == DocId).OrderBy(i => i.StepId);
            var findDoc = db.InspectDocs.Find(DocId);

            foreach (var item in flowList)
            {
                if (item.StepOwnerId == item.EngId)
                {
                    item.StepOwnerName = findDoc.EngName;
                }
                else if (item.StepOwnerId == item.CheckerId)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return View(flowList.ToList());
        }

        // GET: Mobile/InspectDocChecker/FlowDocEditForChecker
        public ActionResult FlowDocEditForChecker(int DocId)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocId == DocId)
                                            .OrderByDescending(i => i.StepId).First();

            ViewBag.AreaId = flowDoc.InspectDocs.AreaId;

            /* Use userID to find the user details.*/
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            flowDoc.EditorId = WebSecurity.CurrentUserId;
            flowDoc.Opinions = "";
            flowDoc.EditorName = u.FullName;

            return View("FlowDocEditForChecker", flowDoc);
        }

        // GET: Mobile/InspectDocChecker/FlowDocEditForWorker
        public ActionResult FlowDocEditForWorker(int DocId)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocId == DocId)
                                            .OrderByDescending(i => i.StepId).First();

            ViewBag.AreaId = flowDoc.InspectDocs.AreaId;

            /* Use userID to find the user details.*/
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            flowDoc.EditorId = WebSecurity.CurrentUserId;
            flowDoc.Opinions = "";
            flowDoc.EditorName = u.FullName;

            return View("FlowDocEditForWorker", flowDoc);
        }

        // POST: Mobile/InspectDocChecker/FlowDocEditForChecker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FlowDocEditForChecker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorId;
            int DocId = inspectDocFlow.DocId;
            var inspectDoc = db.InspectDocs.Find(DocId);
            int nextFlowStatusId = System.Convert.ToInt32(Request.Form["NextFlowStatusId"]);

            /* Insert edit time, and change flow status for inspect doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDoc.FlowStatusId = nextFlowStatusId;

            /* If doc is send back to worker. */
            if (nextFlowStatusId == 0)
            {
                /* New next flow for worker. */
                InspectDocFlow nextDocFlow = new InspectDocFlow()
                {
                    DocId = DocId,
                    StepId = inspectDocFlow.StepId + 1,
                    StepOwnerId = inspectDocFlow.EngId,
                    EngId = inspectDocFlow.EngId,
                    CheckerId = inspectDocFlow.CheckerId,
                    Opinions = "",
                    FlowStatusId = nextFlowStatusId,
                    EditorId = 0,
                    EditorName = "",
                    EditTime = null,
                };

                if (ModelState.IsValid)
                {
                    db.InspectDocFlows.Add(nextDocFlow);
                    db.SaveChanges();

                    //Send Mail
                    Mail mail = new Mail();
                    //string body = "";
                    //mail.from = inspectDocFlow.CheckerId + "@cch.org.tw";
                    //mail.to = inspectDocFlow.EngId + "@cch.org.tw";
                    //mail.subject = "巡檢系統[退件通知]";
                    //body += "<p>表單編號：" + inspectDoc.DocId + "</p>";
                    //body += "<p>日期：" + inspectDoc.Date.ToString("yyyy/MM/dd") + "</p>";
                    //body += "<p>區域：" + db.InspectAreas.Find(inspectDoc.AreaId).AreaName + "</p>";
                    //body += "<p><a href='https://inspectsys.azurewebsites.net/'" + ">處理案件</a></p>";
                    //body += "<br/>";
                    //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    //body += "<br/>";
                    //mail.msg = body;

                    //HttpClient client = new HttpClient();
                    //client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                    //string url = "WebApi/Mail/SendMail";
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(
                    //    new MediaTypeWithQualityHeaderValue("application/json"));
                    //var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                    //HttpResponseMessage response = await client.PostAsync(url, content);
                }
            }
            else if (nextFlowStatusId == 2) // If doc closed.
            {
                /* New next flow for closed. */
                InspectDocFlow nextDocFlow = new InspectDocFlow()
                {
                    DocId = DocId,
                    StepId = inspectDocFlow.StepId + 1,
                    StepOwnerId = inspectDocFlow.EngId,
                    EngId = inspectDocFlow.EngId,
                    CheckerId = inspectDocFlow.CheckerId,
                    Opinions = "",
                    FlowStatusId = nextFlowStatusId,
                    EditorId = inspectDocFlow.EditorId,
                    EditorName = inspectDocFlow.EditorName,
                    EditTime = inspectDocFlow.EditTime,
                };

                if (ModelState.IsValid)
                {
                    db.InspectDocFlows.Add(nextDocFlow);
                    db.SaveChanges();
                }
            }

            if (ModelState.IsValid)
            {
                /* Add new data to doc flow, and modefiy doc. */
                db.Entry(inspectDocFlow).State = EntityState.Modified;
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();

                /* return save success message. */
                if (inspectDocFlow.FlowStatusId == 2)
                {
                    TempData["SendMsg"] = "文件已結案";
                }
                else if (inspectDocFlow.FlowStatusId == 0)
                {
                    TempData["SendMsg"] = "文件已退回";
                }
                return RedirectToAction("DocListForChecker", new { area = "Mobile" });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForChecker", new { area = "Mobile" });
            }
        }

        // POST: Mobile/InspectDocChecker/FlowDocEditForWorker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FlowDocEditForWorker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorId;
            int DocId = inspectDocFlow.DocId;
            var inspectDoc = db.InspectDocs.Find(DocId);

            /* Insert edit time, and change flow status to "checking" for doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDoc.FlowStatusId = 1;

            /* New next flow for checker. */
            InspectDocFlow nextDocFlow = new InspectDocFlow()
            {
                DocId = DocId,
                StepId = inspectDocFlow.StepId + 1,
                StepOwnerId = inspectDocFlow.CheckerId,
                EngId = inspectDocFlow.EngId,
                CheckerId = inspectDocFlow.CheckerId,
                Opinions = "",
                FlowStatusId = 1,
                EditorId = 0,
                EditorName = "",
                EditTime = null,
            };

            if (ModelState.IsValid)
            {
                db.Entry(inspectDocFlow).State = EntityState.Modified;
                db.InspectDocFlows.Add(nextDocFlow);
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();

                /* return save success message. */
                TempData["SendMsg"] = "文件傳送成功";
                return RedirectToAction("DocListForWorker", new { area = "Mobile" });
            }
            else
            {
                TempData["SendMsg"] = "文件傳送失敗";
                return RedirectToAction("DocListForWorker", new { area = "Mobile" });
            }
        }
    }
}