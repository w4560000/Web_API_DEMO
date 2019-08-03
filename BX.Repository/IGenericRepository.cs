namespace BX.Repository
{
    /// <summary>
    /// 基礎儲存庫介面
    /// </summary>
    /// <typeparam name="TEntity">資料庫物件</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">資料庫物件</param>
        /// <returns>是否新增成功</returns>
        bool Add(TEntity entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">資料庫物件</param>
        /// <returns>是否更新成功</returns>
        bool Update(TEntity entity);

        /// <summary>
        /// 根據條件取得物件
        /// </summary>
        /// <param name="conditions">條件</param>
        /// <param name="parameters">參數</param>
        /// <returns>資料庫物件</returns>
        TEntity Get(string conditions, object parameters = null);
    }
}
