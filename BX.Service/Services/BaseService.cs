using BX.Repository;
using BX.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BX.Service
{
    public class BaseService : IBaseService
    {
        private IRepositoryFactory _repositoryFactory;
        private ISQLServerConnectionBase DbConnection;
        public BaseService(IRepositoryFactory repositoryFactory, ISQLServerConnectionBase dbConnection)
        {
            this._repositoryFactory = repositoryFactory;
            this.DbConnection = dbConnection;
        }

        public GenericRepository<T> CreateService<T>() where T : class, new()
        {
            return this._repositoryFactory.CreateRepository<T>();
        }
    }
}
