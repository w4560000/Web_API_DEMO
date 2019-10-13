using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace BX.Repository.Base
{
    /// <summary>
    /// SQL SERVER 連線
    /// </summary>
    public class SQLServerConnectionBase : ISQLServerConnectionBase
    {
        private IConfiguration _Configuration;

        public SQLServerConnectionBase(IConfiguration configuration)
        {
            this._Configuration = configuration;
        }

        /// <summary>
        /// 資料庫連線
        /// </summary>
        private IDbConnection ConnectionInstance;

        /// <summary>
        /// 資料庫連線
        /// </summary>
        public virtual IDbConnection Connection
        {
            get
            {
                if (this.ConnectionInstance == null)
                {
                    // Creates a ProfiledDbConnection instance and opens it
                    this.ConnectionInstance = new SqlConnection(GetBxDbConnectionFromAzureKeyVault());
                }

                return this.ConnectionInstance;
            }
            set => this.ConnectionInstance = value;
        }

        /// <summary>
        /// 建立資料庫連線
        /// </summary>
        public virtual DbConnection CreateConnection()
        {
            // 連線字串
            string connectionString = GetBxDbConnectionFromAzureKeyVault();

            // 資料庫類型
            string providerName = this._Configuration.GetSection("ConnectionStrings").GetChildren().ToString();

            DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);
            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
        }

        /// <summary>
        /// 取得儲存在Azure上的資料庫連線字串
        /// </summary>
        private static string GetBxDbConnectionFromAzureKeyVault()
        {
            return AzureKeyvaultHelper.GetAzureSecretVaule("bxdbconnection");
        }
    }
}