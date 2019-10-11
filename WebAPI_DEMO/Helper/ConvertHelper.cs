using BX.Service;
using System.Collections.Generic;

namespace BX.Web
{
    /// <summary>
    /// 轉換型別Helper
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 轉成ApiResponse
        /// </summary>
        /// <param name="result">ServerResult</param>
        /// <returns>ApiResponse</returns>
        public static ApiResponseViewModel<List<string>> CovertToApiResponse(this Result result)
        {
            return new ApiResponseViewModel<List<string>>()
            {
                IsSuccess = result.IsSuccess,
                Result = result.Message,
                ResponseStatusCode = result.ResponseStatusCode
            };
        }
    }
}