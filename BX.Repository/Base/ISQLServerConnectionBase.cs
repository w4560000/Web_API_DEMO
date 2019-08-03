using System.Data;

namespace BX.Repository.Base
{
    /// <summary>
    /// 資料庫連線
    /// </summary>
    public interface ISQLServerConnectionBase
    {
        /// <summary>
        /// 資料庫連線
        /// </summary>
        IDbConnection Connection { get; set; }
    }
}
