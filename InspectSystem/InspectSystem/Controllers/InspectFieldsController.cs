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
        /// <summary>
        /// Search inspectFields.
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="shiftId"></param>
        /// <param name="classId"></param>
        /// <param name="itemId"></param>
        /// <returns>The search results of inspectFields' list.</returns>
        public ActionResult Search(int areaId, int shiftId, int classId, int itemId)
        {
            var SearchResult = db.InspectFields.Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                           i.ClassId == classId && i.ItemId == itemId);

            ViewBag.ItemId = itemId;
            return PartialView(SearchResult.ToList());
        }

        // Not been used.
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InspectFields inspectFields, FormCollection collection)
        {
            // Set variables
            int areaId = inspectFields.AreaId;
            int shiftId = inspectFields.ShiftId;
            int classId = inspectFields.ClassId;
            int itemId = inspectFields.ItemId;

            int fieldCount = db.InspectFields.Where(fc => fc.AreaId == areaId && fc.ShiftId == shiftId &&
                                                          fc.ClassId == classId && fc.ItemId == itemId).Count();
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
                                    ItemId = itemId,
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
                return RedirectToAction("Search", new { ItemId = itemId });
            }
            return RedirectToAction("Search", new { ItemId = itemId });
        }

        // GET: InspectFields/Edit/5
        public ActionResult Edit(int? AreaId, int? ShiftId, int? ClassId, int? ItemId, int? FieldId)
        {

            ViewBag.ItemNameForEdit = db.InspectItems.Find(AreaId, ShiftId, ClassId, ItemId).ItemName;

            var DropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId &&
                                                                  i.ClassId == ClassId && i.ItemId == ItemId &&
                                                                  i.FieldId == FieldId)
                                                      .OrderBy(i => i.Id);
            TempData["DropDownList"] = DropDownList.ToList();

            if (AreaId == null || ShiftId == null || ClassId == null || ItemId == null || FieldId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectFields inspectFields = db.InspectFields.Find(AreaId, ShiftId, ClassId, ItemId, FieldId);
            if (inspectFields == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectFields);
        }

        // POST: InspectFields/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InspectFields inspectFields, FormCollection collection)
        {
            // Set variables
            int areaId = inspectFields.AreaId;
            int shiftId = inspectFields.ShiftId;
            int classId = inspectFields.ClassId;
            int itemId = inspectFields.ItemId;
            var fieldId = inspectFields.FieldId;

            if (ModelState.IsValid)
            {
                if(inspectFields.DataType == "dropdownlist")
                {
                    /* for datatype dropdownlist, and dynamic inset textbox. */
                    var inputCount = 0;
                    var DropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                                          i.ClassId == classId && i.ItemId == itemId &&
                                                                          i.FieldId == fieldId)
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
                                        AreaId = areaId,
                                        ShiftId = shiftId,
                                        ClassId = classId,
                                        ItemId = itemId,
                                        FieldId = fieldId,
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
                return RedirectToAction("Search",new { areaId = areaId, shiftId = shiftId, classId = classId, itemId = itemId });
            }
            return RedirectToAction("Search", new { areaId = areaId, shiftId = shiftId, classId = classId, itemId = itemId });
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
