using BX.Repository.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BX.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private ISQLServerConnectionBase SqlServerConnectionBase;
        public RepositoryFactory(
            ISQLServerConnectionBase sqlServerConnectionBase)
        {
            this.SqlServerConnectionBase = sqlServerConnectionBase;
        }
        public GenericRepository<T> CreateRepository<T>() where T : class
        {
            return new GenericRepository<T>(this.SqlServerConnectionBase);
        }
    }
}
