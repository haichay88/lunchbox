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


namespace Bizkasa.Bizlunch.Presentation.MVC.Controllers
{
    [RoutePrefix("api/Account")]
    public class ApiAccountController : ApiController
    {
        #region Properties
        private readonly IBizlunchService _Service;
        private static readonly log4net.ILog logger =
     LogManager.GetLogger(typeof(ApiAccountController));

        #endregion
        #region Constructors

        public ApiAccountController(IBizlunchService Service)
        {
            //  this.Log = log;
            this._Service = Service;
        }

        #endregion



        [Route("login")]
        [HttpPost]
        public IHttpActionResult RequestLogin(LoginDTO request)
        {
            return Ok(Login(request));
        }
        public Response Login(LoginDTO request)
        {
            logger.InfoFormat("data controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
            request.AppCode = (int)AppCode.Mobile;
            request.Password = CommonUtil.CreateMD5(request.Password);
            var result = _Service.Login(request);
            return result;
        }
    }
}
