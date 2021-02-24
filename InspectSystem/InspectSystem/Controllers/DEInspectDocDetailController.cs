using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class DEInspectDocDetailController : Controller
    {
        // GET: DEInspectDocDetail
        public ActionResult Index()
        {
            return View();
        }
    }
}