using BX.Service;
using System.Collections.Generic;

namespace BX.Web
{
    /// <summary>
    /// 轉換型別Helper
    /// </summary>
    public class ConvertHelper
    {
        /// <summary>
        /// 轉成ApiResponse
        /// </summary>
        /// <param name="result">ServerResult</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponseViewModel<List<string>> CovertToApiResponse(Result result)
        {
            return new ApiResponseViewModel<List<string>>()
            {
                IsSuccess = result.IsSuccess,
                Result = result.Message
            };
        }

        /// <summary>
        /// 轉成ApiResponse
        /// </summary>
        /// <param name="result">ServerResult</param>
        /// <param name="apiResponse">ApiResponse</param>
        public static void CovertToApiResponse(Result result, ApiResponseViewModel<List<string>> apiResponse)
        {
            apiResponse.IsSuccess = result.IsSuccess;
            apiResponse.Result = result.Message;
        }
    }
}