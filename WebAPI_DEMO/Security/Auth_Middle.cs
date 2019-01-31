using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebAPI_DEMO.Controllers.OauthController;

namespace WebAPI_DEMO.Security
{
    public class Auth_Middle : IAuthorizationRequirement
    {


        public int testpa { get; private set; }

        public Auth_Middle(int pa1)
        {
            this.testpa = pa1;
        }

        public class Auth_MiddleHandler : AuthorizationHandler<Auth_Middle>
        {
            protected override Task HandleRequirementAsync(
                    AuthorizationHandlerContext context,
                    Auth_Middle requirement)
            {


                //var parameter = context.User.FindFirst(claim => claim.Type == CustomClaimTypes.testCommenced).Value;
                //if (requirement.testpa == int.Parse(parameter))
                //{
                //REDIS TEST
                
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("182.155.24.37:6379");

                IDatabase db = redis.GetDatabase(0);

                //db.HashSet("LoginAccountDate", new HashEntry[] { new HashEntry(context.User.FindFirst(claim => claim.Type == "aud").Value , context.User.FindFirst(claim => claim.Type == "exp").Value) });

                //每次request需要驗證的action時，更新JWT時間(代表該user有在使用)
                db.HashSet("LoginAccountDate", new HashEntry[] { new HashEntry(context.User.FindFirst(claim => claim.Type == "aud").Value, DateTime.Now.ToString()) });

                redis.Dispose();

                context.Succeed(requirement);
                //}

                return Task.CompletedTask;

            }
        }

    }
}
