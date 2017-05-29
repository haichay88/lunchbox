using Fgo.API.Infractstructure;
using System.Web;
using System.Web.Mvc;

namespace Fgo.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionHandler());
        }
    }
}
