using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// Jwt服務
    /// </summary>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// 組態服務
        /// </summary>
        private readonly IConfiguration Configuration;

        /// <summary>
        /// Redis服務
        /// </summary>
        private readonly IRedisService RedisService;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="configuration">組態服務</param>
        /// <param name="redisService">Redis服務</param>
        public JwtService(
            IConfiguration configuration,
            IRedisService redisService)
        {
            this.Configuration = configuration;
            this.RedisService = redisService;
        }

        /// <summary>
        /// 建立JWT
        /// </summary>
        /// <param name="account">帳號</param>
        /// <param name="isUpdatExp">是否更新JWT時效</param>
        /// <returns></returns>
        public string ResponseJWT(string account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //更新該帳號登入時間
            this.RedisService.UpdateAccountLoginDate(account);

            //取得該帳號登入時間
            DateTime Account_Login_Date = this.RedisService.GetAccountLoginDate(account);

            //設定JWT時效為一小時
            DateTime expiresAt = Account_Login_Date.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: "admin",
                audience: account,
                claims: new[] { new Claim(ClaimTypes.Name, account) },
                expires: expiresAt,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}