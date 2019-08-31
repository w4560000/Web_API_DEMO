namespace BX.Service
{
    /// <summary>
    /// 服務致行結果帶有查詢資料
    /// </summary>
    /// <typeparam name="T">查詢資料</typeparam>
    public class Result<T> : Result, IResultData<T>
    {
        /// <summary>
        /// 取得回傳結果訊息
        /// </summary>
        public T Data { get; set; }
    }
}