﻿namespace BX.Service
{
    /// <summary>
    /// 服務致行結果
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// 設定為錯誤結果
        /// </summary>
        /// <param name="result">結果</param>
        /// <returns>錯誤的結果</returns>
        Result SetErrorMode();

        /// <summary>
        /// 設定訊息
        /// </summary>
        /// <param name="responseMessageEnum">回傳訊息列舉</param>
        void SetMessage(ResponseMessageEnum responseMessageEnum);
    }
}