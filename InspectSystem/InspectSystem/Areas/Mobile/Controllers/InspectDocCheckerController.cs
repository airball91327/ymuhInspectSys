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
            int UserID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            /* Find All not completed documents. */
            var CheckingDocs = db.InspectDocs.Where(i => i.CheckerID == UserID &&
                                                         i.FlowStatusID != 2);

            return View(CheckingDocs.ToList());
        }

        // GET: Mobile/InspectDocChecker/DocListForWorker
        public ActionResult DocListForWorker()
        {
            int UserID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            /* Find all not completed or not send documents. */
            var CheckingDocs = db.InspectDocs.Where(i => i.WorkerID == UserID &&
                                                         i.FlowStatusID != 2 &&
                                                         i.FlowStatusID != 1);

            return View(CheckingDocs.ToList());
        }

        // Select Classes for checker to view DocDetails
        // GET: Mobile/InspectDocChecker/DocDetailsChecker
        public ActionResult DocDetailsChecker(int docID)
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

        // GET: Mobile/InspectDocChecker/ClassContentOfArea
        public ActionResult ClassContentOfArea(int ACID, int docID)
        {
            ViewBag.ClassName = db.ClassesOfAreas.Find(ACID).InspectClasses.ClassName;
            ViewBag.DocID = docID;

            /* Find the data. */
            var classID = db.ClassesOfAreas.Find(ACID).ClassID;
            var inspectDocDetails = db.InspectDocDetails.Where(i => i.DocID == docID &&
                                                                    i.ClassID == classID);

            /* Get items and fields from DocDetails. */
            ViewBag.itemsByDocDetails = inspectDocDetails.GroupBy(i => i.ItemID)
                                                         .Select(g => g.FirstOrDefault())
                                                         .OrderBy(s => s.ItemOrder).ToList();
            ViewBag.fieldsByDocDetails = inspectDocDetails.ToList();

            InspectDocDetailsViewModels inspectDocDetailsViewModels = new InspectDocDetailsViewModels()
            {
                InspectDocDetails = inspectDocDetails.ToList(),
            };

            return View(inspectDocDetailsViewModels);
        }

        // GET: Mobile/InspectDocChecker/GetFlowList
        public ActionResult GetFlowList(int docID)
        {
            var flowList = db.InspectDocFlows.Where(i => i.DocID == docID).OrderBy(i => i.StepID);
            var findDoc = db.InspectDocs.Find(docID);

            foreach (var item in flowList)
            {
                if (item.StepOwnerID == item.WorkerID)
                {
                    item.StepOwnerName = findDoc.WorkerName;
                }
                else if (item.StepOwnerID == item.CheckerID)
                {
                    item.StepOwnerName = findDoc.CheckerName;
                }
            }

            return View(flowList.ToList());
        }

        // GET: Mobile/InspectDocChecker/FlowDocEditForChecker
        public ActionResult FlowDocEditForChecker(int docID)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocID == docID)
                                            .OrderByDescending(i => i.StepID).First();

            ViewBag.AreaID = flowDoc.InspectDocs.AreaID;

            /* Use userID to find the user details.*/
            flowDoc.EditorID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            flowDoc.Opinions = "";

            // Get real name.
            // 先取得該使用者的 FormsIdentity
            FormsIdentity id = (FormsIdentity)User.Identity;
            // 再取出使用者的 FormsAuthenticationTicket
            FormsAuthenticationTicket ticket = id.Ticket;
            // 將儲存在 FormsAuthenticationTicket 中的角色定義取出，並轉成字串陣列
            char[] charSpilt = new char[] { ',', '{', '}', '[', ']', '"', ':', '\\' };
            string[] roles = ticket.UserData.Split(charSpilt, StringSplitOptions.RemoveEmptyEntries);
            flowDoc.EditorName = roles.Last();

            return View("FlowDocEditForChecker", flowDoc);
        }

        // GET: Mobile/InspectDocChecker/FlowDocEditForWorker
        public ActionResult FlowDocEditForWorker(int docID)
        {
            /* Find FlowDoc and set step to next. */
            var flowDoc = db.InspectDocFlows.Where(i => i.DocID == docID)
                                            .OrderByDescending(i => i.StepID).First();

            ViewBag.AreaID = flowDoc.InspectDocs.AreaID;

            /* Use userID to find the user details.*/
            flowDoc.EditorID = System.Convert.ToInt32(WebSecurity.CurrentUserName);
            flowDoc.Opinions = "";

            // Get real name.
            // 先取得該使用者的 FormsIdentity
            FormsIdentity id = (FormsIdentity)User.Identity;
            // 再取出使用者的 FormsAuthenticationTicket
            FormsAuthenticationTicket ticket = id.Ticket;
            // 將儲存在 FormsAuthenticationTicket 中的角色定義取出，並轉成字串陣列
            char[] charSpilt = new char[] { ',', '{', '}', '[', ']', '"', ':', '\\' };
            string[] roles = ticket.UserData.Split(charSpilt, StringSplitOptions.RemoveEmptyEntries);
            flowDoc.EditorName = roles.Last();

            return View("FlowDocEditForWorker", flowDoc);
        }

        // POST: Mobile/InspectDocChecker/FlowDocEditForChecker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FlowDocEditForChecker(InspectDocFlow inspectDocFlow)
        {
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);
            int nextFlowStatusID = System.Convert.ToInt32(Request.Form["NextFlowStatusID"]);

            /* Insert edit time, and change flow status for inspect doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDoc.FlowStatusID = nextFlowStatusID;

            /* If doc is send back to worker. */
            if (nextFlowStatusID == 0)
            {
                /* New next flow for worker. */
                InspectDocFlow nextDocFlow = new InspectDocFlow()
                {
                    DocID = docID,
                    StepID = inspectDocFlow.StepID + 1,
                    StepOwnerID = inspectDocFlow.WorkerID,
                    WorkerID = inspectDocFlow.WorkerID,
                    CheckerID = inspectDocFlow.CheckerID,
                    Opinions = "",
                    FlowStatusID = nextFlowStatusID,
                    EditorID = 0,
                    EditorName = "",
                    EditTime = null,
                };

                if (ModelState.IsValid)
                {
                    db.InspectDocFlows.Add(nextDocFlow);
                    db.SaveChanges();

                    //Send Mail
                    Mail mail = new Mail();
                    string body = "";
                    mail.from = inspectDocFlow.CheckerID + "@cch.org.tw";
                    mail.to = inspectDocFlow.WorkerID + "@cch.org.tw";
                    mail.subject = "巡檢系統[退件通知]";
                    body += "<p>表單編號：" + inspectDoc.DocID + "</p>";
                    body += "<p>日期：" + inspectDoc.Date.ToString("yyyy/MM/dd") + "</p>";
                    body += "<p>區域：" + db.InspectAreas.Find(inspectDoc.AreaID).AreaName + "</p>";
                    body += "<p><a href='https://inspectsys.azurewebsites.net/'" + ">處理案件</a></p>";
                    body += "<br/>";
                    body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    body += "<br/>";
                    mail.msg = body;

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                    string url = "WebApi/Mail/SendMail";
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    var content = new StringContent(JsonConvert.SerializeObject(mail), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                }
            }
            else if (nextFlowStatusID == 2) // If doc closed.
            {
                /* New next flow for closed. */
                InspectDocFlow nextDocFlow = new InspectDocFlow()
                {
                    DocID = docID,
                    StepID = inspectDocFlow.StepID + 1,
                    StepOwnerID = inspectDocFlow.WorkerID,
                    WorkerID = inspectDocFlow.WorkerID,
                    CheckerID = inspectDocFlow.CheckerID,
                    Opinions = "",
                    FlowStatusID = nextFlowStatusID,
                    EditorID = inspectDocFlow.EditorID,
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
                if (inspectDocFlow.FlowStatusID == 2)
                {
                    TempData["SendMsg"] = "文件已結案";
                }
                else if (inspectDocFlow.FlowStatusID == 0)
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
            int userID = inspectDocFlow.EditorID;
            int docID = inspectDocFlow.DocID;
            var inspectDoc = db.InspectDocs.Find(docID);

            /* Insert edit time, and change flow status to "checking" for doc. */
            inspectDocFlow.EditTime = DateTime.UtcNow.AddHours(08);
            inspectDoc.FlowStatusID = 1;

            /* New next flow for checker. */
            InspectDocFlow nextDocFlow = new InspectDocFlow()
            {
                DocID = docID,
                StepID = inspectDocFlow.StepID + 1,
                StepOwnerID = inspectDocFlow.CheckerID,
                WorkerID = inspectDocFlow.WorkerID,
                CheckerID = inspectDocFlow.CheckerID,
                Opinions = "",
                FlowStatusID = 1,
                EditorID = 0,
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