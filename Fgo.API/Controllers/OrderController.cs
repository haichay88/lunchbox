using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fgo.API.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {

        #region Properties
        private readonly IBizlunchService _Service;
        private static readonly log4net.ILog logger =
     LogManager.GetLogger(typeof(OrderController));

        #endregion
        #region Constructors

        public OrderController(IBizlunchService Service)
        {
            //  this.Log = log;
            this._Service = Service;
        }

        #endregion


        [Route("GetOrders")]
        [HttpPost]
        public IHttpActionResult GetOrders(BaseRequest request)
        {
            try
            {
                var result = _Service.GetOrders(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("GetOrders Exception is {0}", ex);
                return null;
            }

        }
    }
}
