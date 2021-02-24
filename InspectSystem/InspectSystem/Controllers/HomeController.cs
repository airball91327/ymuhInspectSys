using InspectSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            UnsignCountsVModel v = new UnsignCountsVModel();

            var insCount = db.InspectDocFlow.Where(df => df.FlowStatusId == "?")
                                            .Where(df => df.UserId == WebSecurity.CurrentUserId).Count();
            v.InspectCount = insCount;
            var DEinsCount = db.DEInspectDocFlow.Where(df => df.FlowStatusId == "?" || df.FlowStatusId == "0")
                                                .Where(df => df.UserId == WebSecurity.CurrentUserId).Count();
            v.DEInspectCount = DEinsCount;

            return View(v);
        }
    }
}