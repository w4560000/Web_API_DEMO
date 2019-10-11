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
        /// API輸出結果
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// 結果訊息
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// 回傳狀態碼
        /// </summary>
        public string ResponseStatusCode { get; set; }

        /// <summary>
        /// JWTToken
        /// </summary>
        public JwtTokenViewModel JwtData { get; set; } = new JwtTokenViewModel();
    }
}
