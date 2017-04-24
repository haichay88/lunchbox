using Bizkasa.Bizlunch.Business.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bizkasa.Bizlunch.Presentation.MVC.Infractstructure
{
    public static class UrlHelper
    {
        public static string GetHomeUrl(this Controller controller)
        {

            var principal = controller.HttpContext.User;
         
            if (principal != null && principal.Identity.IsAuthenticated)
            {
               
             

                if (WorkContext.UserContext == null)
                {
                    return controller.Url.Action("Relogin","Home");
                   // return controller.Url.Action("Login", "Home", null, controller.Request.Url.Scheme);
                }
                else
                {
                    if (WorkContext.UserContext.IsAdmin)
                        return controller.Url.Action("Activity", "Contract");
                   
                    return controller.Url.Action("Index", "Home");
                }

            }
            return controller.Url.Action("Login","Home");
            //return controller.Url.Action("Login", "Home", null, controller.Request.Url.Scheme);
        }
    }
}