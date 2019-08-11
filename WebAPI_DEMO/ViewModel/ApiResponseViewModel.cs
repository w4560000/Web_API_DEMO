using BX.Service;

namespace BX.Web
{
    /// <summary>
    /// api 輸出的 Model
    /// </summary>
    /// <typeparam name="T">輸出型態</typeparam>
    public class ApiResponseViewModel<T>
    {
        /// <summary>
        /// 代碼
        /// </summary>
        public int Code { get; set; } = (int)ResponseEnum.Fail;

        /// <summary>
        /// 結果
        /// </summary>
        public T Result { get; set; }
    }
}
