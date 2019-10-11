using System;
using System.Collections.Generic;
using System.Linq;

namespace BX.Service
{
    /// <summary>
    /// 服務回傳結果
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// 回傳訊息
        /// </summary>
        public List<string> Message { get; private set; } = new List<string>();

        /// <summary>
        /// 回傳狀態碼
        /// </summary>
        public string ResponseStatusCode { get; private set; }

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
        public void SetMessage(ResponseMessageEnum responseMessageEnum)
        {
            string message = responseMessageEnum.GetEnumDescription();

            this.Message.Add(message);
            this.ResponseStatusCode = this.SetResponseStatusCode(message);
        }

        /// <summary>
        /// 設定訊息
        /// </summary>
        /// <param name="message">訊息</param>
        public void SetMessage(string exceptionMessage)
        {
            this.Message.Add(exceptionMessage);
            this.ResponseStatusCode = this.SetResponseStatusCode(exceptionMessage);
        }

        /// <summary>
        /// 設定回傳狀態碼
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string SetResponseStatusCode(string message)
        {
            return Enum.GetValues(typeof(ResponseMessageEnum))
                       .Cast<ResponseMessageEnum>()
                       .Where(s => s.GetEnumDescription().Equals(message))
                       .FirstOrDefault()
                       .GetEnumAttributeOfType<ResponseStatusAttribute>().StatusCode;
        }
    }
}