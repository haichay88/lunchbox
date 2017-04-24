using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizkasa.Bizlunch.Business.Base
{
    public abstract class BusinessBase : IBusinessBase
    {
        private BusinessProcess businessProcess = BusinessProcess.Current;

        protected void AddError(object obj)
        {
            businessProcess.AddError(this.GetType(), obj);
        }

        protected void AddInfo(object obj)
        {
            businessProcess.AddInfo(this.GetType(), obj);
        }

        protected List<object> GetInfos()
        {
            return businessProcess.GetInfos();
        }

        protected bool HasError
        {
            get
            {
                return businessProcess.HasError;
            }
        }
        /// <summary>
        /// Chỉ gọi khi thực sự cần
        /// </summary>
        protected void ClearError()
        {
            businessProcess.ClearError();
        }
    }
}
