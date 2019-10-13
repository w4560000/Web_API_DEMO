namespace BX.Web
{
    /// <summary>
    /// api 輸出的 Model
    /// </summary>
    /// <typeparam name="T">結果訊息輸出型態</typeparam>
    /// <typeparam name="U">回傳資料輸出型態</typeparam>
    public class ApiResponseViewModel<T, U>
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

        /// <summary>
        /// 回傳資料
        /// </summary>
        public U Data { get; set; }

        /// <summary>
        /// 是否從FB登入
        /// </summary>
        public bool IsFacebookLogin { get; set; } = false;
    }
}