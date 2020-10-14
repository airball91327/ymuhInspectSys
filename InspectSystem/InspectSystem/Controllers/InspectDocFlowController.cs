using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;
using InspectSystem.Filters;
using WebMatrix.WebData;
using System.Web.Security;

namespace InspectSystem.Controllers
{
    public class InspectDocFlowController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDocFlow
        public async Task<ActionResult> Index()
        {
            var inspectDocFlow = db.InspectDocFlow.Include(i => i.InspectDocIdTable).Include(i => i.InspectFlowStatus);
            return View(await inspectDocFlow.ToListAsync());
        }

        // GET: InspectDocFlow/FlowList
        public ActionResult FlowList(string docId)
        {
            AppUser user;
            var inspectDocFlow = db.InspectDocFlow.Include(i => i.InspectDocIdTable).Include(i => i.InspectFlowStatus).ToList();
            inspectDocFlow = inspectDocFlow.Where(df => df.DocId == docId).ToList();
            foreach(var item in inspectDocFlow)
            {
                user = db.AppUsers.Find(item.UserId);
                item.UserFullName = user == null ? "" : user.FullName + "(" + user.UserName + ")";
                user = db.AppUsers.Find(item.Rtp);
                item.RtpFullName = user == null ? "" : user.FullName + "(" + user.UserName + ")";
            }
            return PartialView(inspectDocFlow);
        }

        // GET: InspectDocFlow/NextShift
        public ActionResult NextShift(string docId, string shiftId)
        {
            Assign assign = new Assign();
            assign.DocId = docId;
            assign.ShiftId = shiftId;
            InspectDocFlow df = db.InspectDocFlow.Where(f => f.DocId == docId && f.FlowStatusId == "?").ToList().FirstOrDefault();
            if (df != null)
            {
                assign.ClsNow = df.Cls;
            }
            var dt = db.InspectDoc.Find(docId, Convert.ToInt32(shiftId));
            if (dt != null)
            {
                assign.ShiftOpn = dt.Note;
            }
            return PartialView(assign);
        }

        // POST: InspectDocFlow/NextShift
        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextShift(Assign assign)
        {
            int shiftId = Convert.ToInt32(assign.ShiftId);
            var docId = assign.DocId;
            var docIdTable = db.InspectDocIdTable.Find(docId);
            var inspectDoc = db.InspectDoc.Find(docId, shiftId);
            var docDetailTemp = db.InspectDocDetailTemp.Where(d => d.DocId == docId && d.ShiftId == shiftId).ToList();
            var docDetail = db.InspectDocDetail.Where(d => d.DocId == docId && d.ShiftId == shiftId).ToList();
            InspectDocFlow df = db.InspectDocFlow.Where(f => f.DocId == docId && f.FlowStatusId == "?").ToList().FirstOrDefault();

            // If has docDetails, remove all docDetails and reinsert temp data.
            List<InspectDocDetail> inspectDocDetails = new List<InspectDocDetail>();
            if (docDetail.Count() > 0) 
            {
                db.InspectDocDetail.RemoveRange(docDetail);
                db.SaveChanges();
            }
            foreach(var item in docDetailTemp)
            {
                inspectDocDetails.Add(new InspectDocDetail()
                {
                    DocId = item.DocId,
                    AreaId = item.AreaId,
                    AreaName = item.AreaName,
                    ShiftId = item.ShiftId,
                    ShiftName = item.ShiftName,
                    ClassId = item.ClassId,
                    ClassName = item.ClassName,
                    ClassOrder = item.ClassOrder,
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ItemOrder = item.ItemOrder,
                    FieldId = item.FieldId,
                    FieldName = item.FieldName,
                    DataType = item.DataType,
                    UnitOfData = item.UnitOfData,
                    MinValue = item.MinValue,
                    MaxValue = item.MaxValue,
                    IsRequired = item.IsRequired,
                    Value = item.Value,
                    IsFunctional = item.IsFunctional,
                    ErrorDescription = item.ErrorDescription,
                    RepairDocId = item.RepairDocId,
                    DropDownItems = item.DropDownItems
                });
            }
            try
            {
                db.InspectDocDetail.AddRange(inspectDocDetails);
                db.SaveChanges();

                // Edit docIdTable's doc status.
                docIdTable.DocStatusId = "2";
                db.Entry(docIdTable).State = EntityState.Modified;

                // Edit InspectDoc
                inspectDoc.EndTime = DateTime.Now;
                inspectDoc.Note = assign.ShiftOpn;
                db.Entry(inspectDoc).State = EntityState.Modified;
                db.SaveChanges();
                //
                df.FlowStatusId = "1";
                df.Rtp = WebSecurity.CurrentUserId;
                df.Rtt = DateTime.Now;
                db.Entry(df).State = EntityState.Modified;
                db.SaveChanges();

                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                return new JsonResult
                {
                    Data = new { success = false, error = e.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // GET: InspectDocFlow/NextShiftConfirm
        public ActionResult NextShiftConfirm(string docId, string shiftId)
        {
            Assign assign = new Assign();
            assign.DocId = docId;
            assign.ShiftId = shiftId;
            var dt = db.InspectDoc.Find(docId, Convert.ToInt32(shiftId));
            if (dt != null)
            {
                assign.ShiftOpn = dt.Note;
            }
            // Get next shift id
            var docIdTable = db.InspectDocIdTable.Find(docId);
            int sid = Convert.ToInt32(shiftId);
            int nextShiftId = 0;
            var shiftsInAreas = db.ShiftsInAreas.Where(s => s.AreaId == docIdTable.AreaId).OrderBy(s => s.ShiftId).ToList();
            var nextShift = shiftsInAreas.SkipWhile(s => s.ShiftId != sid).Skip(1).DefaultIfEmpty(null).FirstOrDefault();
            if (nextShift != null)
            {
                nextShiftId = nextShift.ShiftId;
            }
            assign.NextShiftId = nextShiftId;
            return PartialView(assign);
        }

        // POST: InspectDocFlow/ShiftConfirm
        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextShiftConfirm(Assign assign)
        {
            var user = db.AppUsers.Find(WebSecurity.CurrentUserId);
            int shiftId = Convert.ToInt32(assign.ShiftId);
            int nextShiftId = assign.NextShiftId;
            var docId = assign.DocId;
            var docIdTable = db.InspectDocIdTable.Find(docId);
            var inspectDoc = db.InspectDoc.Find(docId, shiftId);
            var docDetailTemp = db.InspectDocDetailTemp.Where(d => d.DocId == docId && d.ShiftId == shiftId).ToList();
            var docDetail = db.InspectDocDetail.Where(d => d.DocId == docId && d.ShiftId == shiftId).ToList();
            InspectDocFlow df = db.InspectDocFlow.Where(f => f.DocId == docId).OrderBy(f => f.StepId).ToList().Last();

            if (assign.AssignCls == "退回")
            {
                // Edit docIdTable's doc status.
                docIdTable.DocStatusId = "1";
                db.Entry(docIdTable).State = EntityState.Modified;
                // Edit InspectDoc
                if (user != null)
                {
                    inspectDoc.CheckerId = user.Id;
                    inspectDoc.CheckerName = user.FullName;
                    db.Entry(inspectDoc).State = EntityState.Modified;
                }
                // Back flow to last flow user.
                df.FlowStatusId = "?";
                df.Rtp = null;
                df.Rtt = null;
                db.Entry(docIdTable).State = EntityState.Modified;
                db.SaveChanges();
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                // Edit InspectDoc
                if (user != null)
                {
                    inspectDoc.CheckerId = user.Id;
                    inspectDoc.CheckerName = user.FullName;
                    db.Entry(inspectDoc).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //
            try
            {
                if (nextShiftId == 0)   // Now is the last shift.
                {
                    // Edit docIdTable's doc status.
                    docIdTable.DocStatusId = "3";
                    db.Entry(docIdTable).State = EntityState.Modified;
                    // Create next flow.
                    InspectDocFlow flow = new InspectDocFlow();
                    flow.DocId = df.DocId;
                    flow.StepId = df.StepId + 1;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "?";
                    flow.Rtp = WebSecurity.CurrentUserId;
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "巡檢工程師";
                    db.InspectDocFlow.Add(flow);
                    db.SaveChanges();
                    //
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    var shift = db.InspectShift.Find(nextShiftId);
                    // Edit docIdTable's doc status.
                    docIdTable.DocStatusId = "1";
                    docIdTable.ShiftId = nextShiftId;
                    db.Entry(docIdTable).State = EntityState.Modified;
                    // Create next flow.
                    InspectDocFlow flow = new InspectDocFlow();
                    flow.DocId = df.DocId;
                    flow.StepId = df.StepId + 1;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "?";
                    flow.Opinions = "【" + shift.ShiftName + "】";
                    flow.Rtp = WebSecurity.CurrentUserId;
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "巡檢工程師";
                    db.InspectDocFlow.Add(flow);
                    db.SaveChanges();

                    // 產生下一張巡檢單
                    InspectDoc nextDoc = new InspectDoc();
                    nextDoc.DocId = docIdTable.DocId;
                    nextDoc.ShiftId = docIdTable.ShiftId;
                    nextDoc.ApplyDate = docIdTable.ApplyDate;
                    nextDoc.EngId = user.Id;
                    nextDoc.EngName = user.FullName;
                    db.InspectDoc.Add(nextDoc);
                    // 擷取巡檢單內容
                    ShiftsInAreas shiftsInAreas = db.ShiftsInAreas.Include(i => i.InspectArea).Include(i => i.InspectShift)
                                                                  .Where(i => i.AreaId == docIdTable.AreaId && i.ShiftId == docIdTable.ShiftId).FirstOrDefault();
                    List<InspectClass> inspectClasses = db.InspectClass.Where(i => i.AreaId == docIdTable.AreaId && i.ShiftId == docIdTable.ShiftId).ToList();
                    List<InspectItem> inspectItems = db.InspectItem.Where(i => i.AreaId == docIdTable.AreaId && i.ShiftId == docIdTable.ShiftId).ToList();
                    List<InspectField> inspectFields = db.InspectField.Include(i => i.InspectItem).Include(i => i.InspectItem.InspectClass)
                                                                      .Where(i => i.AreaId == docIdTable.AreaId && i.ShiftId == docIdTable.ShiftId)
                                                                      .Where(i => i.InspectItem.InspectClass.ClassStatus == true)
                                                                      .Where(i => i.InspectItem.ItemStatus == true)
                                                                      .Where(i => i.FieldStatus == true).ToList();
                    // Insert values.
                    InspectDocDetailTemp detailTemp;
                    foreach (var field in inspectFields)
                    {
                        detailTemp = new InspectDocDetailTemp();
                        detailTemp.DocId = docId;
                        detailTemp.AreaId = field.AreaId;
                        detailTemp.AreaName = shiftsInAreas.InspectArea.AreaName;
                        detailTemp.ShiftId = field.ShiftId;
                        detailTemp.ShiftName = shiftsInAreas.InspectShift.ShiftName;
                        detailTemp.ClassId = field.ClassId;
                        detailTemp.ClassName = field.InspectItem.InspectClass.ClassName;
                        detailTemp.ClassOrder = field.InspectItem.InspectClass.ClassOrder;
                        detailTemp.ItemId = field.ItemId;
                        detailTemp.ItemName = field.InspectItem.ItemName;
                        detailTemp.ItemOrder = field.InspectItem.ItemOrder;
                        detailTemp.FieldId = field.FieldId;
                        detailTemp.FieldName = field.FieldName;
                        detailTemp.DataType = field.DataType;
                        detailTemp.UnitOfData = field.UnitOfData;
                        detailTemp.MinValue = field.MinValue;
                        detailTemp.MaxValue = field.MaxValue;
                        detailTemp.IsRequired = field.IsRequired;
                        /* If field is dropdown, set dropdownlist items to string and save to DB. */
                        if (field.DataType == "dropdownlist")
                        {
                            var itemDropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == field.AreaId &&
                                                                                      i.ShiftId == field.ShiftId &&
                                                                                      i.ClassId == field.ClassId &&
                                                                                      i.ItemId == field.ItemId &&
                                                                                      i.FieldId == field.FieldId).ToList();
                            foreach (var dropItem in itemDropDownList)
                            {
                                detailTemp.DropDownItems += dropItem.Value.ToString() + ";";
                            }
                        }
                        //
                        db.InspectDocDetailTemp.Add(detailTemp);
                    }
                    db.SaveChanges();
                    //
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception e)
            {
                return new JsonResult
                {
                    Data = new { success = false, error = e.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // GET: InspectDocFlow/NextFlow
        public ActionResult NextFlow(string docId)
        {
            Assign assign = new Assign();
            assign.DocId = docId;
            assign.CanClose = true;
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
            listItem.Add(new SelectListItem { Text = "巡檢工程師", Value = "巡檢工程師" });
            listItem.Add(new SelectListItem { Text = "組長", Value = "組長" });
            listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
            //
            InspectDocFlow flow = db.InspectDocFlow.Where(f => f.DocId == docId && f.FlowStatusId == "?").ToList().FirstOrDefault();
            if (flow != null)
            {
                assign.ClsNow = flow.Cls;
                if (flow.Cls == "驗收人" || flow.Cls == "設備主管")
                {
                    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                }
                if (flow.Cls == "巡檢工程師")
                {
                    listItem.Clear();
                    listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
                    listItem.Add(new SelectListItem { Text = "組長", Value = "組長" });
                    listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
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
        public ActionResult NextFlow(Assign assign)
        {
            if (ModelState.IsValid)
            {
                if (assign.FlowCls == "結案" || assign.FlowCls == "廢除")
                {
                    assign.FlowUid = WebSecurity.CurrentUserId;
                }
                InspectDocFlow df = db.InspectDocFlow.Where(f => f.DocId == assign.DocId && f.FlowStatusId == "?").FirstOrDefault();
                if (assign.FlowCls == "結案")
                {
                    InspectDocIdTable inspectDocId = db.InspectDocIdTable.Find(assign.DocId);
                    var inspectDocs = db.InspectDoc.Where(d => d.DocId == assign.DocId).ToList();
                    // Update close date.
                    inspectDocId.CloseDate = DateTime.Now;
                    inspectDocId.DocStatusId = "4";
                    foreach(var doc in inspectDocs)
                    {
                        doc.CloseDate = DateTime.Now;
                        db.Entry(doc).State = EntityState.Modified;
                    }
                    //
                    df.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    df.FlowStatusId = "2";
                    df.Rtt = DateTime.Now;
                    df.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(df).State = EntityState.Modified;
                    db.Entry(inspectDocId).State = EntityState.Modified;
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
                    InspectDocFlow flow = new InspectDocFlow();
                    flow.DocId = assign.DocId;
                    flow.StepId = df.StepId + 1;
                    flow.UserId = assign.FlowUid.Value;
                    flow.FlowStatusId = "?";
                    flow.Cls = assign.FlowCls;
                    flow.Rtt = DateTime.Now;
                    db.InspectDocFlow.Add(flow);
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
            InspectDocIdTable r = db.InspectDocIdTable.Find(docid);
            string g = "";

            switch (cls)
            {
                case "設備主管":
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
                    var inspectDocFlow = db.InspectDocFlow.Where(f => f.DocId == docid && f.Cls.Contains("工程師")).ToList();
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
                case "組長":
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
