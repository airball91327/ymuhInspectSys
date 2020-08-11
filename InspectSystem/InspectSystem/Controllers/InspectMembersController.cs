using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using InspectSystem.Models;
using WebMatrix.WebData;

namespace InspectSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InspectMembersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectMembers
        public ActionResult Index()
        {
            var inspectMemberAreas = db.InspectMemberAreas.Include(i => i.InspectAreas)
                                                          .Include(i => i.InspectMembers)
                                                          .OrderBy(i => i.AreaId);
            var inspectMembers = db.InspectMembers.OrderBy(i => i.DptId);
            InspectMembersViewModel inspectMembersViewModel = new InspectMembersViewModel()
            {
                InspectMemberAreas = inspectMemberAreas.ToList(),
                InspectMembers = inspectMembers.ToList()
            };
            ViewBag.CountAreas = db.InspectAreas.Count();
            foreach(var item in inspectMembersViewModel.InspectMembers)
            {
                item.MemberUserName = db.AppUsers.Where(u => u.Id == item.MemberId).FirstOrDefault() == null ? "" : db.AppUsers.Where(u => u.Id == item.MemberId).FirstOrDefault().UserName;
                item.DptName = db.Departments.Where(d => d.DptId == item.DptId).FirstOrDefault() == null ? "" : db.Departments.Where(d => d.DptId == item.DptId).FirstOrDefault().Name_C;
            }
            return View(inspectMembersViewModel);
        }

        // GET: InspectMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Create/5
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /* New a InspectMemberAreas. */
            InspectMemberAreas inspectMemberAreas = new InspectMemberAreas();
            inspectMemberAreas.MemberId = db.InspectMembers.Find(id).MemberId;
            inspectMemberAreas.InspectMembers = db.InspectMembers.Find(id);

            /* Find the AreaIds which is not already exist. */
            var q = ( from a in db.InspectAreas select a.AreaId )
                    .Except( from m in db.InspectMemberAreas where m.MemberId == id select m.AreaId ).ToList();
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach(var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName");
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas)
        {
            if (ModelState.IsValid)
            {
                db.InspectMemberAreas.Add(inspectMemberAreas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            /* Find the AreaIds which is not already exist. */
            var id = inspectMemberAreas.MemberId;
            var q = (from a in db.InspectAreas select a.AreaId)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id select m.AreaId);
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName");
            inspectMemberAreas.InspectMembers = db.InspectMembers.Find(id);
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Create2/5
        public ActionResult Create2()
        {
            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            AppUser u;
            s = Roles.GetUsersInRole("MedEngineer").ToList();
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (!string.IsNullOrEmpty(u.DptId))
                {
                    li = new ListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["MemberId"] = new SelectList(list, "Value", "Text", list.First().Value);

            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in db.InspectAreas)
            {
                areaList.Add(item);
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas)
        {
            var memberExist = db.InspectMembers.Where(i => i.MemberId == inspectMemberAreas.MemberId).FirstOrDefault();
            if (memberExist != null)
            {
                ModelState.AddModelError("MemberId", "已有相同員工於列表中");
            }
            else
            {
                AppUser ur = db.AppUsers.Find(inspectMemberAreas.MemberId);
                InspectMembers inspectMember = new InspectMembers()
                {
                    MemberId = ur.Id,
                    MemberName = ur.FullName,
                    DptId = ur.DptId
                };
                db.InspectMembers.Add(inspectMember);
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                db.InspectMemberAreas.Add(inspectMemberAreas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            AppUser u;
            s = Roles.GetUsersInRole("MedEngineer").ToList();
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (!string.IsNullOrEmpty(u.DptId))
                {
                    li = new ListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["MemberId"] = new SelectList(list, "Value", "Text", list.First().Value);

            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in db.InspectAreas)
            {
                areaList.Add(item);
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName");
            return View(inspectMemberAreas);
        }

        // Get: InspectMembers/EditList/5
        public ActionResult EditList(int? id)
        {
            var inspectMemberAreas = db.InspectMemberAreas.Where(i => i.MemberId == id);
            if(inspectMemberAreas.Count() == 1)
            {
                return RedirectToAction("Edit", new { id = id, areaID = inspectMemberAreas.First().AreaId });
            }
            return View(inspectMemberAreas.ToList());
        }

        // GET: InspectMembers/Edit/5
        public ActionResult Edit(int? id, int? areaID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }

            /* Find the AreaIds which is not already exist and selected. */
            var q = (from a in db.InspectAreas select a.AreaId)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id && 
                                                                  m.AreaId != areaID select m.AreaId).ToList();
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName", areaID);
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberId,AreaId")] InspectMemberAreas inspectMemberAreas, int originAreaId)
        {
            if (ModelState.IsValid)
            {
                if(inspectMemberAreas.AreaId == originAreaId)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    /* Add a InspectMemberAreas. */
                    db.InspectMemberAreas.Add(inspectMemberAreas);
                    /* Delete old area. */
                    InspectMemberAreas deleteMemberAreas = db.InspectMemberAreas.Find(inspectMemberAreas.MemberId, originAreaId);
                    db.InspectMemberAreas.Remove(deleteMemberAreas);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            /* Find the AreaIds which is not already exist. */
            var id = inspectMemberAreas.MemberId;
            int areaID = originAreaId;
            var q = (from a in db.InspectAreas select a.AreaId)
                    .Except(from m in db.InspectMemberAreas where m.MemberId == id &&
                                                                  m.AreaId != areaID select m.AreaId);
            /* Insert Areas into dropdownlist. */
            List<InspectAreas> areaList = new List<InspectAreas>();
            foreach (var item in q)
            {
                areaList.Add(db.InspectAreas.Find(item));
            }
            ViewBag.AreaId = new SelectList(areaList, "AreaId", "AreaName", areaID);
            return View(inspectMemberAreas);
        }

        // GET: InspectMembers/Delete/5
        public ActionResult Delete(int? id, int? areaID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            if (inspectMemberAreas == null)
            {
                return HttpNotFound();
            }
            return View(inspectMemberAreas);
        }

        // POST: InspectMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int areaID)
        {
            InspectMemberAreas inspectMemberAreas = db.InspectMemberAreas.Find(id, areaID);
            db.InspectMemberAreas.Remove(inspectMemberAreas);
            db.SaveChanges();
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
