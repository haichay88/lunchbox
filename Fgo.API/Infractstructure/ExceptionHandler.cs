using Bizkasa.Bizlunch.Business.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Fgo.API.Infractstructure
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

                return;
            }
        }
    }
}