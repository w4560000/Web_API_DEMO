//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Security.Claims;
//using System.Text;

//namespace BX.Web.Controllers
//{
//    [AllowAnonymous]
//    [Route("api/[controller]")]
//    public class OauthController : Controller
//    {

//        public IConfiguration Configuration { get; }

//        public OauthController(IConfiguration configuration)
//        {
//            this.Configuration = configuration;
//        }

//        public static class CustomClaimTypes
//        {
//            public const string testCommenced = "20";
//        }


//        [HttpPost("authenticate")]
//        public IActionResult RequestToken([FromBody]AccountData request)
//        {
//            if (request != null)
//            {
//                //驗證帳號密碼，等DB建好在判斷帳密
//                if ("test1".Equals(request.Account) && "334567".Equals(request.PassWord))
//                {
//                    var claims = new[] {
//                        //加入用户的名稱
//                        new Claim(ClaimTypes.Name,request.Account),
//                        new Claim("CompletedBasicTraining", ""),
//                        //new Claim(CustomClaimTypes.EmploymentCommenced,"EmploymentCommenced", ClaimValueTypes.String)
//                         new Claim(CustomClaimTypes.testCommenced,
//                               "1",
//                                ClaimValueTypes.Integer32)
//                    };

//                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:SecurityKey"]));
//                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//                    var authTime = DateTime.UtcNow;
//                    var expiresAt = authTime.AddDays(5);

//                    var token = new JwtSecurityToken(
//                        issuer: "admin",
//                        audience: request.Account,
//                        claims: claims,
//                        expires: expiresAt,
//                        signingCredentials: creds);

//                    return this.Ok(new
//                    {
//                        access_token = new JwtSecurityTokenHandler().WriteToken(token),
//                        token_type = "Bearer",
//                        profile = new
//                        {
//                            Account = request.Account,
//                            auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
//                            expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
//                        }
//                    });
//                }
//            }

//            return BadRequest("Could not verify username and password.Pls check your information.");
//        }
//    }
//}