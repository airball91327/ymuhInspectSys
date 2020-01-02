using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Optimization;
using System.Web.Routing;

namespace InspectSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                // 先取得該使用者的 FormsIdentity
                FormsIdentity id = (FormsIdentity)User.Identity;
                // 再取出使用者的 FormsAuthenticationTicket
                FormsAuthenticationTicket ticket = id.Ticket;
                // 將儲存在 FormsAuthenticationTicket 中的角色定義取出，並轉成字串陣列
                char[] charSpilt = new char[] { ',', '{', '}', '[', ']', '"', ':', '\\' };
                string[] roles = ticket.UserData.Split(charSpilt, StringSplitOptions.RemoveEmptyEntries);
                int count = 0, j = 0;
                string[] userRoles = new string[10];
                foreach(var value in roles)
                {
                    if(value == "roleName")
                    {
                        userRoles[j] = roles[count + 1];
                        j++;
                    }
                    count++;
                }
                // 指派角色到目前這個 HttpContext 的 User 物件去

                // For System Role Test ---> Change Admin to usual
                //if (userRoles[0] == "Admin")
                //{
                //    userRoles[0] = "Usual";
                //}

                Context.User = new GenericPrincipal(Context.User.Identity, userRoles);
            }
        }
    }
}
