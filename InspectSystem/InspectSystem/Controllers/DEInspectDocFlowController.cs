using InspectSystem.Filters;
using InspectSystem.Models;
using InspectSystem.Models.DEquipment;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class DEInspectDocFlowController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: DEInspectDocFlow
        public async Task<ActionResult> Index()
        {
            var inspectDocFlow = db.DEInspectDocFlow.Include(i => i.DEInspectFlowStatus);
            return View(await inspectDocFlow.ToListAsync());
        }

        // GET: DEInspectDocFlow/FlowList
        public ActionResult FlowList(string docId)
        {
            AppUser user;
            var inspectDocFlow = db.DEInspectDocFlow.Include(i => i.DEInspectFlowStatus)
                                                    .Where(df => df.DocId == docId).ToList();
            foreach (var item in inspectDocFlow)
            {
                user = db.AppUsers.Find(item.UserId);
                item.UserFullName = user == null ? "" : user.FullName + "(" + user.UserName + ")";
                user = db.AppUsers.Find(item.Rtp);
                item.RtpFullName = user == null ? "" : user.FullName + "(" + user.UserName + ")";
            }
            return PartialView(inspectDocFlow);
        }

        // GET: InspectDocFlow/NextFlow
        public ActionResult NextFlow(string docId)
        {
            DEAssign assign = new DEAssign();
            assign.DocId = docId;
            assign.CanClose = true;
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
            listItem.Add(new SelectListItem { Text = "巡檢工程師", Value = "巡檢工程師" });
            listItem.Add(new SelectListItem { Text = "工務組長", Value = "工務組長" });
            listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
            //
            DEInspectDocFlow flow = db.DEInspectDocFlow.Where(f => f.DocId == docId)
                                                       .Where(f => f.FlowStatusId == "?" || f.FlowStatusId == "0").FirstOrDefault();
            if (flow != null)
            {
                assign.ClsNow = flow.Cls;
                if (flow.Cls == "驗收人" || flow.Cls == "單位主管")
                {
                    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                }
                if (flow.Cls == "巡檢工程師")
                {
                    listItem.Clear();
                    //listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
                    listItem.Add(new SelectListItem { Text = "工務組長", Value = "工務組長" });
                    listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
                }
            }
            ViewData["FlowCls"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "", Value = "" });
            ViewData["FlowUid"] = new SelectList(listItem2, "Value", "Text", "");

            return PartialView(assign);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextFlow(DEAssign assign)
        {
            if (ModelState.IsValid)
            {
                if (assign.FlowCls == "結案" || assign.FlowCls == "廢除")
                {
                    assign.FlowUid = WebSecurity.CurrentUserId;
                }
                DEInspectDocFlow df = db.DEInspectDocFlow.Where(f => f.DocId == assign.DocId)
                                                         .Where(f => f.FlowStatusId == "?" || f.FlowStatusId == "0").FirstOrDefault();
                if (assign.FlowCls == "結案")
                {
                    var inspectDocs = db.DEInspectDoc.Find(assign.DocId);
                    // Update close date.
                    inspectDocs.CloseDate = DateTime.Now;
                    //
                    df.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    df.FlowStatusId = "2";
                    df.Rtt = DateTime.Now;
                    df.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(df).State = EntityState.Modified;
                    db.Entry(inspectDocs).State = EntityState.Modified;
                    db.SaveChanges();
                    //Send Mail
                    //Tmail mail = new Tmail();
                    //string body = "";
                    //string sto = "";
                    //AppUser u;
                    //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    //db.RepairFlows.Where(f => f.DocId == assign.DocId)
                    //    .ToList()
                    //    .ForEach(f =>
                    //    {
                    //        if (!f.Cls.Contains("工程師"))
                    //        {
                    //            u = db.AppUsers.Find(f.UserId);
                    //            sto += u.Email + ",";
                    //        }
                    //    });
                    //mail.sto = sto.TrimEnd(new char[] { ',' });
                    //mail.message.Subject = "醫療儀器管理資訊系統[請修案-結案通知]：儀器名稱： " + repair.AssetName;
                    //body += "<p>申請人：" + repair.UserName + "</p>";
                    //body += "<p>財產編號：" + repair.AssetNo + "</p>";
                    //body += "<p>儀器名稱：" + repair.AssetName + "</p>";
                    //body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                    //body += "<p>故障描述：" + repair.TroubleDes + "</p>";
                    //body += "<p>處理描述：" + rd.DealDes + "</p>";
                    //body += "<p><a href='https://mdms.ymuh.ym.edu.tw/'>檢視案件</a></p>";
                    //body += "<br/>";
                    //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    //mail.message.Body = body;
                    //mail.message.IsBodyHtml = true;
                    //mail.SendMail();
                }
                else if (assign.FlowCls == "廢除")
                {
                    //Do nothing.
                }
                else
                {
                    //轉送下一關卡
                    df.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    df.FlowStatusId = "1";
                    df.Rtt = DateTime.Now;
                    df.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(df).State = EntityState.Modified;
                    db.SaveChanges();
                    //
                    DEInspectDocFlow flow = new DEInspectDocFlow();
                    flow.DocId = assign.DocId;
                    flow.StepId = df.StepId + 1;
                    flow.UserId = assign.FlowUid.Value;
                    flow.FlowStatusId = "?";
                    flow.Cls = assign.FlowCls;
                    flow.Rtt = DateTime.Now;
                    db.DEInspectDocFlow.Add(flow);
                    db.SaveChanges();
                }
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                string msg = "";
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    msg += error.ErrorMessage + Environment.NewLine;
                }
                throw new Exception(msg);
            }
        }

        [MyErrorHandler]
        public JsonResult GetNextEmp(string cls, string docid)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            List<string> s;
            SelectListItem li;
            AppUser u;
            DEInspectDoc r = db.DEInspectDoc.Find(docid);
            string g = "";

            switch (cls)
            {
                case "單位主管": //設備主管
                    s = Roles.GetUsersInRole("MedMgr").ToList();
                    list = new List<SelectListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        if (u != null)
                        {
                            li = new SelectListItem();
                            li.Text = "(" + u.UserName + ")" + u.FullName;
                            li.Value = WebSecurity.GetUserId(l).ToString();
                            list.Add(li);
                        }
                    }
                    break;
                case "巡檢工程師":
                    list = new List<SelectListItem>();
                    var inspectDocFlow = db.DEInspectDocFlow.Where(f => f.DocId == docid && f.Cls.Contains("工程師")).ToList();
                    if (inspectDocFlow.Count() > 0)
                    {
                        var engId = inspectDocFlow.OrderBy(f => f.StepId).Last().UserId;
                        u = db.AppUsers.Find(engId);
                        if (u != null)
                        {
                            li = new SelectListItem();
                            li.Text = "(" + u.UserName + ")" + u.FullName;
                            li.Value = engId.ToString();
                            list.Add(li);
                        }
                    }
                    break;
                case "驗收人":
                    list = new List<SelectListItem>();
                    break;
                case "工務組長":
                    list = new List<SelectListItem>();
                    break;
                default:
                    list = new List<SelectListItem>();
                    break;
            }
            return Json(list);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}