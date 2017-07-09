using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
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
        public IHttpActionResult GetOrders(SearchDTO request)
        {
            try
            {
                var result = _Service.GetOrders(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("GetOrders Exception is {0}", ex);
                return null;
            }

        }

        [Route("GetPlaces")]
        [HttpPost]
        public IHttpActionResult GetPlaces(SearchDTO request)
        {
            try
            {
                var result = _Service.GetRestaurants(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("GetPlaces Exception is {0}", ex);
                return null;
            }

        }
        [Route("GetOrder")]
        [HttpPost]
        public IHttpActionResult GetOrder(SearchDTO request)
        {
            try
            {
                var result = _Service.GetOrderBy(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("GetOrder Exception is {0}", ex);
                return null;
            }

        }

        [Route("AddOrUpdatePlace")]
        [HttpPost]
        public IHttpActionResult AddOrUpdatePlace(RestaurantDTO request)
        {
            try
            {
                var result = _Service.AddOrUpdateRestaurant(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("AddOrUpdatePlace Exception is {0}", ex);
                return null;
            }

        }

        [Route("AddInvite")]
        [HttpPost]
        public IHttpActionResult AddInvite(InviteDTO request)
        {
            logger.InfoFormat("data AddInvite controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
            try
            {
                var result = _Service.AddInvite(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("AddInvite Exception is {0}", ex);
                return null;
            }

        }

        [Route("AddMoreFriend")]
        [HttpPost]
        public IHttpActionResult AddMoreFriend(InviteMoreFriendDTO request)
        {
            logger.InfoFormat("data AddMoreFriend controller is {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request, Newtonsoft.Json.Formatting.Indented));
            try
            {
                var result = _Service.AddMoreFriend(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("AddMoreFriend Exception is {0}", ex);
                return null;
            }

        }

        [Route("AddOrderDetail")]
        [HttpPost]
        public IHttpActionResult AddOrderDetail(OrderDTO request)
        {            
            try
            {
                var result = _Service.AddOrderDetail(request);
                return Ok(result.ToJsonResult(result.Data));
            }
            catch (Exception ex)
            {
                logger.Error("AddOrderDetail Exception is {0}", ex);
                return null;
            }

        }
    }
}
