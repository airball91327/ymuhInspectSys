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
using InspectSystem.Filters;
using InspectSystem.Models.DEquipment;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DEInspectFieldController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: Admin/DEInspectField
        public async Task<ActionResult> Index()
        {
            var inspectField = db.DEInspectField.Include(i => i.DEInspectItem);
            return View(await inspectField.ToListAsync());
        }

        // GET: Admin/DEInspectField/Create
        public ActionResult Create(int AreaId, int CycleId, int ClassId, int ItemId)
        {
            /* Set the default values for create field. */
            DEInspectField inspectField = new DEInspectField
            {
                AreaId = AreaId,
                CycleId = CycleId,
                ClassId = ClassId,
                ItemId = ItemId,
                MaxValue = 0,
                MinValue = 0,
                FieldStatus = true,
                IsRequired = true
            };
            var item = db.DEInspectItem.Find(AreaId, CycleId, ClassId, ItemId);
            if (item != null)
            {
                ViewBag.ItemName = item.ItemName;
                ViewBag.ItemOrder = item.ItemOrder;
            }
            return PartialView(inspectField);
        }

        // POST: Admin/DEInspectField/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DEInspectField inspectField, FormCollection collection)
        {
            // Set variables
            var areaId = inspectField.AreaId;
            var cycleId = inspectField.CycleId;
            var classId = inspectField.ClassId;
            var itemId = inspectField.ItemId;

            int fieldCount = db.DEInspectField.Count(fc => fc.AreaId == areaId && fc.CycleId == cycleId &&
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
                                DEInspectFieldDropDown inspectFieldDropDown = new DEInspectFieldDropDown
                                {
                                    AreaId = areaId,
                                    CycleId = cycleId,
                                    ClassId = classId,
                                    ItemId = itemId,
                                    FieldId = fieldId,
                                    Value = collection["textbox" + i]
                                };
                                db.DEInspectFieldDropDown.Add(inspectFieldDropDown);
                            }
                        }
                    }
                }

                inspectField.Rtp = WebSecurity.CurrentUserId;
                inspectField.Rtt = DateTime.Now;
                db.DEInspectField.Add(inspectField);
                db.SaveChanges();
                return RedirectToAction("GetFieldList", new { AreaId = areaId, CycleId = cycleId, ClassId = classId, ItemId = itemId });
            }
            return RedirectToAction("GetFieldList", new { AreaId = areaId, CycleId = cycleId, ClassId = classId, ItemId = itemId });
        }

        // GET: Admin/DEInspectField/Edit/5
        public async Task<ActionResult> Edit(int? AreaId, int? CycleId, int? ClassId, int? ItemId, int? FieldId)
        {
            var DropDownList = db.DEInspectFieldDropDown.Where(i => i.AreaId == AreaId && i.CycleId == CycleId &&
                                                                    i.ClassId == ClassId && i.ItemId == ItemId &&
                                                                    i.FieldId == FieldId).OrderBy(i => i.Id);
            TempData["DropDownList"] = DropDownList.ToList();
            if (AreaId == null || CycleId == null || ClassId == null || ItemId == null || FieldId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectField inspectField = await db.DEInspectField.FindAsync(AreaId, CycleId, ClassId, ItemId, FieldId);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            return PartialView(inspectField);
        }

        // POST: Admin/DEInspectField/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [MyErrorHandler]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DEInspectField inspectField, FormCollection collection)
        {
            var areaId = inspectField.AreaId;
            var cycleId = inspectField.CycleId;
            var classId = inspectField.ClassId;
            var itemId = inspectField.ItemId;
            var fieldId = inspectField.FieldId;

            if (ModelState.IsValid)
            {
                if (inspectField.DataType == "dropdownlist")
                {
                    int boxCount = int.Parse(collection["TextBoxCount"]);
                    for (int j = 1; j <= boxCount; j++)
                    {
                        if (string.IsNullOrWhiteSpace(collection["textbox" + j]))
                        {
                            throw new Exception("下拉選單的項目尚未輸入任何內容。");
                        }
                    }
                    /* for datatype dropdownlist, and dynamic inset textbox. */
                    var inputCount = 0;
                    var DropDownList = db.DEInspectFieldDropDown.Where(i => i.AreaId == areaId && i.CycleId == cycleId &&
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
                                    DEInspectFieldDropDown inspectFieldDropDown = new DEInspectFieldDropDown
                                    {
                                        AreaId = areaId,
                                        CycleId = cycleId,
                                        ClassId = classId,
                                        ItemId = itemId,
                                        FieldId = fieldId,
                                        Value = collection["textbox" + j]
                                    };
                                    db.DEInspectFieldDropDown.Add(inspectFieldDropDown);
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
                                    db.DEInspectFieldDropDown.Remove(item);
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
                return RedirectToAction("GetFieldList", new { AreaId = areaId, CycleId = cycleId, ClassId = classId, ItemId = itemId });
            }
            return RedirectToAction("GetFieldList", new { AreaId = areaId, CycleId = cycleId, ClassId = classId, ItemId = itemId });
        }

        // GET: Admin/DEInspectField/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEInspectField inspectField = await db.DEInspectField.FindAsync(id);
            if (inspectField == null)
            {
                return HttpNotFound();
            }
            return View(inspectField);
        }

        // POST: Admin/DEInspectField/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DEInspectField inspectField = await db.DEInspectField.FindAsync(id);
            db.DEInspectField.Remove(inspectField);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/DEInspectField/GetFieldList/5
        public async Task<ActionResult> GetFieldList(int? AreaId, int? CycleId, int? ClassId, int? ItemId)
        {
            var inspectFields = db.DEInspectField.Include(i => i.DEInspectItem)
                                                 .Where(i => i.AreaId == AreaId && i.CycleId == CycleId && 
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
            ViewData["CYCLEID"] = CycleId;
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
