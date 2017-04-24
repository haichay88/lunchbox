using Bizkasa.Bizlunch.Business.Model;
using Bizkasa.Bizlunch.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bizkasa.Bizlunch.Presentation.MVC.Infractstructure;
using Bizkasa.Bizlunch.Business.Utils;

namespace Bizkasa.Bizlunch.Presentation.MVC.Controllers
{
    public class UserController : Controller
    {
        #region Properties
        private readonly IBizlunchService _Service;
        #endregion


        #region Contructors

        public UserController(IBizlunchService service)
        {
            _Service = service;
        }
        #endregion
        #region ActionView

      
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Confirm(string token)
        {
            var result = _Service.GetUser(token);
            return View(result.Data);
        }

        [HttpPost]
        public ActionResult Confirm(AccountDTO dto)
        {
            if (ModelState.IsValid)
            {
                dto.Password = CommonUtil.CreateMD5(dto.Password);
                var result = _Service.ConfirmedUser(dto);
                return RedirectToAction("Index", "User");
            }
            return View(dto);
            
        }
        #endregion

        #region Ajax

        [HttpGet]
        public JsonResult GetUsers()
        {
            var result = _Service.GetUsers();
            return result.ToJsonResult(result.Data);
        }
        [HttpPost]
        public JsonResult AddOrUpdateAccount(AccountDTO model)
        {
            var result = _Service.AddOrUpdateAccount(model);
            return result.ToJsonResult(result.Data);
        }


        
        #endregion


        public ActionResult GetUser(int Id)
        {
            var result = _Service.GetUser(Id);
            if (result.HasError)
            {
                AccountDTO dto = new AccountDTO();
                ModelState.AddModelError("error", result.ToErrorMsg());
                return PartialView("_AddUser", dto);
            }
            return PartialView("_AddUser", result.Data);

        }

       

    }
}