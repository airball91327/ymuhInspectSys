using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    [Authorize]
    public class DEInspectDocFlowController : Controller
    {
        // GET: DEInspectDocFlow
        public ActionResult Index()
        {
            return View();
        }
    }
}