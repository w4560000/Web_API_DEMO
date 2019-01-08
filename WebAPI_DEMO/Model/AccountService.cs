using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Model
{
    public class AccountService :IAccountService
    {
        private FOR_VUEContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AccountService(FOR_VUEContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<AccountData> GetAccountData()
        {
            var result = this._dbContext.AccountData.ToList();
            return result;
        }

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
        //註冊帳號時，先檢查帳號是否重複
        public bool CheckAccountCanUse(string Account)
        {
            if (Account != null)
            {
                var result = this._dbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

                if (result == null)
                    return true;
            }

            return false;
        }
        //註冊帳號時，先檢查mail是否重複
        public bool CheckEmailCanUse(string Email)
        {
            if (Email != null)
            {
                var result = this._dbContext.AccountData.Where(r => r.Email == Email).FirstOrDefault();

                if (result == null)
                    return true;
            }

            return false;
        }

        //寄送驗證碼
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
        //產生四位驗證碼
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
        //完成註冊，檢查user輸入的驗證碼是否正確
        public string SignupFinish(string Account,string ValidationCode)
        {
            var item = this._dbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

            if (item.VerificationCode == ValidationCode)
                return "驗證成功！";
            else
                return "驗證失敗！　請確認驗證碼是否輸入正確！";
        }
    }
}
