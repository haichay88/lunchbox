using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Data.Core
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Constructors

        public EFUnitOfWork(DbContext context)
        {
            this.Context = context;
        }

        #endregion

        #region Properties

        public DbContext Context { get; protected set; }

        private Hashtable Repositories { get; set; }

        private bool isDisposed;

        #endregion

        #region Methods

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            // Validate setting
            if (Repositories == null)
                Repositories = new Hashtable();

            // Gets the Entity's name
            var type = typeof(TEntity).Name;

            // validate the exist state of the Entity in the repository by the Name.
            if (Repositories.ContainsKey(type))
                return (IRepository<TEntity>)Repositories[type];

            // Gets the type of repository.
            var repositoryType = typeof(EFGenericRepository<>);
            var m_Repository = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), this.Context);
            if (!this.Repositories.ContainsKey(type)) this.Repositories.Add(type, m_Repository);

            return (IRepository<TEntity>)Repositories[type];
        }
        public TRepository GetCustomRepository<TRepository>() where TRepository : class
        {
            throw new NotImplementedException();
        }
       
        public TContext GetContext<TContext>() where TContext : class
        {
            return Context as TContext;
        }

        public void BeginTransaction()
        {
            Context.Database.BeginTransaction();
        }
        public void Rollback()
        {
            if (this.Context.Database.CurrentTransaction != null)
                this.Context.Database.CurrentTransaction.Rollback();
        }
        public void Commit()
        {
            try
            {
                this.Context.SaveChanges();

                if (this.Context.Database.CurrentTransaction != null)
                    this.Context.Database.CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                this.Rollback();

                throw ex;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            // Make sure only one dispose
            if (isDisposed)
                GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing) this.Context.Dispose();

            isDisposed = true;
        }

        #endregion

        #region Constants

        #endregion
    }
}
