using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectFieldsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectFields
        public ActionResult Index()
        {
            return RedirectToAction("Index", "InspectItems", null);
        }

        // GET: InspectFields/Search
        /* Use ACID and ItemId to search the fields. */
        public ActionResult Search(int acid, int ItemId)
        {
            //var SearchResult = db.InspectFields
            //                     .Where(i => i.ACID == acid &&
            //                                 i.ItemId == ItemId);
            //ViewBag.ACID = acid;
            //ViewBag.ItemId = ItemId;
            return PartialView(/*SearchResult.ToList()*/);
        }

        /* Unused code
        // GET: InspectFields/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(id);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }
        */

        // GET: InspectFields/Create
        public ActionResult Create(int acid, int ItemId)
        {
            /* Pass the ACID, ItemId, ItemName for View to print. */
            ViewBag.CreateACID = acid;
            ViewBag.CreateItemId = ItemId;
            ViewBag.ItemNameForCreate = db.InspectItems.Find(acid, ItemId).ItemName;

            /* Set the default values for create field. */
            InspectFields inspectFields = new InspectFields
            {
                ItemId = ItemId,
                MaxValue = 0,
                MinValue = 0,
                FieldStatus = true,
                IsRequired = true
            };

            return PartialView(inspectFields);
        }

        // POST: InspectFields/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InspectFields inspectFields, FormCollection collection)
        {
            // Set variables
            int ItemId = inspectFields.ItemId;

            int fieldCount = db.InspectFields.Count(fc => fc.ACID == ACID && fc.ItemId == ItemId);
            int FieldId = fieldCount + 1;
            inspectFields.FieldId = FieldId;

            if (ModelState.IsValid)
            {
                /* for datatype dropdownlist, and dynamic inset textbox. */
                if (inspectFields.DataType == "dropdownlist")
                { 
                    var inputCount = 0;

                    if (int.TryParse(collection["TextBoxCount"], out inputCount))
                    {
                        for (int i = 1; i <= inputCount; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(collection["textbox" + i]))
                            {
                                InspectFieldDropDown inspectFieldDropDown = new InspectFieldDropDown
                                {
                                    ItemId = ItemId,
                                    FieldId = FieldId,
                                    Value = collection["textbox" + i]
                                };
                                db.InspectFieldDropDown.Add(inspectFieldDropDown);
                            }
                        }
                    }
                }

                db.InspectFields.Add(inspectFields);
                db.SaveChanges();
                return RedirectToAction("Search", new { ItemId = ItemId });
            }
            return RedirectToAction("Search", new { ItemId = ItemId });
        }

        // GET: InspectFields/Edit/5
        public ActionResult Edit(int? ACID, int? ItemId, int? FieldId)
        {

            ViewBag.ItemNameForEdit = db.InspectItems.Find(ACID, ItemId).ItemName;

            var DropDownList = db.InspectFieldDropDown.Where(i => i.ACID == ACID &&
                                                                  i.ItemId == ItemId &&
                                                                  i.FieldId == FieldId)
                                                      .OrderBy(i => i.Id);
            TempData["DropDownList"] = DropDownList.ToList();

            if (ACID == null || ItemId == null || FieldId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(ACID, ItemId, FieldId);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // POST: InspectFields/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InspectFields inspectFields, FormCollection collection)
        {
            var ACID = inspectFields.ACID;
            var ItemId = inspectFields.ItemId;
            var FieldId = inspectFields.FieldId;

            if (ModelState.IsValid)
            {
                if(inspectFields.DataType == "dropdownlist")
                {
                    /* for datatype dropdownlist, and dynamic inset textbox. */
                    var inputCount = 0;
                    var DropDownList = db.InspectFieldDropDown.Where(i => i.ACID == ACID &&
                                                                          i.ItemId == ItemId &&
                                                                          i.FieldId == FieldId)
                                                              .OrderBy(i => i.Id);

                    if (int.TryParse(collection["TextBoxCount"], out inputCount))
                    {
                        // If insert data is more than origin.
                        if (inputCount > DropDownList.Count())
                        {
                            var i = 1;
                            foreach (var item in DropDownList)
                            {
                                if (!string.IsNullOrWhiteSpace(collection["textbox" + i]))
                                {
                                    item.Value = collection["textbox" + i];
                                    db.Entry(item).State = EntityState.Modified;
                                }
                                i++;
                            }
                            for (int j = i; j <= inputCount; j++)
                            {
                                if (!string.IsNullOrWhiteSpace(collection["textbox" + j]))
                                {
                                    InspectFieldDropDown inspectFieldDropDown = new InspectFieldDropDown
                                    {
                                        ACID = ACID,
                                        ItemId = ItemId,
                                        FieldId = FieldId,
                                        Value = collection["textbox" + j]
                                    };
                                    db.InspectFieldDropDown.Add(inspectFieldDropDown);
                                }
                            }
                        }
                        else if (inputCount < DropDownList.Count())
                        {
                            var i = 1;
                            foreach (var item in DropDownList)
                            {
                                if (!string.IsNullOrWhiteSpace(collection["textbox" + i]))
                                {
                                    if (i <= inputCount)
                                    {
                                        item.Value = collection["textbox" + i];
                                        db.Entry(item).State = EntityState.Modified;
                                    }
                                }
                                else
                                {
                                    db.InspectFieldDropDown.Remove(item);
                                }
                                i++;
                            }
                        }
                        else
                        {
                            var i = 1;
                            foreach (var item in DropDownList)
                            {
                                if (!string.IsNullOrWhiteSpace(collection["textbox" + i]))
                                {
                                    item.Value = collection["textbox" + i];
                                    db.Entry(item).State = EntityState.Modified;
                                }
                                i++;
                            }
                        }
                    }
                }

                db.Entry(inspectFields).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Search",new { acid = ACID, ItemId = ItemId });
            }
            return RedirectToAction("Search", new { acid = ACID, ItemId = ItemId });
        }

        /* Unused code
        // GET: InspectFields/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(id);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // POST: InspectFields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InspectFields inspectFields = db.InspectFields.Find(id);
            db.InspectFields.Remove(inspectFields);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */

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
