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
            try
            {
                request.AppCode = (int)AppCode.Mobile;
                var result = _Service.Login(request);
  
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("Exception is {0}", ex);
                return null;
            }
            
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
