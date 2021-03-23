using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspectSystem.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 依參數決定列印的報表。
        /// </summary>
        /// <param name="reportType">列印的報表種類</param>
        /// <returns></returns>
        // GET: Report/PrintReport
        public ActionResult PrintReport(string reportType)
        {
            if (reportType == "DaliyReport")
            {
                return View("DaliyReport");
            }
            else if (reportType == "MonthReport")
            {
                return View("MonthReport");
            }
            else
            {
                return View();
            }
        }
    }
}