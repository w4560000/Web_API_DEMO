using BX.Repository;

namespace BX.Service
{
    /// <summary>
    /// 基礎服務
    /// </summary>
    public class BaseService : IBaseService
    {
        /// <summary>
        /// 基礎儲存庫
        /// </summary>
        private IRepositoryFactory RepositoryFactory;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="repositoryFactory">基礎儲存庫</param>
        public BaseService(IRepositoryFactory repositoryFactory)
        {
            this.RepositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// 取得基礎儲存庫
        /// </summary>
        /// <typeparam name="T">資料庫物件</typeparam>
        /// <returns>基礎儲存庫</returns>
        public GenericRepository<T> CreateService<T>() where T : class, new()
        {
            return this.RepositoryFactory.CreateRepository<T>();
        }
    }
}
