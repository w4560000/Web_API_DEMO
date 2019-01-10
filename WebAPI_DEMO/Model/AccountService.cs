using MailKit.Net.Smtp;
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

namespace WebAPI_DEMO.Model
{
    public class AccountService :IAccountService
    {
        private FOR_VUEContext _dbContext;
        public IConfiguration Configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AccountService(FOR_VUEContext dbContext, IConfiguration configuration)
        {
            this._dbContext = dbContext;
            this.Configuration = configuration;
        }

        public static class CustomClaimTypes
        {
            public const string testCommenced = "20";
        }

        /// <summary>
        /// 取得帳號基本資料
        /// </summary>
        /// <returns></returns>
        public List<AccountData> GetAccountData()
        {
            var result = this._dbContext.AccountData.ToList();
            return result;
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
                PassWord = AccountData.PassWord,
                Email = AccountData.Email,
                SignupDate = DateTime.Now
            };

            this._dbContext.AccountData.Add(NEWAccountData);
            this._dbContext.SaveChanges();
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
                using (var connection = new SqlConnection(Configuration.GetConnectionString("FOR_VUEContext")))
                {
                    var Item = connection.Query("select * from AccountData where Account = @p1", new { p1 = Account });

                    if (Item == null)
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
                using (var connection = new SqlConnection(Configuration.GetConnectionString("FOR_VUEContext")))
                {
                    var Item = connection.Query("select * from AccountData where Email = @p1 ", new { p1 = Email });

                    if (Item == null)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// 寄送驗證碼
        /// </summary>
        /// <param name="Email">信箱</param>
        public void SendMail(string Email)
        {
             int[] ValidationCodeArray= CreateValidationCode();//產生4位驗證碼
            var ValidationCode = "";

             var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test Project", "netcoremailtest87@gmail.com"));
            message.To.Add(new MailboxAddress("test", Email));
            message.Subject = "test send mail form asp.net core ";
            

            var bodyBuilder = new BodyBuilder();
            string body= "<span style=\"color:black;\">安安你好!恭喜中毒囉~</span> <br/><br/> <span style=\"color:black;\">It's joking! Don't worry~!</span><br/><br/><br/><br/>";

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

            AccountData Item = this._dbContext.AccountData.Where(r => r.Email == Email).FirstOrDefault();
            Item.SendmailDate = DateTime.Now;
            Item.VerificationCode = ValidationCode;


            this._dbContext.SaveChanges();
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
        /// 註冊完成後，確認使用者輸入的驗證碼是否正確
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="VerificationCode">四位驗證碼</param>
        /// <returns></returns>
        public string CheckVerificationCode(string Account,string VerificationCode)
        {
            var item = this._dbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

            if (item.VerificationCode == VerificationCode)
                return "驗證成功！";
            else
                return "驗證失敗！請確認驗證碼是否輸入正確！";
        }

        /// <summary>
        /// 建立JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="IsUpdatExp">是否更新JWT時效</param>
        /// <returns></returns>
        public string ResponseJWT(string Account, bool IsUpdatExp)
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ1456789513"));//Encoding.UTF8.GetBytes(Configuration["JWT:SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            DateTime authTime = DateTime.UtcNow;
            DateTime expiresAt;
            if (IsUpdatExp)
            {
                //Redis連線
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase(0);

                var Account_Login_Date = db.HashGetAll(Account + "_Login_Date").ToList();

                redis.Dispose();

                expiresAt = authTime;

            }
            else
            {
                expiresAt = authTime.AddHours(1);
            }
            var token = new JwtSecurityToken(
                issuer: "admin",
                audience: Account,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

           
             return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 帳號登入，驗證帳密
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        /// <returns></returns>
        public bool SigninValidation(string Account, string PassWord)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("FOR_VUEContext")))
            {
                var Item = connection.Query("select * from AccountData where Account = @p1 and PassWord =@p2", new { p1 = Account, p2 = PassWord });

                if (Item != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
