using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using Bizkasa.Bizlunch.Business.Utils;
using Fgo.API.Infractstructure;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace Fgo.API.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
    
   
        #region Properties
        private readonly IBizlunchService _Service;
        private static readonly log4net.ILog logger =
     LogManager.GetLogger(typeof(AccountController));

        #endregion
        #region Constructors

        public AccountController(IBizlunchService Service)
        {
            //  this.Log = log;
            this._Service = Service;
        }

        #endregion


        [Route("Login")]
        [HttpPost]
        public IHttpActionResult RequestLogin(LoginDTO request)
        {
            try
            {
                request.AppCode = (int)AppCode.Mobile;
                var result = _Service.Login(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("Exception is {0}", ex);
                return null;
            }
            
        }

        [Route("GetRestaurants")]
        [HttpPost]
        public IHttpActionResult GetRestaurants(SearchDTO request)
        {
          
             var result = _Service.GetRestaurants(request);
            return Ok(result.ToJsonResult(result.Data));
        }

        [Route("GetFriends")]
        [HttpPost]
        public IHttpActionResult GetFriends(SearchDTO request)
        {

            var result = _Service.GetFriends(request);
            return Ok(result.ToJsonResult(result.Data));
        }

        [Route("AddOrUpdateFriend")]
        [HttpPost]
        public IHttpActionResult AddOrUpdateFriend(AccountDTO request)
        {
            try
            {
                var result = _Service.AddOrUpdateFriend(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("AddOrUpdateFriend Exception is {0}", ex);
                return null;
            }

        }

        [Route("SendOneEmailInvite")]
        [HttpPost]
        public IHttpActionResult SendOneEmailInvite(InviteEmailDTO request)
        {
            try
            {
                var result = _Service.SendOneEmailInvite(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("AddOrUpdateFriend Exception is {0}", ex);
                return null;
            }

        }

        [Route("AddTemplateEmail")]
        [HttpPost]
        public IHttpActionResult AddTemplateEmail(TemplateEmailDTO request)
        {
            try
            {
               
                var result = _Service.AddTemplateEmail(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("AddOrUpdateFriend Exception is {0}", ex);
                return null;
            }

        }

        [Route("SyncFriends")]
        [HttpPost]
        public IHttpActionResult SyncFriends(InviteMoreFriendDTO request)
        {
            try
            {
                var result = _Service.SyncFriends(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("SyncFriends Exception is {0}", ex);
                return null;
            }

        }

        [Route("SignUp")]
        [HttpPost]
        public IHttpActionResult SignUp(SignUpDTO request)
        {
            try
            {
                logger.InfoFormat("data SignUp controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
                var result = _Service.SignUp(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("SignUp Exception is {0}", ex);
                return null;
            }

        }
    }
}
