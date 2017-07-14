using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using Fgo.API.Hubs;
using Fgo.API.Infractstructure;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fgo.API.Controllers
{
    [RoutePrefix("api/Message")]
    public class MessageController : ApiController
    {

        #region Properties
        private readonly IBizlunchService _Service;
        private static readonly log4net.ILog logger =
     LogManager.GetLogger(typeof(MessageController));

        #endregion
        #region Constructors

        public MessageController(IBizlunchService Service)
        {
            //  this.Log = log;
            this._Service = Service;
        }

        #endregion
        #region methods

        [Route("GetMessageChat")]
        [HttpPost]
        public IHttpActionResult  GetMessageChat(MessageRow request)
        {
            try
            {
            
                return Ok(_Service.GetMessageChat(request));
            }
            catch (Exception ex)
            {
                logger.Error("Exception is {0}", ex);
                return null;
            }

        }

        #endregion
    }
}
