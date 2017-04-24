using LacViet.HPS.Ricoh.Business.IServices;
using LacViet.HPS.Ricoh.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Http.Filters;
using Bizkasa.Bizlunch.Business.Utils;

namespace Bizkasa.Bizlunch.Presentation.MVC.Infractstructure
{
     
    public class RicohAuthorize : AuthorizeAttribute
    {
        private string Permission;
        private string ItemType;
        public RicohAuthorize( string itemType,string _Permission)
        {
            this.Permission = _Permission;
            this.ItemType = itemType;
        }
        public static bool IsAjaxRequest()
        {
            var request = HttpContext.Current.Request;
            return ((request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest")));
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
         
            if (filterContext.Result == null)
            {
                if (WorkContext.UserContext == null)
                {                    
                    filterContext.HttpContext.Response.StatusCode =(int)HttpStatusCode.RequestTimeout;
                    return;
                }
                if (WorkContext.UserContext.UserId >0)
                {
                    if (WorkContext.UserContext.IsAdmin)
                        return;

                    Scope checkItemtype = WorkContext.UserContext.Scopes.Where(a => a.ScopeName == ItemType).FirstOrDefault();
                    if (checkItemtype == null)
                    {
                        if (!IsAjaxRequest())
                        {
                          filterContext.HttpContext.Response.StatusCode =(int)HttpStatusCode.Unauthorized;
                            return;
                        }
                        filterContext.Result = new RedirectResult("~/Home/UnAuthorized");
                        return;
                    }
                    else
                    {
                        bool m_per = checkItemtype.Action.Contains(Permission);
                        if (!m_per)
                        {
                            if (!IsAjaxRequest())
                            {
                                filterContext.HttpContext.Response.StatusCode =(int)HttpStatusCode.Unauthorized;
                                return;
                            }
                            filterContext.Result = new RedirectResult("~/Home/UnAuthorized");
                            return;
                        }
                    }
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode =(int)HttpStatusCode.RequestTimeout;
                    return;
                }
            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;

                return;
            }
            
        }

      
    }

  

    public class AuthorizationRequiredAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            var provider = filterContext.ControllerContext.Configuration
                .DependencyResolver.GetService(typeof(IBizkasaService)) as IRicohService;

            if (filterContext.Request.Headers.Contains(Token))
            {
                var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                // Validate Token
                if (provider != null && !provider.ValidateToken(tokenValue).Data)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
                    filterContext.Response = responseMessage;
                }
            }
            else
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            base.OnActionExecuting(filterContext);

        }
    }
}