using BX.Repository;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace BX.Repository.Base
{
    /// <summary>
    /// 基礎儲存庫
    /// </summary>
    public class RepositoryBase
    {
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool Disposed = false;

        /// <summary>
        /// SQL SERVER連線
        /// </summary>
        private ISQLServerConnectionBase SqlServerConnectionBase;
        public RepositoryBase(
            ISQLServerConnectionBase sqlServerConnectionBase)
        {
            this.SqlServerConnectionBase = sqlServerConnectionBase;
        }

        /// <summary>
        /// 資料庫連線
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
                // 登記指定的交易。
                DbConnection conn = this.SqlServerConnectionBase.Connection as DbConnection;
                if (Transaction.Current != null)
                {
                    conn?.EnlistTransaction(Transaction.Current);
                }

                return conn;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, dispose all managed and unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.Disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.Connection.Dispose();
                }
                // Note disposing has been done.
                this.Disposed = true;
            }
        }

        //System.Configuration.ConfigurationManager.ConnectionStrings[this.ConnectionKey].ConnectionString
    }
}
