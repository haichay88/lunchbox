
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bizkasa.Bizlunch.Business.Utils;

namespace Bizkasa.Bizlunch.Presentation.MVC.Infractstructure
{
    public class JsonCommonResult<T> : JsonCommonResult
    {
        public T Data { get; set; }
    }

    public class JsonCommonResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }

        public static JsonResult CreateError(string message)
        {
            JsonCommonResult result = new JsonCommonResult();
            if (message != null)
                result.IsError = true;
            else
                result.IsError = false;
            result.Message = message;
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
           
        }






    }
    public static class Utils
    {
        public static JsonResult ToJsonResult<T>(this Response<T> response, T withData)
        {
            JsonCommonResult<T> result = new JsonCommonResult<T>();
            result.Data = withData;
            result.IsError = response.HasError;
            result.Message = response.ToErrorMsg();
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

       

    }
}