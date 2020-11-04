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
using Microsoft.Ajax.Utilities;
using WebMatrix.WebData;

namespace InspectSystem.Areas.Admin.Controllers
{
    public class InspectFieldController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/InspectField
        public async Task<ActionResult> Index()
        {
            var inspectField = db.InspectField.Include(i => i.InspectItem);
            return View(await inspectField.ToListAsync());
        }

        // GET: Admin/InspectField/Create
        public ActionResult Create(int AreaId, int ShiftId, int ClassId, int ItemId)
        {
            /* Set the default values for create field. */
            InspectField inspectField = new InspectField
            {
                AreaId = AreaId,
                ShiftId = ShiftId,
                ClassId = ClassId,
                ItemId = ItemId,
                MaxValue = 0,
                MinValue = 0,
                FieldStatus = true,
                IsRequired = true
            };
            var item = db.InspectItem.Find(AreaId, ShiftId, ClassId, ItemId);
            if (item != null)
            {
                ViewBag.ItemName = item.ItemName;
                ViewBag.ItemOrder = item.ItemOrder;
            }
            return PartialView(inspectField);
        }

        // POST: Admin/InspectField/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InspectField inspectField, FormCollection collection)
        {
            // Set variables
            var areaId = inspectField.AreaId;
            var shiftId = inspectField.ShiftId;
            var classId = inspectField.ClassId;
            var itemId = inspectField.ItemId;

            int fieldCount = db.InspectField.Count(fc => fc.AreaId == areaId && fc.ShiftId == shiftId &&
                                                         fc.ClassId == classId && fc.ItemId == itemId);
            int fieldId = fieldCount + 1;
            inspectField.FieldId = fieldId;

            if (ModelState.IsValid)
            {
                /* for datatype dropdownlist, and dynamic inset textbox. */
                if (inspectField.DataType == "dropdownlist")
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
                                    AreaId = areaId,
                                    ShiftId = shiftId,
                                    ClassId = classId,
                                    ItemId = itemId,
                                    FieldId = fieldId,
                                    Value = collection["textbox" + i]
                                };
                                db.InspectFieldDropDown.Add(inspectFieldDropDown);
                            }
                        }
                    }
                }

                inspectField.Rtp = WebSecurity.CurrentUserId;
                inspectField.Rtt = DateTime.Now;
                db.InspectField.Add(inspectField);
                db.SaveChanges();
                return RedirectToAction("GetFieldList", new { AreaId = areaId, ShiftId = shiftId, ClassId = classId, ItemId = itemId });
            }
            return RedirectToAction("GetFieldList", new { AreaId = areaId, ShiftId = shiftId, ClassId = classId, ItemId = itemId });
        }

        // GET: Admin/InspectField/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? ShiftId, int? ClassId, int? ItemId, int? FieldId)
        {
            var DropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId &&
                                                                  i.ClassId == ClassId && i.ItemId == ItemId &&
                                                                  i.FieldId == FieldId).OrderBy(i => i.Id);
            TempData["DropDownList"] = DropDownList.ToList();
            if (AreaId == null || ShiftId == null || ClassId == null || ItemId == null || FieldId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectField inspectField = await db.InspectField.FindAsync(AreaId, ShiftId, ClassId, ItemId, FieldId);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectField);
        }

        // POST: Admin/InspectField/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InspectField inspectField, FormCollection collection)
        {
            var areaId = inspectField.AreaId;
            var shiftId = inspectField.ShiftId;
            var classId = inspectField.ClassId;
            var itemId = inspectField.ItemId;
            var fieldId = inspectField.FieldId;

            if (ModelState.IsValid)
            {
                if (inspectField.DataType == "dropdownlist")
                {
                    /* for datatype dropdownlist, and dynamic inset textbox. */
                    var inputCount = 0;
                    var DropDownList = db.InspectFieldDropDown.Where(i => i.AreaId == areaId && i.ShiftId == shiftId &&
                                                                          i.ClassId == classId && i.ItemId == itemId &&
                                                                          i.FieldId == fieldId).OrderBy(i => i.Id);

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

                inspectField.Rtp = WebSecurity.CurrentUserId;
                inspectField.Rtt = DateTime.Now;
                db.Entry(inspectField).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetFieldList", new { AreaId = areaId, ShiftId = shiftId, ClassId = classId, ItemId = itemId });
            }
            return RedirectToAction("GetFieldList", new { AreaId = areaId, ShiftId = shiftId, ClassId = classId, ItemId = itemId });
        }

        // GET: Admin/InspectField/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectField inspectField = await db.InspectField.FindAsync(id);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            return View(inspectField);
        }

        // POST: Admin/InspectField/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            InspectField inspectField = await db.InspectField.FindAsync(id);
            db.InspectField.Remove(inspectField);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/InspectField/GetFieldList/5
        public async Task<ActionResult> GetFieldList(int? AreaId, int? ShiftId, int? ClassId, int? ItemId)
        {
            var inspectFields = db.InspectField.Include(i => i.InspectItem)
                                               .Where(i => i.AreaId == AreaId && i.ShiftId == ShiftId && 
                                                           i.ClassId == ClassId && i.ItemId == ItemId).ToList();
            foreach (var item in inspectFields)
            {
                var user = db.AppUsers.Find(item.Rtp);
                if (user != null)
                {
                    item.RtpName = user.FullName + "(" + user.UserName + ")";   // Get Rtp's name.
                }
            }
            ViewData["AREAID"] = AreaId;
            ViewData["SHIFTID"] = ShiftId;
            ViewData["CLASSID"] = ClassId;
            ViewData["ITEMID"] = ItemId;

            return PartialView(inspectFields);
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
