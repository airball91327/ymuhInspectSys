using System.Web.Mvc;

namespace InspectSystem.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new { controller = "InspectDocChecker", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}