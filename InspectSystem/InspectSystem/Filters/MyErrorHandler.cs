using InspectSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace InspectSystem.Filters
{
    public class MyErrorHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private BMEDcontext db = new BMEDcontext();
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = new JsonResult
            {
                Data = new { success = false, error = filterContext.Exception.Message },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}