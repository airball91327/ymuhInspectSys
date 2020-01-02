using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using InspectSystem.Models;
using Microsoft.Reporting.WebForms;

namespace InspectSystem.Controllers
{
    public class InspectReportsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: InspectReports
        public ActionResult Index()
        {
            var reportSource = db.InspectDocDetails.Where(i => i.ClassID == 6);

            //建立ReportViewer物件
            var reportViewer = new ReportViewer()
            {
                //設定處理模式
                ProcessingMode = ProcessingMode.Local,
                SizeToReportContent = true,
                Width = Unit.Percentage(100),
                Height = Unit.Percentage(100),
            };

            reportViewer.LocalReport.ReportPath = $"{Request.MapPath(Request.ApplicationPath)}Report\\Rdlc\\WaterSystemReport_M.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet_WaterSystem", reportSource.ToList()));
            return View(reportViewer);
        }
    }
}