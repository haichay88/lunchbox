
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;

namespace Bizkasa.Bizlunch.Presentation.MVC.Infractstructure
{
    public class ExceptionHandler : IExceptionFilter
    {
        private static readonly ILog logger =
         LogManager.GetLogger(typeof(ExceptionHandler));
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                logger.Error(filterContext.Exception);

                if (filterContext.HttpContext.Request.IsAuthenticated)
                {
                   HttpContext.Current.Response.Redirect("~/Home/Relogin");
                   return;
                }

                if (!HttpContext.Current.IsDebuggingEnabled)
                {
                    if (filterContext.Exception is System.Web.HttpRequestValidationException)
                    {
                        HttpContext.Current.Response.Redirect("~/Home/Error?code=9000");
                        return;
                    }
                }

                HttpContext.Current.Response.Redirect("~/Home/Login");
            }
        }
    }
}