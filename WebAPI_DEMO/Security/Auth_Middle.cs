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
                if (context.User.FindFirst(claim => claim.Type == CustomClaimTypes.testCommenced) != null)
                {
                    

                    var parameter = context.User.FindFirst(claim => claim.Type == CustomClaimTypes.testCommenced).Value;
                    if (requirement.testpa == int.Parse(parameter))
                    {
                        //REDIS TEST
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

                        IDatabase db = redis.GetDatabase(0);
                        db.StringSet("test1", context.User.FindFirst(claim => claim.Type == CustomClaimTypes.testCommenced).Value);

                        db.HashSet("LoginAccount", new HashEntry[] { new HashEntry(context.User.FindFirst(claim => claim.Type == "aud").Value + "_Login_Date", context.User.FindFirst(claim => claim.Type == "exp").Value) });

                        redis.Dispose();

                        context.Succeed(requirement);
                    }
                }
                return Task.CompletedTask;

            }
        }

    }
}
