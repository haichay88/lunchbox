using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Data.Core
{
    public class EFGenericRepository<TEntity> : IRepository<TEntity>
          where TEntity : class
    {
        #region Constructors

        public EFGenericRepository(DbContext context)
        {
            this.Context = context;

            if (this.Context != null)
                DbSet = this.Context.Set<TEntity>();
        }

        #endregion

        #region Properties

        private DbContext Context { get; set; }
        private DbSet<TEntity> DbSet { get; set; }

        #endregion

        #region Methods

        public IList<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public IList<TEntity> GetAll(out int total, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> selectFields = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? pageIndex = null, int? pageSize = null)
        {
            total = this.Select(filter).Count();
            return this.Select(filter, orderBy, selectFields, includes, pageIndex, pageSize).ToList();
        }

        public IList<TEntity> GetAll(out int total,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            return this.GetAll(out total, filter, orderBy, null, includes, pageIndex, pageSize);
        }

        public IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            return this.Select(filter, orderBy, includes, null, null).ToList();
        }

        public IList<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return this.Select(filter, null, null, null, null).ToList();
        }

        public IList<TEntity> GetAll(out int total,
            string filter = null, string orderBy = null, string includeFields = null,
            int? pageIndex = null, int? pageSize = null)
        {
            throw new NotImplementedException();
        }

        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> selectFields = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (selectFields != null)
            {
                foreach (var m_SelectField in selectFields)
                {
                    query.Select(m_SelectField);
                }
            }

            if (pageIndex != null && pageSize != null)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        public IList<TEntity> GetAll(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> field, string searchTerm)
        {
            IList<TEntity> m_Entities = null;

            IQueryable<TEntity> query = DbSet;

            m_Entities = query.ContainsSearch(field, searchTerm).Where(expression).ToList();

            return m_Entities;
        }
        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Where(where).FirstOrDefault<TEntity>();
        }
        public IList<TResult> GetByStore<TResult>(string storeName, params object[] parameters)
        {
            var m_Method = this.Context.GetType().GetMethod(storeName);
            var m_StoreResult = m_Method.Invoke(this.Context, parameters) as ObjectResult<TResult>;

            return m_StoreResult.ToList();
        }

        public TResult ExecuteStore<TResult>(string storeName, params object[] parameters)
        {
            var m_Method = this.Context.GetType().GetMethod(storeName);
            return (TResult)m_Method.Invoke(this.Context, parameters);
        }

        public TEntity GetById(object entityId)
        {
            return DbSet.Find(entityId);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return this.Context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            this.DbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            this.DbSet.Attach(entity);
            this.Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            if (this.Context.Entry(entity).State == EntityState.Detached)
                this.DbSet.Attach(entity);
            this.DbSet.Remove(entity);
        }

        #endregion

        #region Constants

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public static class LanguageExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T source, params T[] list)
        {
            return (list as IList<T>).Contains(source);
        }
    }
    public static class FullTextPrefixes
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ContainsPrefix = "-CONTAINS-";

        /// <summary>
        /// 
        /// </summary>
        public const string FreetextPrefix = "-FREETEXT-";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string Contains(string searchTerm)
        {
            return string.Format("({0}{1})", ContainsPrefix, searchTerm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string Freetext(string searchTerm)
        {
            return string.Format("({0}{1})", FreetextPrefix, searchTerm);
        }

    }
    public class FtsInterceptor : IDbCommandInterceptor
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            RewriteFullTextQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            RewriteFullTextQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public static void RewriteFullTextQuery(DbCommand cmd)
        {
            var text = cmd.CommandText;
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                var parameter = cmd.Parameters[i];
                if (
                    !parameter.DbType.In(DbType.String, DbType.AnsiString, DbType.StringFixedLength,
                        DbType.AnsiStringFixedLength)) continue;
                if (parameter.Value == DBNull.Value)
                    continue;
                var value = (string)parameter.Value;
                if (value.IndexOf(FullTextPrefixes.ContainsPrefix, StringComparison.Ordinal) >= 0)
                {
                    parameter.Size = 4096;
                    parameter.DbType = DbType.AnsiStringFixedLength;
                    value = value.Replace(FullTextPrefixes.ContainsPrefix, ""); // remove prefix we added n linq query
                    value = value.Substring(1, value.Length - 2); // remove %% escaping by linq translator from string.Contains to sql LIKE
                    parameter.Value = value;
                    cmd.CommandText = Regex.Replace(text,
                        string.Format(
                            @"\[(\w*)\].\[(\w*)\]\s*LIKE\s*@{0}\s?(?:ESCAPE N?'~')", parameter.ParameterName),
                        string.Format(@"CONTAINS([$1].[$2], @{0})", parameter.ParameterName));
                    if (text == cmd.CommandText)
                        throw new Exception("FTS was not replaced on: " + text);
                    text = cmd.CommandText;
                }
                else if (value.IndexOf(FullTextPrefixes.FreetextPrefix, StringComparison.Ordinal) >= 0)
                {
                    parameter.Size = 4096;
                    parameter.DbType = DbType.AnsiStringFixedLength;
                    value = value.Replace(FullTextPrefixes.FreetextPrefix, ""); // remove prefix we added n linq query
                    value = value.Substring(1, value.Length - 2); // remove %% escaping by linq translator from string.Contains to sql LIKE
                    parameter.Value = value;
                    cmd.CommandText = Regex.Replace(text,
                        string.Format(
                            @"\[(\w*)\].\[(\w*)\]\s*LIKE\s*@{0}\s?(?:ESCAPE N?'~')", parameter.ParameterName),
                        string.Format(@"FREETEXT([$1].[$2], @{0})", parameter.ParameterName));
                    if (text == cmd.CommandText)
                        throw new Exception("FTS was not replaced on: " + text);
                    text = cmd.CommandText;
                }
            }
        }

    }
    public static class FullTextSearchExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="expression"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> FreeTextSearch<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, object>> expression, string searchTerm) where TEntity : class
        {
            return FreeTextSearchImp(source, expression, FullTextPrefixes.Freetext(searchTerm));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="expression"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ContainsSearch<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, object>> expression, string searchTerm) where TEntity : class
        {
            return FreeTextSearchImp(source, expression, FullTextPrefixes.Contains(searchTerm));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="expression"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private static IQueryable<TEntity> FreeTextSearchImp<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, object>> expression, string searchTerm)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                return source;
            }

            // The below represents the following lamda:
            // source.Where(x => x.[property].Contains(searchTerm))

            //Create expression to represent x.[property].Contains(searchTerm)
            //var searchTermExpression = Expression.Constant(searchTerm);
            var searchTermExpression = Expression.Property(Expression.Constant(new { Value = searchTerm }), "Value");
            var checkContainsExpression = Expression.Call(expression.Body, typeof(string).GetMethod("Contains"), searchTermExpression);

            //Join not null and contains expressions

            var methodCallExpression = Expression.Call(typeof(Queryable),
                                                       "Where",
                                                       new[] { source.ElementType },
                                                       source.Expression,
                                                       Expression.Lambda<Func<TEntity, bool>>(checkContainsExpression, expression.Parameters));

            return source.Provider.CreateQuery<TEntity>(methodCallExpression);
        }
    }

}
