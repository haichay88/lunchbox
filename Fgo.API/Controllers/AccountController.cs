using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using Bizkasa.Bizlunch.Business.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            logger.InfoFormat("data controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
            request.AppCode = (int)AppCode.Mobile;
            request.Password = CommonUtil.CreateMD5(request.Password);
            var result = _Service.Login(request);
            //if(!result.HasError)
            //    result.Data.Token = EncryptDecryptUtility.Encrypt(result.Data.Token,true);
            return Ok(result);
        }

        [Route("GetRestaurants")]
        [HttpPost]
        public IHttpActionResult GetRestaurants(BaseRequest request)
        {
            logger.InfoFormat("data controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
          
             var result = _Service.GetRestaurants(request);
           
            return Ok(result);
        }
    }
}
