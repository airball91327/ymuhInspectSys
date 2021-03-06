﻿using System;
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
using WebGrease.Css.Extensions;
using System.Security.AccessControl;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class InspectDocController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectDoc
        public async Task<ActionResult> Index()
        {
            // Get shift list.
            var inspectShifts = db.InspectShift.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var item in inspectShifts)
            {
                listItem.Add(new SelectListItem()
                {
                    Value = item.ShiftId.ToString(),
                    Text = item.ShiftName
                });
            }
            ViewData["ShiftId"] = new SelectList(listItem, "Value", "Text");
            //
            return View();
        }

        // POST: InspectDoc
        [HttpPost]
        [MyErrorHandler]
        public async Task<ActionResult> Index(InspectDocQryVModel qry)
        {
            // query variables.
            string docid = qry.DocId;
            string shiftId = qry.ShiftId; 

            // Get all inspect docs.
            var inspectDocs = db.InspectDoc.Include(d => d.InspectDocIdTable);
            var inspectShifts = await db.InspectShift.ToListAsync();
            // query conditions.
            if (!string.IsNullOrEmpty(docid))   //案件編號(關鍵字)
            {
                docid = docid.Trim();
                inspectDocs = inspectDocs.Where(d => d.DocId.Contains(docid));
            }
            if (!string.IsNullOrEmpty(shiftId))    //班別
            {
                int sid = Convert.ToInt32(shiftId);
                inspectDocs = inspectDocs.Where(d => d.ShiftId == sid);
            }

            return PartialView("List", inspectDocs.ToList());
        }

        // GET: InspectDoc/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectDocIdTable inspectDocIdTable = await db.InspectDocIdTable.FindAsync(id);
            if (inspectDocIdTable == null)
            {
                return HttpNotFound();
            }
            // Set variables.
            var shiftId = inspectDocIdTable.ShiftId;
            var docStatusId = inspectDocIdTable.DocStatusId;
            var docDetailTemps = db.InspectDocDetailTemp.Where(d => d.DocId == id && d.ShiftId == shiftId).ToList();
            var docDetailTempsClasses = docDetailTemps.GroupBy(t => t.ClassId).Select(g => g.FirstOrDefault())
                                                      .OrderBy(d => d.ClassOrder);
            var shiftName = docDetailTemps.First().ShiftName;
            var areaName = inspectDocIdTable.AreaName;
            List<InspectClassVModel> inspectClassVs = new List<InspectClassVModel>();
            InspectClassVModel classVModel;
            //
            foreach(var item in docDetailTempsClasses)
            {
                // Get class error fields.
                var classErrors = docDetailTemps.Where(d => d.ClassId == item.ClassId &&
                                                            d.IsFunctional == "N").ToList();
                // Get details of class.
                var findDocTemps = docDetailTemps.Where(d => d.ClassId == item.ClassId);

                /* Check all the required fields. */
                if (findDocTemps.Count() > 0)
                {
                    bool isDataCompleted = true;
                    foreach (var tempItem in findDocTemps)
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
                        item.IsSaved = true;
                    }
                }
                else
                {
                    item.IsSaved = false;
                }
                // Check all classes are fill out or not.
                var isAllSaved = docDetailTempsClasses.Where(c => c.IsSaved == false).ToList();
                if (isAllSaved.Count() <= 0)
                {
                    ViewBag.AllSaved = "true";
                }
                else
                {
                    ViewBag.AllSaved = "false";
                }
                // Insert values to classVModel.
                classVModel = new InspectClassVModel();
                classVModel.DocId = item.DocId;
                classVModel.AreaId = item.AreaId;
                classVModel.ShiftId = item.ShiftId;
                classVModel.ClassId = item.ClassId;
                classVModel.ClassName = item.ClassName;
                classVModel.ClassOrder = item.ClassOrder;
                classVModel.IsSaved = item.IsSaved;
                classVModel.CountErrors = classErrors.Count();
                inspectClassVs.Add(classVModel);
            }
            //
            var notes = GetDocNotes(id);
            if (notes != null)
            {
                ViewBag.Notes = notes;
            }
            //
            ViewBag.Header = areaName + "【" + shiftName + "】";
            if (docStatusId == "2") //交班中
            {
                return View("ShiftConfirm", inspectClassVs);
            }
            else if (docStatusId == "3") //簽核中
            {
                return View("ShiftConfirm", inspectClassVs);
            }
            else //巡檢中
            {
                return View(inspectClassVs);
            }
        }

        // POST: InspectDoc/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DocId,ShiftId,ApplyDate,EndTime,CloseDate,EngId,EngName,CheckerId,CheckerName")] InspectDoc inspectDoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspectDoc).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DocId = new SelectList(db.InspectDocIdTable, "DocId", "AreaName", inspectDoc.DocId);
            return View(inspectDoc);
        }

        /// <summary>
        /// Get InspectDocs' notes by docId.
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public string GetDocNotes (string docId)
        {
            string notes = null;
            var inspectDocs = db.InspectDoc.Where(d => d.DocId == docId).ToList();
            foreach (var doc in inspectDocs)
            {
                var shift = db.InspectShift.Find(doc.ShiftId);
                if (shift != null)
                {
                    notes += "【" + shift.ShiftName + "】" + doc.Note + Environment.NewLine;
                }
            }
            return notes;
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
