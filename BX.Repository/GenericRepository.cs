using BX.Repository.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;

namespace BX.Repository
{
    //public class GenericRepository<TEntity> : RepositoryBase<TEntity> , IGenericRepository<TEntity> where TEntity : class
    public class GenericRepository<TEntity> : RepositoryBase
    {
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool Disposed = false;

        private IDbConnection connection;

        public GenericRepository(ISQLServerConnectionBase sqlServerConnectionBase) : base(sqlServerConnectionBase)
        {
            this.connection = sqlServerConnectionBase.Connection;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~GenericRepository()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Add(TEntity entity)
        {
            return this.Connection.Insert(entity);
        }

        public bool Update(TEntity entity)
        {
            return this.Connection.Update(entity);
        }

        public TEntity Get(string conditions, object parameters = null)
        {
            return this.Connection.GetList<TEntity>(conditions , parameters).SingleOrDefault();
        }
    }
}
