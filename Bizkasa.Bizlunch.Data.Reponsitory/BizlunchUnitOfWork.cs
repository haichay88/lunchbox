using Bizkasa.Bizlunch.Data.Core;
using Bizkasa.Bizlunch.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Data.Reponsitory
{
    public interface IBizlunchUnitOfWork : IUnitOfWork
    {
    }
    public class BizlunchUnitOfWork : EFUnitOfWork, IBizlunchUnitOfWork, IUnitOfWork
    {
        #region Constructors

        public BizlunchUnitOfWork()
            : base(new BizlunchEntities())
        {

            //  DbInterception.Add(new FtsInterceptor());
            //  context.Configuration.LazyLoadingEnabled = false;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion

        #region Constants

        #endregion
    }
}
