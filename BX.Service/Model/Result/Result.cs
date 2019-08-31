using System.Collections.Generic;

namespace BX.Service
{
    /// <summary>
    /// 服務回傳結果
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// 訊息
        /// </summary>
        public List<string> Message { get; private set; } = new List<string>();

        /// <summary>
        /// 成功或是失敗
        /// </summary>
        public bool IsSuccess { get; private set; } = true;

        /// <summary>
        /// 設定為錯誤結果
        /// </summary>
        /// <param name="result">結果</param>
        /// <returns>錯誤的結果</returns>
        public Result SetErrorMode()
        {
            this.IsSuccess = false;

            return this;
        }

        /// <summary>
        /// 設定訊息
        /// </summary>
        /// <param name="message">訊息</param>
        public void SetMessage(string message)
        {
            this.Message.Add(message);
        }
    }
}