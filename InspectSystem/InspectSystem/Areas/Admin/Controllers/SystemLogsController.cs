using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InspectSystem.Models;
using PagedList;
using WebMatrix.WebData;

namespace InspectSystem.Areas.Admin.Controllers
{
    [Authorize]
    public class SystemLogsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 50;

        // GET: MedEngMgt/SystemLogs
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                string logClass = "系統管理者紀錄";
                string logAction = "系統訊息紀錄";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
            }
            //
            //var logClasses = db.SystemLogs.Select(s => s.LogClass).Distinct().ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "請選擇", Value = "" });
            listItem.Add(new SelectListItem { Text = "管理紀錄", Value = "管理紀錄" });
            listItem.Add(new SelectListItem { Text = "醫療儀器紀錄", Value = "醫療儀器紀錄" });
            listItem.Add(new SelectListItem { Text = "系統管理者紀錄", Value = "系統管理者紀錄" });
            listItem.Add(new SelectListItem { Text = "系統錯誤訊息", Value = "系統錯誤訊息" });
            ViewData["qryLogClass"] = listItem;
            return View();
        }

        // GET: MedEngMgt/SystemLogs/List
        public ActionResult List(string qryLogClass, string qryUserName, int page = 1)
        {
            var qrySystemLogs = db.SystemLogs.Join(db.AppUsers, s => s.UserId, u => u.Id,
                                              (s, u) => new
                                              {
                                                  log = s,
                                                  user = u
                                              });
            if (!string.IsNullOrEmpty(qryLogClass))
            {
                qrySystemLogs = qrySystemLogs.Where(s => s.log.LogClass == qryLogClass);
            }
            if (!string.IsNullOrEmpty(qryUserName))
            {
                qrySystemLogs = qrySystemLogs.Where(s => s.user.UserName == qryUserName);
            }
            List<SystemLog> systemLogs = new List<SystemLog>();
            qrySystemLogs.ToList()
            .ForEach(s => systemLogs.Add(new SystemLog()
            {
                Id = s.log.Id,
                Action = s.log.Action,
                LogClass = s.log.LogClass,
                LogTime = s.log.LogTime,
                UserName = s.user.UserName
            }));

            systemLogs = systemLogs.OrderByDescending(s => s.LogTime).ToList();
            if (systemLogs.ToPagedList(page, pageSize).Count <= 0)
                return PartialView(systemLogs.ToPagedList(1, pageSize));
            return PartialView(systemLogs.ToPagedList(page, pageSize));
        }

        // GET: MedEngMgt/SystemLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/SystemLogs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LogClass,LogTime,UserId,Action")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.SystemLogs.Add(systemLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: MedEngMgt/SystemLogs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LogClass,LogTime,UserId,Action")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: MedEngMgt/SystemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemLog systemLog = db.SystemLogs.Find(id);
            db.SystemLogs.Remove(systemLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Save system log to DB.
        /// </summary>
        /// <param name="logClass">Class of system log.</param>
        /// <param name="logAction">User action.</param>
        /// <returns>If save success return true, else false.</returns>
        public bool SaveLog(string logClass, string logAction)
        {
            SystemLog log = new SystemLog();
            log.LogClass = logClass;
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.UserId = WebSecurity.CurrentUserId;
            log.Action = logAction;
            try
            {
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Save system log to DB.
        /// </summary>
        /// <param name="logClass">Class of system log.</param>
        /// <param name="logAction1">User action.</param>
        /// <param name="logAction2">String list of user actions.</param>
        /// <returns>If save success return true, else false.</returns>
        public bool SaveLog(string logClass, string logAction1, IEnumerable<string> logAction2)
        {
            SystemLog log = new SystemLog();
            log.LogClass = logClass;
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.UserId = WebSecurity.CurrentUserId;
            log.Action = logAction1;
            if (logAction2.Count() > 0)
            {
                foreach (string s in logAction2)
                {
                    log.Action += "【" + s + "】";
                }
            }
            try
            {
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
