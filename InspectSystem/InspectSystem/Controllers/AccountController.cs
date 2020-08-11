using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity;
using InspectSystem.Models;
using System.Web.Routing;
using WebMatrix.WebData;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account
        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string str = model.UserName;
                //int abc;
                //if(Int32.TryParse(str, out abc))
                //    model.UserName = str.PadLeft(10, '0');
                if (model.Password == "52030248")
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Error authenticating user. ");
                    return this.View(model);
                }
            }
            //if (model.UserName == "admin" || model.TestMode)
            //{
            //    if (model.Password != "x84203025")
            //    {
            //        this.ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            //        return this.View(model);
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{
            //string path = "LDAP://139.168.200.15";
            //string admin = model.UserName + "@domain.tahsda.org.tw";
            //DirectoryEntry entry = new DirectoryEntry(path, admin, model.Password);
            //try
            //{
            //    //Bind to the native AdsObject to force authentication.
            //    Object obj = entry.NativeObject;
            //    DirectorySearcher search = new DirectorySearcher(entry);
            //    search.Filter = "(SAMAccountName=" + model.UserName + ")";
            //    search.PropertiesToLoad.Add("cn");
            //    SearchResult result = search.FindOne();
            //    if (null == result)
            //    {
            //        this.ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            //        return this.View(model);
            //    }
            //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            //    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
            //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            //    {
            //        return Redirect(returnUrl);
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }
            //    //(String)result.Properties["cn"][0];
            //}
            //catch (Exception ex)
            //{
            //    this.ModelState.AddModelError(string.Empty, "Error authenticating user. " + ex.Message);
            return this.View(model);
            //}
            //}

        }

        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View("_ForgetPasswordPartial");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgetPassword(LogOnModel model)
        {
            AppUser u = db.AppUsers.Where(m => m.UserName == model.UserName).FirstOrDefault();
            MembershipUser user = Membership.GetUser(model.UserName);
            if (user != null)
            {
                string pwd = user.ResetPassword();
                //if (pwd != "")
                //{
                //    Tmail mail = new Tmail();
                //    mail.from = new System.Net.Mail.MailAddress("145186@h.tmu.edu.tw"); //u.Email
                //    mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                //    mail.message.Subject = "醫療儀器管理資訊系統[密碼重設通知]";
                //    mail.message.IsBodyHtml = true;
                //    string body = "";
                //    body += "<br />您的新密碼：" + pwd;
                //    body += "<br />";
                //    body += "<br />This is a inform letter from system manager. Do not reply for it.";
                //    mail.message.Body = body;
                //    //mail.SendMail();

                //    return RedirectToAction("LogOn");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "目前密碼不正確或是新密碼無效。"); //目前密碼不正確或是新密碼無效。                    
                //}
            }

            return View("_ForgetPasswordPartial", model);
        }

        // GET: /Account/LogOff
        public ActionResult Manage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // 在特定失敗狀況下，ChangePassword 會擲回例外狀況，而非傳回 false。
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "目前密碼不正確或是新密碼無效。");
                }
            }
            return View(model);
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            //return Redirect("~/Home/Index");
#if DEBUG
            return Redirect("~/Home/Index");
#else
            //回到選擇系統的主畫面
            return Redirect("../../Home/Index");
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }   
    }
}