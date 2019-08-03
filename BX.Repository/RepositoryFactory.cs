using BX.Repository.Base;

namespace BX.Repository
{
    /// <summary>
    /// 資料庫連線
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        /// <summary>
        /// SQL Server連線
        /// </summary>
        private ISQLServerConnectionBase SqlServerConnectionBase;
        
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="sqlServerConnectionBase"></param>
        public RepositoryFactory(
            ISQLServerConnectionBase sqlServerConnectionBase)
        {
            this.SqlServerConnectionBase = sqlServerConnectionBase;
        }

        /// <summary>
        /// 建立或取得資料庫連線
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public GenericRepository<T> CreateRepository<T>() where T : class
        {
            return new GenericRepository<T>(this.SqlServerConnectionBase);
        }
    }
}
