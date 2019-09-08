using BX.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BX.Web.Security
{
    /// <summary>
        /// 驗證中介層
        /// </summary>
    public class Auth_Middle : IAuthorizationRequirement
    {
        /// <summary>
        /// 驗證中介層
        /// </summary>
        public class Auth_MiddleHandler : AuthorizationHandler<Auth_Middle>
        {
            private readonly IHttpContextAccessor HttpContextAccessor;

            /// <summary>
            /// 建構子
            /// </summary>
            /// <param name="redisService"></param>
            public Auth_MiddleHandler(IHttpContextAccessor httpContextAccessor)
            {
                this.HttpContextAccessor = httpContextAccessor;
            }

            protected override Task HandleRequirementAsync(
                    AuthorizationHandlerContext context,
                    Auth_Middle requirement)
            {
                //authorizationFilterContext.Result = new JsonResult("Need a custom message") { StatusCode = 418 };
                //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

                //IDatabase db = redis.GetDatabase(0);

                ////db.HashSet("LoginAccountDate", new HashEntry[] { new HashEntry(context.User.FindFirst(claim => claim.Type == "aud").Value , context.User.FindFirst(claim => claim.Type == "exp").Value) });

                ////每次request需要驗證的action時，更新JWT時間(代表該user有在使用)
                //db.HashSet("LoginAccountDate", new HashEntry[] { new HashEntry(context.User.FindFirst(claim => claim.Type == "aud").Value, DateTime.Now.ToString()) });

                //redis.Dispose();

                //context.Succeed(requirement);
                //HttpContext httpContext = this.HttpContextAccessor.HttpContext; // Access context here
                //var a = httpContext.Request.Query["accountData"].FirstOrDefault();
                //var result = string.Empty;
                //using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
                //{
                //    result = reader.ReadToEnd();
                //}


                return Task.CompletedTask;

            }
        }

    }
}
