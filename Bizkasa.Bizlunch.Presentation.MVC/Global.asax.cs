using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Utils;
using Bizkasa.Bizlunch.Presentation.MVC.Infractstructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Bizkasa.Bizlunch.Presentation.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityMvcActivator.Start();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[WorkContext.SessionKey];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                LoginResultDTO serializeModel = JsonConvert.DeserializeObject<LoginResultDTO>(authTicket.UserData);
                RicohPrincipal newUser = new RicohPrincipal(serializeModel.Email);
                if (serializeModel != null)
                {
                    newUser.UserId = serializeModel.Id;
                    //newUser.FirstName = serializeModel.;
                    newUser.FirstName = serializeModel.Email;
                    //newUser.roles = serializeModel.;

                    HttpContext.Current.User = newUser;
                }


            }
        }
    }
}
