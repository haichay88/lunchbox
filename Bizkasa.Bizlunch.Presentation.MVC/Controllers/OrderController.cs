using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using Bizkasa.Bizlunch.Presentation.MVC.Infractstructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bizkasa.Bizlunch.Presentation.MVC.Controllers
{
    public class OrderController : Controller
    {

        #region Properties
        private readonly IBizlunchService _Service;
        #endregion


        #region Contructors

        public OrderController(IBizlunchService service)
        {
            _Service = service;
        }
        #endregion
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(int Id)
        {
            ViewData["OrderId"] = Id;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddOrUpdateOrder(OrderDTO dto)
        {
            var result = _Service.AddOrUpdateOrder(dto);
            return result.ToJsonResult(result.Data);
        }


        [HttpPost]
        public JsonResult AddOrderDetail(OrderDTO dto)
        {
            var result = _Service.AddOrderDetail(dto);
            return result.ToJsonResult(result.Data);
        }

        [HttpGet]
        public JsonResult GetOrders()
        {
            BaseRequest request = new BaseRequest();
            var result = _Service.GetOrders(request);
            return result.ToJsonResult(result.Data);
        }
        [HttpPost]
        public JsonResult GetOrder(int orderId)
        {
            var result = _Service.GetOrderBy(orderId);
            return result.ToJsonResult(result.Data);
        }
    }
}