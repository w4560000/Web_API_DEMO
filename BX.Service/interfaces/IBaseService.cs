using BX.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BX.Service
{
    public interface IBaseService
    {
        GenericRepository<T> CreateService<T>() where T : class, new();
    }
}
