using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Extention
{
    public static class IoC
    {
        public static T Get<T>()
        {

            return ServiceLocator.Current.GetInstance<T>();

        }

        public static T Get<T>(string key)
        {
            return ServiceLocator.Current.GetInstance<T>(key);
        }


    }
}
