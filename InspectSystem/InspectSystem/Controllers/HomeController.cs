using InspectSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            UnsignCountsVModel v = new UnsignCountsVModel();
            v.VentilatorCount = 0;
            v.InspectCount = 0;

            return View(v);
        }
    }
}