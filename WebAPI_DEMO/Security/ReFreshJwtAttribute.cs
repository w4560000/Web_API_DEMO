using BX.Service;
using BX.Service.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace BX.Web.Security
{
    /// <summary>
    /// ReFreshJwtFilter
    /// </summary>
    public class ReFreshJwtFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 當前使用者名稱  todo: 需要一個static class來獲取UserInfo
        /// </summary>
        private string CurrentAccountName { get; set; }

        /// <summary>
        /// action完成後 自動刷新Jwt
        /// </summary>
        /// <param name="context">ActionExecutedContext</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is ApiResponseViewModel<List<string>> response)
                {
                    if (response.IsSuccess)
                    {
                        response.JWT = context.HttpContext.RequestServices.GetService<IJwtService>()
                                                                          .ResponseJWT(this.CurrentAccountName);
                    }
                }
            }
        }

        /// <summary>
        /// action動作前 先把request 的 帳號名稱記下來  
        /// </summary>
        /// <param name="context">ActionExecutingContext</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Values.FirstOrDefault() is AccountViewModel accountData)
            {
                this.CurrentAccountName = accountData.AccountName;
            }
        }
    }
}