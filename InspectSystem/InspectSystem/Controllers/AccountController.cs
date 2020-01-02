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

namespace InspectSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //將顯示用訊息從 TempData 取出
            var errmsg = TempData["Timeout"] as string;
            if (!string.IsNullOrWhiteSpace(errmsg))
            {
                ViewBag.Timeout = errmsg;//將顯示訊息放入 ViewBag 供 view 使用
            }

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Clear the existing external cookie to ensure a clean login process
                FormsAuthentication.SignOut();

                HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
                cookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Response.Cookies.Add(cookie);
            }
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AntiForgeryErrorHandler(ExceptionType = typeof(HttpAntiForgeryException), View = "Login", Controller = "Account", ErrorMessage = "連線預時，回到登入畫面")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                //string url = "WebApi/Accounts/CheckPasswd?id=" + model.UserName;
                string url = "WebApi/Accounts/CheckPasswdForCch?id=" + model.UserName;
                url += "&pwd=" + HttpUtility.UrlEncode(model.Password, Encoding.GetEncoding("UTF-8"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(url);
                string rstr = "";
                if (response.IsSuccessStatusCode)
                {
                    rstr = await response.Content.ReadAsStringAsync();
                }

                /* If the UserName and the Password are legal. */
                if (rstr.Contains("成功"))
                {
                    // Get user role
                    HttpClient clientRole = new HttpClient();
                    clientRole.BaseAddress = new Uri("http://dms.cch.org.tw:8080/");
                    string urlRole = "WebApi/Accounts/GetRoles?id=" + model.UserName;
                    clientRole.DefaultRequestHeaders.Accept.Clear();
                    clientRole.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responseRole = await clientRole.GetAsync(urlRole);
                    string rstrRole = "";
                    if (responseRole.IsSuccessStatusCode)
                    {
                        rstrRole = await responseRole.Content.ReadAsStringAsync();
                    }

                    // Get user real name
                    char[] charSpilt = new char[] { ',', '{', '}', '[', ']', '"', ':', '\\', '!', ';' };
                    string[] rstrSpilt = rstr.Split(charSpilt, StringSplitOptions.RemoveEmptyEntries);
                    string userRealName = rstrSpilt[1].ToString();

                    // Set user role and real name to userData.
                    string userData = rstrRole + userRealName;

                    // Set authentication cookie with userName, and userData(real name and user role)
                    //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var authTicket = new FormsAuthenticationTicket(
                    1,                                              // version
                    model.UserName,                                 // user name
                    DateTime.UtcNow.AddHours(08),                   // created
                    DateTime.UtcNow.AddHours(08).AddMinutes(480),   // expires
                    true,                                           // persistent 
                    userData,                                       // can be used to store roles
                    FormsAuthentication.FormsCookiePath
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    // Create the cookie.
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);                  
                    Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (rstr.Contains("密碼錯誤"))
                    {
                        ModelState.AddModelError(string.Empty, "密碼錯誤.");
                        return View(model);
                    }
                    else if (rstr.Contains("無此員工"))
                    {
                        ModelState.AddModelError(string.Empty, "無此帳號.");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "登入無效.");
                        return View(model);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            //清除所有 session
            Session.RemoveAll();

            HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Response.Cookies.Add(cookie);

            return RedirectToAction("Login", "Account");
        }


        public class AntiForgeryErrorHandlerAttribute : HandleErrorAttribute
        {
            //用來指定 redirect 的目標 controller
            public string Controller { get; set; }
            //用來儲存想要顯示的訊息
            public string ErrorMessage { get; set; }
            //覆寫預設發生 exception 時的動作
            public override void OnException(ExceptionContext filterContext)
            {
                //如果發生的 exception 是 HttpAntiForgeryException 就轉導至設定的 controller、action (action 在 base HandleErrorAttribute已宣告)
                if (filterContext.Exception is HttpAntiForgeryException)
                {
                    //這個屬性要設定為 true 才能接手處理 exception 也才可以 redirect
                    filterContext.ExceptionHandled = true;
                    //將 errormsg 使用 TempData 暫存 (ViewData 與 ViewBag 因為生命週期的關係都無法正確傳遞)
                    filterContext.Controller.TempData.Add("Timeout", ErrorMessage);
                    //指定 redirect 的 controller 偶 action
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "action", View },
                            { "controller", Controller},
                        });
                }
                else
                    base.OnException(filterContext);// exception 不是 HttpAntiForgeryException 就照 mvc 預設流程
            }
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