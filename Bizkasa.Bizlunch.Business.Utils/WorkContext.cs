using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public static class WorkContext
    {
        public const string SessionKey = "FGO";
        public static void SetInSession(object value)
        {
            HttpContext.Current.Session[SessionKey] = value;
        }
        public static T GetSession<T>()
        {
            return (T)HttpContext.Current.Session[SessionKey];
        }

        public static UserContext UserContext
        {
            get
            {
                if (HttpContext.Current.Session != null)
                    return HttpContext.Current.Session[SessionKey] as UserContext;
                else
                    return null;
            }
            set
            {
                if (HttpContext.Current.Session != null)
                    HttpContext.Current.Session[SessionKey] = value;
            }
        }
    }
    [Serializable]
    public class UserContext
    {
        public int UserId { get; set; }
        public int TechnicalId { get; set; }
        public int? WarehouseId { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool IsAdmin{ get; set; }
        public List<Scope> Scopes { get; set; }
        public int UserType { get; set; }
        public bool IsLogined { get { return this.UserId > 0; } }
        public DateTime? LastDateLogin { get; set; }
    }

    public class Scope {
        public string ScopeName { get; set; }
        public string[] Action { get; set; }
    }
}
