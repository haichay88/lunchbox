using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bizkasa.Bizlunch.Business.Utils;
using Bizkasa.Bizlunch.Presentation.MVC.Infractstructure;

namespace Bizkasa.Bizlunch.Presentation.MVC.Controllers
{
    public class RestaurantController : Controller
    {
        #region Properties
        private readonly IBizlunchService _Service;
        #endregion


        #region Contructors

        public RestaurantController(IBizlunchService service)
        {
            _Service = service;
        }
        #endregion
        // GET: Restaurant
        public ActionResult Index()
        {
            BaseRequest request = new BaseRequest() { };
            var result = _Service.GetRestaurants(request);
            return View(result.Data);
        }
        [HttpGet]
        public JsonResult GetRestaurants()
        {
            BaseRequest request = new BaseRequest() { };
            var result = _Service.GetRestaurants(request);
            return result.ToJsonResult(result.Data);
        }

        public ActionResult AddOrUpdateRestaurant()
        {
            RestaurantDTO dto = new RestaurantDTO();
            return PartialView("_AddRestaurant", dto);
        }

        public ActionResult GetRestaurant(int Id)
        {
            var result = _Service.GetRestaurant(Id);
            if (result.HasError)
            {
                RestaurantDTO dto = new RestaurantDTO();
                ModelState.AddModelError("error", result.ToErrorMsg());
                return PartialView("_AddRestaurant", dto);
            }
            return PartialView("_AddRestaurant", result.Data);

        }

        [HttpPost]
        public ActionResult AddOrUpdateRestaurant(RestaurantDTO dto)
        {
            var result = _Service.AddOrUpdateRestaurant(dto);
            if (result.HasError)
            {
                ModelState.AddModelError("error", result.ToErrorMsg());
                return PartialView("_AddRestaurant", dto);
            }
            return RedirectToAction("Index");



        }
    }
}