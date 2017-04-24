using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Data.Core
{
    /// <summary>
    /// Generic interface Repository pattern
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class
    {
        IList<TEntity> GetAll();

        IList<TEntity> GetAll(out int total, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> selectFields = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? pageIndex = null, int? pageSize = null);

        IList<TEntity> GetAll(out int total, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? pageIndex = null, int? pageSize = null);

        IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null);

        IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);

        IList<TEntity> GetAll(out int total,
            string filter = null, string orderBy = null, string includeFields = null,
            int? pageIndex = null, int? pageSize = null);

        IList<TEntity> GetAll(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> field, string searchTerm);

        IList<TResult> GetByStore<TResult>(string storeName, params object[] parameters);
        TResult ExecuteStore<TResult>(string storeName, params object[] parameters);

        TEntity GetById(object entityId);

        IQueryable<TEntity> GetQueryable();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        TEntity Get(Expression<Func<TEntity, bool>> where);
    }
}
