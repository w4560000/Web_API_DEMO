﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI_DEMO.Model.Table;
using Dapper;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace WebAPI_DEMO.Model
{
    public class AccountService :IAccountService
    {
        private FOR_VUEContext _DbContext;
        private IConfiguration _Configuration;
        private IRedisService _RedisService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AccountService(FOR_VUEContext DbContext, IConfiguration Configuration , IRedisService RedisService)
        {
            this._DbContext = DbContext;
            this._Configuration = Configuration;
            this._RedisService = RedisService;
        }

        public static class CustomClaimTypes
        {
            public const string testCommenced = "20";
        }

        /// <summary>
        /// 取得帳號基本資料
        /// </summary>
        /// <returns></returns>
        public AccountData GetAccountData(string Account)
        {
            var result = this._DbContext.AccountData.Where(r=>r.Account==Account).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 註冊時，確認帳號是否可使用(已被註冊則無法使用)
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        public bool CheckAccountCanUse(string Account)
        {
            if (Account != null)
            {
                using (var connection = new SqlConnection(_Configuration.GetConnectionString("FOR_VUEContext")))
                {
                    var Item = connection.Query("select * from AccountData where Account = @p1", new { p1 = Account });

                    if (Item.Count() == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 註冊時，確認Email是否可使用(已被註冊則無法使用)
        /// </summary>
        /// <param name="Email">信箱</param>
        /// <returns></returns>
        public bool CheckEmailCanUse(string Email)
        {
            if (Email != null)
            {
                using (var connection = new SqlConnection(_Configuration.GetConnectionString("FOR_VUEContext")))
                {
                    var Item = connection.Query("select * from AccountData where Email = @p1 ", new { p1 = Email });

                    if (Item.Count() == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="AccountData">註冊帳號資料(Account & Password & Email)</param>
        public void SignupAccount(AccountData AccountData)
        {
            var NEWAccountData = new AccountData()
            {
                Account = AccountData.Account,
                PassWord = GetMD5PassWord(AccountData.PassWord),
                Email = AccountData.Email,
                SignupDate = DateTime.Now,
                SignupFinish = "N"
            };

            this._DbContext.AccountData.Add(NEWAccountData);
            this._DbContext.SaveChanges();
        }

        /// <summary>
        /// 取得MD5加密過的密碼
        /// </summary>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public string GetMD5PassWord(string PassWord)
        {
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(PassWord));

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }
        

        

        /// <summary>
        /// 寄送驗證碼
        /// </summary>
        /// <param name="Email">信箱</param>
        public void SendMail(string Email,string dosomeing)
        {
             int[] ValidationCodeArray= CreateValidationCode();//產生4位驗證碼
            var ValidationCode = "";

             var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test Project", "netcoremailtest87@gmail.com"));
            message.To.Add(new MailboxAddress("test", Email));
            message.Subject = "test send mail form asp.net core ";
            

            var bodyBuilder = new BodyBuilder();
            string body = "";

            switch (dosomeing)
            {
                case "signup":
                     body += "<span style=\"color:black;\">安安你好!恭喜中毒囉~</span> <br/><br/> <span style=\"color:black;\">It's joking! Don't worry~!</span><br/><br/><br/><br/>";
                    break;
                case "reset_password":
                     body += "<span style=\"color:black;\">安安你好!</span> <br/><br/><span style=\"color:black;\">輸入完驗證碼即可重設密碼！</span><br/><br/><br/><br/>";
                    break;
                case "resendemail":
                    body += "<span style=\"color:black;\">安安你好</span> <br/><br/> <span style=\"color:black;\">這是系統重發的驗證信　有收到嗎？？？？？</span><br/><br/><br/><br/>";
                    break;
            }

            body += "<span style=\"font-size:20px;color:black;\">Hello</span><br/><span style=\"font-size:20px;color:black;\">This is 鄭秉庠's .Net Core Mail Test!</span><br/>";

            body += "<span style=\"font-size:20px;color:black;\">Your Validation Code is : <span style=\"font-size:30px;color:red;\">";

            for(int i=0;i<4;i++)
            {
                body += string.Format("{0}",ValidationCodeArray[i] ) + "  ";
                ValidationCode += ValidationCodeArray[i];
            }
            body += "</span></span>";
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("netcoremailtest87@gmail.com", "test.0123");

                client.Send(message);

                client.Disconnect(true);
            }

            AccountData Item = this._DbContext.AccountData.Where(r => r.Email == Email).FirstOrDefault();
            Item.SendmailDate = DateTime.Now;
            Item.VerificationCode = ValidationCode;


            this._DbContext.SaveChanges();
        }

        /// <summary>
        /// 註冊完成後，確認使用者輸入的驗證碼是否正確
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="VerificationCode">四位驗證碼</param>
        /// <returns></returns>
        public string CheckVerificationCode(string Account, string VerificationCode)
        {
            var item = this._DbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

            if (item.VerificationCode == VerificationCode)
            {
                item.SignupFinish = "Y";
                this._DbContext.SaveChanges();

                return "驗證成功！";
            }
            else
                return "驗證失敗！請確認驗證碼是否輸入正確！";
        }

        /// <summary>
        /// 產生四位驗證碼
        /// </summary>
        /// <returns></returns>
        public int[] CreateValidationCode()
        {
            int[] randomArray = new int[4];
            Random rnd = new Random();  //產生亂數初始值
            for (int i = 0; i < 4; i++)
            {
                randomArray[i] = rnd.Next(1, 10);   //亂數產生，亂數產生的範圍是1~9

                for (int j = 0; j < i; j++)
                {
                    while (randomArray[j] == randomArray[i])    //檢查是否與前面產生的數值發生重複，如果有就重新產生
                    {
                        j = 0;  //如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                        randomArray[i] = rnd.Next(1, 10);   //重新產生，存回陣列，亂數產生的範圍是1~9
                    }
                }
            }

            return randomArray;
        }

        /// <summary>
        /// 帳號登入，驗證帳密
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        /// <returns></returns>
        public bool SigninValidation(string Account, string PassWord)
        {
            using (var connection = new SqlConnection(_Configuration.GetConnectionString("FOR_VUEContext")))
            {
                var Item = connection.Query("select * from AccountData where Account = @p1 and PassWord =@p2", new { p1 = Account, p2 = GetMD5PassWord(PassWord) });

                if (Item.Count() != 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 登入前確認是否認證過驗證碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        public bool CheckSignupFinish(string Account)
        {
            var item = this._DbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

            if (item.SignupFinish == "Y")
            {
                item.SigninDate = DateTime.Now;
                this._DbContext.SaveChanges();

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 確認帳號與信箱是否是同一人所有
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Email">信箱</param>
        /// <returns></returns>
        public string CheckAccount_Email_for_reset_PassWord_or_resendEmail(string Account, string Email)
        {
            var Item = GetAccountData(Account);
            if (Item != null)
            {
                if (Item.Email == Email)
                    return "正確";
                else
                    return "信箱錯誤";
            }
            else
            {
                return "無此帳號";
            }
        }

        /// <summary>
        /// 重置密碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        public void ResetPassWord(string Account, string PassWord)
        {
            var Item = this._DbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();


            Item.PassWord = GetMD5PassWord(PassWord);
            this._DbContext.SaveChanges();
        }

        /// <summary>
        /// 建立JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="IsUpdatExp">是否更新JWT時效</param>
        /// <returns></returns>
        public string ResponseJWT(string Account)
        {
            var claims = new[] {
                        //加入用户的名稱
                        new Claim(ClaimTypes.Name,Account),
                        new Claim("CompletedBasicTraining", ""),
                        //new Claim(CustomClaimTypes.EmploymentCommenced,"EmploymentCommenced", ClaimValueTypes.String)
                         new Claim(CustomClaimTypes.testCommenced,
                               "1",
                                ClaimValueTypes.Integer32)
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWT:SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //更新該帳號登入時間
            _RedisService.UpdateLoginDate(Account);

            //取得該帳號登入時間
            DateTime Account_Login_Date = _RedisService.GetRedisDate(Account);
            
            //設定JWT時效為一小時
            DateTime expiresAt = Account_Login_Date.AddHours(1); 


            var token = new JwtSecurityToken(
                issuer: "admin",
                audience: Account,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

             return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Account">帳號</param>
        public void LogOut(string Account)
        {
            _RedisService.DeleteLoginDate(Account);
        }
    }
}
