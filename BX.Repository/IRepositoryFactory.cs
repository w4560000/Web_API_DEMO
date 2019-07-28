using BX.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BX.Repository
{
    public interface IRepositoryFactory
    {
        GenericRepository<TEntity> CreateRepository<TEntity>() where TEntity : class;
    }
}
