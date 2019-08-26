using BX.Repository;

namespace BX.Service
{
    /// <summary>
    /// 基礎服務介面
    /// </summary>
    public interface IBaseService
    {
        /// <summary>
        /// 取得基礎儲存庫
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        GenericRepository<T> CreateService<T>() where T : class, new();
    }
}
