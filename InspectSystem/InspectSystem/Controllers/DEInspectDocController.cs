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
using InspectSystem.Models.DEquipment;
using WebMatrix.WebData;
using InspectSystem.Filters;
using X.PagedList;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class DEInspectDocController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;

        // GET: DEInspectDoc
        public async Task<ActionResult> Index()
        {
            // Get cycle list.
            var inspectAreas = db.DEInspectArea.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem() { Value = "", Text = "所有" });
            foreach (var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");
            // Get cycle list.
            var inspectCycles = db.DEInspectCycle.ToList();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem(){ Value = "", Text = "所有"} );
            foreach (var item in inspectCycles)
            {
                listItem2.Add(new SelectListItem()
                {
                    Value = item.CycleId.ToString(),
                    Text = item.CycleName
                });
            }
            ViewData["CycleId"] = new SelectList(listItem2, "Value", "Text");
            //
            return View();
        }

        // GET: DEInspectDoc
        public ActionResult Index2(DEInspectDocQryVModel qry, int page = 1)
        {
            // query variables.
            string docid = qry.DocId;
            string areaid = qry.AreaId;
            string cycleid = qry.CycleId;
            string classid = qry.ClassId;

            var inspectFlows = db.DEInspectDocFlow.AsQueryable();
            // Get user's docs.
            inspectFlows = inspectFlows.Where(df => df.FlowStatusId == "?" || df.FlowStatusId == "0")
                                       .Where(df => df.UserId == WebSecurity.CurrentUserId);
            var qryResult = inspectFlows.Join(db.DEInspectDoc, f => f.DocId, d => d.DocId, 
                                        (f, d) => new 
                                        { 
                                            inspectDoc = d,
                                            flow = f
                                        }).Join(db.AppUsers, f => f.inspectDoc.EngId, u => u.Id,
                                        (f, u) => new
                                        {
                                            inspectDoc = f.inspectDoc,
                                            flow = f.flow,
                                            eng = u
                                        }).Join(db.AppUsers, f => f.flow.UserId, u => u.Id,
                                        (f, u) => new
                                        {
                                            inspectDoc = f.inspectDoc,
                                            flow = f.flow,
                                            eng = f.eng,
                                            clsUser = u
                                        });
            // query conditions.
            if (!string.IsNullOrEmpty(docid))   //案件編號
            {
                docid = docid.Trim();
                qryResult = qryResult.Where(d => d.inspectDoc.DocId == docid);
            }
            if (!string.IsNullOrEmpty(areaid))    //區域
            {
                int id = Convert.ToInt32(areaid);
                qryResult = qryResult.Where(d => d.inspectDoc.AreaId == id);
            }
            if (!string.IsNullOrEmpty(cycleid))    //週期
            {
                int id = Convert.ToInt32(cycleid);
                qryResult = qryResult.Where(d => d.inspectDoc.CycleId == id);
            }
            if (!string.IsNullOrEmpty(classid))    //類別
            {
                int id = Convert.ToInt32(classid);
                qryResult = qryResult.Where(d => d.inspectDoc.ClassId == id);
            }
            //
            List<DEInspectDocVModel> returnList = new List<DEInspectDocVModel>();
            DEInspectDocVModel docVModel;
            foreach (var item in qryResult.ToList())
            {
                docVModel = new DEInspectDocVModel();
                docVModel.ApplyDate = item.inspectDoc.ApplyDate;
                docVModel.AreaId = item.inspectDoc.AreaId;
                docVModel.AreaName = item.inspectDoc.AreaName;
                docVModel.CheckerId = item.inspectDoc.CheckerId;
                docVModel.CheckerName = item.inspectDoc.CheckerName;
                docVModel.ClassId = item.inspectDoc.ClassId;
                docVModel.ClassName = item.inspectDoc.ClassName;
                docVModel.CloseDate = item.inspectDoc.CloseDate;
                docVModel.Cls = item.flow.Cls;
                docVModel.CycleId = item.inspectDoc.CycleId;
                docVModel.CycleName = item.inspectDoc.CycleName;
                docVModel.DocId = item.inspectDoc.DocId;
                docVModel.EndTime = item.inspectDoc.EndTime;
                docVModel.EngId = item.inspectDoc.EngId;
                docVModel.EngName = item.inspectDoc.EngName;
                docVModel.EngUserName = item.eng.UserName;
                docVModel.FlowStatusId = item.flow.FlowStatusId;
                docVModel.UserId = item.flow.UserId;
                docVModel.UserName = item.clsUser.UserName;
                docVModel.UserFullName = item.clsUser.FullName;
                docVModel.flow = item.flow;
                returnList.Add(docVModel);
            }
            var pageCount = returnList.ToPagedList(page, pageSize).PageCount;
            pageCount = pageCount == 0 ? 1 : pageCount; // If no page.
            if (returnList.ToPagedList(page, pageSize).Count <= 0)  //If the page has no items.
                return PartialView("List", returnList.ToPagedList(pageCount, pageSize));
            return PartialView("List", returnList.ToPagedList(page, pageSize));
        }

        // GET: DEInspectDoc/Views/5
        public async Task<ActionResult> Views(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            // Insert values to classVModel.
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            classVModel.DocId = DEInspectDoc.DocId;
            classVModel.AreaId = DEInspectDoc.AreaId;
            classVModel.CycleId = DEInspectDoc.CycleId;
            classVModel.ClassId = DEInspectDoc.ClassId;
            //
            ViewBag.Header = DEInspectDoc.AreaName + "【" + DEInspectDoc.CycleName + "】" + "巡檢單";
            return View(classVModel);
        }

        // GET: DEInspectDoc/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = db.DEInspectDoc.Find(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return PartialView(DEInspectDoc);
        }

        // GET: DEInspectDoc/Create
        public ActionResult Create()
        {
            // Get inspect area list.
            var inspectAreas = db.DEInspectArea.Where(a => a.AreaStatus == true).ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var item in inspectAreas)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.AreaId.ToString(),
                    Text = item.AreaName
                });
            }
            ViewData["AreaId"] = new SelectList(listItem, "Value", "Text");
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            ViewData["CycleId"] = new SelectList(listItem2, "Value", "Text");
            List<SelectListItem> listItem3 = new List<SelectListItem>();
            ViewData["ClassId"] = new SelectList(listItem3, "Value", "Text");
            //
            // Set default value.
            DEInspectDoc inspectDoc = new DEInspectDoc();
            inspectDoc.ApplyDate = DateTime.Now;
            return View(inspectDoc);
        }

        // POST: DEInspectDoc/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyErrorHandler]
        public async Task<ActionResult> Create([Bind(Include = "ApplyDate,AreaId,AreaName,CycleId,CycleName,ClassId,ClassName")] DEInspectDoc DEInspectDoc)
        {
            AppUser user;
            if (ModelState.IsValid)
            {
                var areaId = DEInspectDoc.AreaId;
                var cycleId = DEInspectDoc.CycleId;
                var classId = DEInspectDoc.ClassId;
                var applyDate = DEInspectDoc.ApplyDate;
                var docId = GetDocId(applyDate);
                //
                try
                {
                    // 產生巡檢單
                    user = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    DEInspectDoc.DocId = docId;
                    DEInspectDoc.AreaName = DEInspectDoc.AreaName;
                    DEInspectDoc.CycleName = DEInspectDoc.CycleName;
                    DEInspectDoc.ClassName = DEInspectDoc.ClassName;
                    DEInspectDoc.ApplyDate = applyDate;
                    DEInspectDoc.EngId = user.Id;
                    DEInspectDoc.EngName = user.FullName;
                    db.DEInspectDoc.Add(DEInspectDoc);
                    await db.SaveChangesAsync();
                    // 擷取巡檢單內容
                    DECyclesInAreas cyclesInAreas = db.DECyclesInAreas.Include(i => i.DEInspectArea).Include(i => i.DEInspectCycle)
                                                                      .Where(i => i.AreaId == areaId && i.CycleId == cycleId).FirstOrDefault();
                    List<DEInspectItem> inspectItems = db.DEInspectItem.Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId).ToList();
                    List<DEInspectField> inspectFields = db.DEInspectField.Include(i => i.DEInspectItem).Include(i => i.DEInspectItem.DEInspectClass)
                                                                          .Where(i => i.AreaId == areaId && i.CycleId == cycleId && i.ClassId == classId)
                                                                          .Where(i => i.DEInspectItem.ItemStatus == true)
                                                                          .Where(i => i.FieldStatus == true).ToList();
                    // Insert values.
                    DEInspectDocDetailTemp detailTemp;
                    foreach (var insField in inspectFields)
                    {
                        detailTemp = new DEInspectDocDetailTemp();
                        detailTemp.DocId = docId;
                        detailTemp.AreaId = insField.AreaId;
                        detailTemp.AreaName = cyclesInAreas.DEInspectArea.AreaName;
                        detailTemp.CycleId = insField.CycleId;
                        detailTemp.CycleName = cyclesInAreas.DEInspectCycle.CycleName;
                        detailTemp.ClassId = insField.ClassId;
                        detailTemp.ClassName = insField.DEInspectItem.DEInspectClass.ClassName;
                        detailTemp.ClassOrder = insField.DEInspectItem.DEInspectClass.ClassOrder;
                        detailTemp.ItemId = insField.ItemId;
                        detailTemp.ItemName = insField.DEInspectItem.ItemName;
                        detailTemp.ItemOrder = insField.DEInspectItem.ItemOrder;
                        detailTemp.FieldId = insField.FieldId;
                        detailTemp.FieldName = insField.FieldName;
                        detailTemp.DataType = insField.DataType;
                        detailTemp.UnitOfData = insField.UnitOfData;
                        detailTemp.MinValue = insField.MinValue;
                        detailTemp.MaxValue = insField.MaxValue;
                        detailTemp.IsRequired = insField.IsRequired;
                        /* If field is dropdown, set dropdownlist items to string and save to DB. */
                        if (insField.DataType == "dropdownlist")
                        {
                            var itemDropDownList = db.DEInspectFieldDropDown.Where(i => i.AreaId == insField.AreaId &&
                                                                                        i.CycleId == insField.CycleId &&
                                                                                        i.ClassId == insField.ClassId &&
                                                                                        i.ItemId == insField.ItemId &&
                                                                                        i.FieldId == insField.FieldId).ToList();
                            foreach (var dropItem in itemDropDownList)
                            {
                                detailTemp.DropDownItems += dropItem.Value.ToString() + ";";
                            }
                        }
                        //
                        db.DEInspectDocDetailTemp.Add(detailTemp);
                    }
                    await db.SaveChangesAsync();

                    //Create first Flow.
                    DEInspectDocFlow flow = new DEInspectDocFlow();
                    flow.DocId = docId;
                    flow.StepId = 1;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "1";  // 流程狀態"已處理"
                    flow.Rtp = WebSecurity.CurrentUserId;
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "申請人";
                    db.DEInspectDocFlow.Add(flow);

                    // Create next flow.
                    flow = new DEInspectDocFlow();
                    flow.DocId = docId;
                    flow.StepId = 2;
                    flow.UserId = WebSecurity.CurrentUserId;
                    flow.FlowStatusId = "0";  // 狀態"巡檢中"
                    flow.Rtt = DateTime.Now;
                    flow.Cls = "巡檢工程師";
                    db.DEInspectDocFlow.Add(flow);
                    await db.SaveChangesAsync();

                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                catch (Exception e)
                {
                    throw new Exception("申請失敗!");
                }
            }
            //
            return new JsonResult
            {
                Data = new { success = false, error = "申請失敗!" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: DEInspectDoc/EditTemp/5
        public async Task<ActionResult> EditTemp(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            //
            var docDetailTemps = db.DEInspectDocDetailTemp.Where(d => d.DocId == id).ToList();
            // Get class error fields.
            var classErrors = docDetailTemps.Where(d => d.IsFunctional == "N").ToList();
            // Insert values to classVModel.
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            classVModel.DocId = DEInspectDoc.DocId;
            classVModel.AreaId = DEInspectDoc.AreaId;
            classVModel.CycleId = DEInspectDoc.CycleId;
            classVModel.ClassId = DEInspectDoc.ClassId;
            classVModel.CountErrors = classErrors.Count();
            /* Check all the required fields. */
            if (docDetailTemps.Count() > 0)
            {
                bool isDataCompleted = true;
                foreach (var tempItem in docDetailTemps)
                {
                    // If required field has no data or isFunctional didn't selected, set isDataCompleted to false.
                    if (tempItem.IsRequired == true && tempItem.DataType != "boolean" && tempItem.Value == null)
                    {
                        isDataCompleted = false;
                        break;
                    }
                    else if (tempItem.IsRequired == true && tempItem.DataType == "checkbox" && tempItem.Value == "false")
                    {
                        isDataCompleted = false;
                        break;
                    }
                    else if (tempItem.DataType == "boolean" && tempItem.IsFunctional == null)
                    {
                        isDataCompleted = false;
                        break;
                    }
                }
                if (isDataCompleted == true)
                {
                    classVModel.IsSaved = true;
                }
            }
            else
            {
                classVModel.IsSaved = false;
            }
            //
            if (classVModel.IsSaved == true)
            {
                ViewBag.AllSaved = "true";
            }
            else
            {
                ViewBag.AllSaved = "false";
            }
            //
            ViewBag.Header = DEInspectDoc.AreaName + "【" + DEInspectDoc.CycleName + "】" + "巡檢單";
            return View(classVModel);
        }

        /// <summary>
        /// 工程師的編輯畫面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: DEInspectDoc/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            // Insert values to classVModel.
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            classVModel.DocId = DEInspectDoc.DocId;
            classVModel.AreaId = DEInspectDoc.AreaId;
            classVModel.CycleId = DEInspectDoc.CycleId;
            classVModel.ClassId = DEInspectDoc.ClassId;
            //
            ViewBag.Header = DEInspectDoc.AreaName + "【" + DEInspectDoc.CycleName + "】" + "巡檢單";
            return View(classVModel);
        }

        /// <summary>
        /// 非工程師的編輯畫面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: DEInspectDoc/Edit2/5
        public async Task<ActionResult> Edit2(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc DEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (DEInspectDoc == null)
            {
                return HttpNotFound();
            }
            // Insert values to classVModel.
            DEInspectClassVModel classVModel = new DEInspectClassVModel(); ;
            classVModel.DocId = DEInspectDoc.DocId;
            classVModel.AreaId = DEInspectDoc.AreaId;
            classVModel.CycleId = DEInspectDoc.CycleId;
            classVModel.ClassId = DEInspectDoc.ClassId;
            //
            ViewBag.Header = DEInspectDoc.AreaName + "【" + DEInspectDoc.CycleName + "】" + "巡檢單";
            return View(classVModel);
        }

        /// <summary>
        /// Get DEInspect DocId.
        /// </summary>
        /// <returns></returns>
        private string GetDocId(DateTime ApplyDate)
        {
            string maxId = db.DEInspectDoc.Select(r => r.DocId).Max();
            string docId = "";
            int yymmdd = (ApplyDate.Year - 1911) * 10000 + (ApplyDate.Month) * 100 + ApplyDate.Day; //例:1090101
            if (!string.IsNullOrEmpty(maxId))
            {
                docId = maxId;
            }
            if (docId != "")
            {
                if (Convert.ToInt64(docId) / 1000 == yymmdd)
                    docId = Convert.ToString(Convert.ToInt64(docId) + 1);
                else
                    docId = Convert.ToString(yymmdd * 1000 + 1);
            }
            else
            {
                docId = Convert.ToString(yymmdd * 1000 + 1);
            }
            return docId;
        }

        // GET: DEInspectDoc/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            if (dEInspectDoc == null)
            {
                return HttpNotFound();
            }
            return View(dEInspectDoc);
        }

        // POST: DEInspectDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            DEInspectDoc dEInspectDoc = await db.DEInspectDoc.FindAsync(id);
            db.DEInspectDoc.Remove(dEInspectDoc);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
