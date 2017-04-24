using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bizkasa.Bizlunch.Business.Extention
{
    public static class UnityServiceLocatorExtensions
    {
        public static T GetInstance<T>(this IServiceLocator locator, ParameterOverride para)
        {
            IUnityContainer container = locator.GetInstance<IUnityContainer>();
            return container.Resolve<T>(para);
        }
        public static T GetInstance<T>(this IServiceLocator locator, string name, ParameterOverride para)
        {
            IUnityContainer container = locator.GetInstance<IUnityContainer>();
            return container.Resolve<T>(name, para);
        }
    }
   
}
