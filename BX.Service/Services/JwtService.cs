using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BX.Service.Services
{
    public class JwtTokenService
    {
        private IConfiguration Configuration;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="configuration"></param>
        public JwtTokenService( IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// 建立JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="IsUpdatExp">是否更新JWT時效</param>
        /// <returns></returns>
        public string ResponseJWT(string Account)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, Account) };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            ////更新該帳號登入時間
            //_RedisService.UpdateLoginDate(Account);

            ////取得該帳號登入時間
            //DateTime Account_Login_Date = _RedisService.GetRedisDate(Account);

            ////設定JWT時效為一小時
            //DateTime expiresAt = Account_Login_Date.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: "admin",
                audience: Account,
                claims: claims,
                //expires: expiresAt,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}