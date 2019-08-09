using System;
using System.Collections.Generic;

namespace BX.Service
{
    /// <summary>
    /// 服務回傳結果
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// 錯誤例外
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 取得回傳結果訊息
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public List<string> ErrorMessage { get; set; } =  new List<string>();

        /// <summary>
        /// 成功或是失敗
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 錯誤訊息設定, 並將Succees設為false
        /// </summary>
        /// <param name="errorMessage">錯誤訊息</param>
        /// <param name="exc">錯誤例外訊息</param>
        public void SetError(string errorMessage, Exception exc = null)
        {
            this.Success = false;
            this.Exception = exc;

            this.ErrorMessage.Add(errorMessage);
        }

        /// <summary>
        /// 設定回傳訊息
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(T message)
        {
            this.Data.Add(message);
        }
    }
}