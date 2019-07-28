using MiniProfiler.Integrations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BX.Repository.Base
{
    /// <summary>
    /// 資料庫連線
    /// </summary>
    public interface ISQLServerConnectionBase : IDbConnectionFactory
    {
        /// <summary>
        /// 資料庫連線
        /// </summary>
        IDbConnection Connection { get; set; }
    }
}
