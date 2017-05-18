using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using Bizkasa.Bizlunch.Business.Utils;
using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Bizkasa.Bizlunch.Presentation.MVC.Controllers
{
    public class HomeController : Controller
    {

        #region Properties
        private readonly IBizlunchService _Service;
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        #endregion


        #region Contructors

        public HomeController(IBizlunchService service)
        {
            _Service = service;
        }
        #endregion

        #region Method

        #region View

        public ActionResult Logout(bool clientLogout = false, string returnUrl = null)
        {
            // mở khóa cho các user khác
           // _Service.Logout();

            HttpContext.Session.Clear();
            HttpCookie myCookie = new HttpCookie(WorkContext.SessionKey);
            myCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(myCookie);
            //SendoSession.Current.Dispose();
            FormsAuthentication.SignOut();
            WorkContext.UserContext = null;
            string absUrl = returnUrl;

            if (absUrl == null)
                absUrl = Url.Action("Index", "Home", null, Request.Url.Scheme);
            return Redirect(absUrl);

        }
        public ActionResult Index()
        {
            var principal = HttpContext.User;
            if (principal.Identity.IsAuthenticated)
            {

                string email = principal.Identity.Name;
                var result = _Service.Relogin(email);
                if (result.Data == null)
                {
                    FormsAuthentication.SignOut();
                }
                else
                {
                    string userData = JsonConvert.SerializeObject(result.Data);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(0, result.Data.Email, DateTime.Now, DateTime.Now.AddDays(30), true, userData, FormsAuthentication.FormsCookiePath);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    faCookie.Expires = DateTime.Now.AddDays(30);
                    faCookie.Path = "/";
                    Response.Cookies.Add(faCookie);

                    UserContext ctx = new UserContext()
                    {
                        UserName = result.Data.Email,
                        UserId = result.Data.Id,
                        FullName = result.Data.LastName + result.Data.FirstName,
                       
                    };
                    WorkContext.UserContext = ctx;
                }
                return View();

            }
            else
            {
                return View();
            }
        }
        public ActionResult Friends()
        {
            var fb = new FacebookClient();
            //dynamic result = fb.Post("oauth/access_token", new
            //{
            //    client_id = "141552049572357",
            //    client_secret = "8b1d44838d41898e2ca1cc752f0bfc2d",
            //    redirect_uri = RedirectUri.AbsoluteUri,
            //    code = code
            //});

           // var accessToken = result.access_token;

            // Store the access token in the session for farther use
         var accessToken=  (string) Session["AccessToken"] ;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get friends
            dynamic friendListData = fb.Get("/me/friends");
            var result = (from i in (IEnumerable<dynamic>)friendListData.data
                          select new
                          {
                              i.name,
                              i.id
                          }).ToList();
            return View("_Friends");
        }
        public ActionResult Login()
        {
            LoginDTO model = new LoginDTO();
            return View(model);
        }

            
           
       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public ActionResult Register()
        {
            AccountDTO dto = new AccountDTO();
            return View(dto);
        }
       

        #endregion

        #region Post

        [HttpPost]
        public ActionResult Login(LoginDTO dto)
        {
            if (ModelState.IsValid)
            {
                dto.Password = CommonUtil.CreateMD5(dto.Password);
                var result = _Service.Login(dto);
                if (result.HasError)
                {
                    ModelState.AddModelError("error", result.ToErrorMsg());
                    return View(dto);
                }
                if (result.Data == null)
                {
                    ModelState.AddModelError("error", result.ToErrorMsg());
                    return View(dto);

                }


                string userData = JsonConvert.SerializeObject(result.Data);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(0, result.Data.Email, DateTime.Now, DateTime.Now.AddDays(30), true, userData, FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie faCookie = new HttpCookie(WorkContext.SessionKey, encTicket);
                faCookie.Expires = DateTime.Now.AddDays(30);
                faCookie.Path = "/";
                Response.Cookies.Add(faCookie);

                UserContext ctx = new UserContext()
                {
                    UserName = result.Data.Email,
                    UserId = result.Data.Id,
                    FullName = result.Data.FirstName

                };
                WorkContext.UserContext = ctx;
                return RedirectToAction("index");
            }
            return View(dto);
        }
        [HttpPost]
        public ActionResult Register(AccountDTO dto)
        {
            if (ModelState.IsValid)
            {
                dto.Password = CommonUtil.CreateMD5(dto.Password);
                var result = _Service.RegisterAccount(dto);
                if (result.HasError)
                    ModelState.AddModelError("error", result.ToErrorMsg());
            }
            return View(dto);
        }


        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "1073352086130252",
                client_secret = "ade6cf1089341c1720f225796d47f36e",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "user_friends" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }


        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "1073352086130252",
                client_secret = "ade6cf1089341c1720f225796d47f36e",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // Store the access token in the session for farther use
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
           
            string email = me.email;
            string firstname = me.first_name;
            string middlename = me.middle_name;
            string lastname = me.last_name;

            dynamic friendListData = fb.Get("/me/friends");
            var a = (from i in (IEnumerable<dynamic>)friendListData.data
                          select new
                          {
                              i.name,
                              i.id
                          }).ToList();


            // add user to DB
            AccountDTO row = new AccountDTO() {
                Email= me.email,
                FirstName = me.first_name,
                LastName= me.last_name+ " "+ me.middle_name,
                SourceId= me.id,
                
            };
        
            // login with new user
            var user= _Service.Relogin(row);
           
            // Set the auth cookie
            //FormsAuthentication.SetAuthCookie(email, false);

            string userData = JsonConvert.SerializeObject(user.Data);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(0, user.Data.Email, DateTime.Now, DateTime.Now.AddDays(30), true, userData, FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(WorkContext.SessionKey, encTicket);
            faCookie.Expires = DateTime.Now.AddDays(30);
            faCookie.Path = "/";
            Response.Cookies.Add(faCookie);

            UserContext ctx = new UserContext()
            {
                UserName = user.Data.Email,
                UserId = user.Data.Id,
                FullName = user.Data.LastName + " " + user.Data.FirstName,

            };
            WorkContext.UserContext = ctx;



            return RedirectToAction("Index", "Home");
        }
        #endregion

        #endregion

    }
}