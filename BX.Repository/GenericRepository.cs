using BX.Repository.Base;
using System;
using System.Data;
using System.Linq;

namespace BX.Repository
{
    /// <summary>
    /// 基礎儲存庫介面
    /// </summary>
    /// <typeparam name="TEntity">資料庫物件</typeparam>
    public class GenericRepository<TEntity> : RepositoryBase
    {
        /// <summary>
        /// 資料庫連線
        /// </summary>
        private IDbConnection connection;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="sqlServerConnectionBase"></param>
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

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">資料庫物件</param>
        /// <returns>是否新增成功</returns>
        public bool Add(TEntity entity)
        {
            return this.Connection.Insert(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">資料庫物件</param>
        /// <returns>是否更新成功</returns>
        public bool Update(TEntity entity)
        {
            return this.Connection.Update(entity);
        }

        /// <summary>
        /// 根據條件取得物件
        /// </summary>
        /// <param name="conditions">條件</param>
        /// <param name="parameters">參數</param>
        /// <returns>資料庫物件</returns>
        public TEntity Get(string conditions, object parameters = null)
        {
            return this.Connection.GetList<TEntity>(conditions , parameters).SingleOrDefault();
        }
    }
}
