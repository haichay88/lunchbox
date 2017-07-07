using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{
   public static class TemplateHelper
    {
        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <returns></returns>
        public static string Render(string body, Dictionary<string, string> modelProperties)
        {
            return modelProperties.Aggregate(body, (current, modelProperty) => current.Replace("%" + modelProperty.Key + "%", modelProperty.Value));
        }
    }
}
