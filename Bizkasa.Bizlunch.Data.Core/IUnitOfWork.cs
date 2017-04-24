using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Data.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        TRepository GetCustomRepository<TRepository>() where TRepository : class;

        TContext GetContext<TContext>() where TContext : class;

        void BeginTransaction();
        void Rollback();
        void Commit();
    }
}
