using BX.Repository;
using BX.Repository.Entity;
using BX.Service.ViewModel;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// 帳號服務
    /// </summary>
    public class AccountService : BaseService, IAccountService
    {
        /// <summary>
        /// 帳號儲存庫
        /// </summary>
        private readonly GenericRepository<Account> AccountRepository;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="dbConnection"></param>
        public AccountService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
            this.AccountRepository = this.CreateService<Account>();
        }

        /// <summary>
        /// 註冊帳號流程
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        public Result<string> SignupAccountProcess(AccountViewModel account)
        {
            Result<string> result = new Result<string>();

            (bool canUse, List<string> message) = this.CheckAccountCanUse(account);

            if (canUse)
            {
                this.SignupAccount(account);
                this.SendMail(account.Email, AccountFunctionEnum.Signup);
            }
            else
            {
                message.ForEach(x=> result.SetError(x));

                return result;
            }

            return result;
        }

        /// <summary>
        /// 檢查帳號可否使用
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        private (bool, List<string>) CheckAccountCanUse(AccountViewModel account)
        {
            List<string> errorMessage = new List<string>();
            if (account != null)
            {
                bool isAccountExist = this.AccountRepository
                                     .Get("WHERE AccountName = @AccountName", new { AccountName = account.AccountName })
                                     .FirstOrDefault() != null ;

                bool isMailExist = this.AccountRepository
                                       .Get("WHERE Email = @Email", new { account.Email })
                                       .FirstOrDefault() != null;
                                    

                if(isAccountExist)
                    errorMessage.Add(ResponseMessageEnum.AccountNameUsed.GetEnumDescription());

                if (isMailExist)
                    errorMessage.Add(ResponseMessageEnum.EmailUsed.GetEnumDescription());
            }
            else
            {
                errorMessage.Add(ResponseMessageEnum.AccountNameIsNull.GetEnumDescription());
            }

            return (!errorMessage.Any(), errorMessage);
        }


        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="AccountData"></param>
        private void SignupAccount(AccountViewModel account)
        {
            Account entity = new Account()
            {
                AccountName = account.AccountName,
                PassWord = this.GetMD5PassWord(account.PassWord),
                Email = account.Email,
                SignupDate = DateTime.Now,
            };

            bool isInsertSuccess = this.AccountRepository.Add(entity);

            if(!isInsertSuccess)
            {
                throw new Exception(ResponseMessageEnum.InsertError.GetEnumDescription());
            }
        }

        /// <summary>
        /// 取得MD5加密過的密碼
        /// </summary>
        /// <param name="PassWord">密碼</param>
        /// <returns>加密過的密碼</returns>
        private string GetMD5PassWord(string PassWord)
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
        private void SendMail(string Email, AccountFunctionEnum accountFunction)
        {
            int[] ValidationCodeArray = this.CreateValidationCode();//產生4位驗證碼
            var ValidationCode = "";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test Project", "netcoremailtest87@gmail.com"));
            message.To.Add(new MailboxAddress("test", Email));
            message.Subject = "test send mail form asp.net core ";


            var bodyBuilder = new BodyBuilder();
            string body = "";

            switch (accountFunction)
            {
                case AccountFunctionEnum.Signup:
                    body += "<span style=\"color:black;\">安安你好!恭喜中毒囉~</span> <br/><br/> <span style=\"color:black;\">It's joking! Don't worry~!</span><br/><br/><br/><br/>";
                    break;
                case AccountFunctionEnum.ResetPassWord:
                    body += "<span style=\"color:black;\">安安你好!</span> <br/><br/><span style=\"color:black;\">輸入完驗證碼即可重設密碼！</span><br/><br/><br/><br/>";
                    break;
                case AccountFunctionEnum.ResetMail:
                    body += "<span style=\"color:black;\">安安你好</span> <br/><br/> <span style=\"color:black;\">這是系統重發的驗證信　有收到嗎？？？？？</span><br/><br/><br/><br/>";
                    break;
            }

            body += "<span style=\"font-size:20px;color:black;\">Hello</span><br/><span style=\"font-size:20px;color:black;\">This is 鄭秉庠's .Net Core Mail Test!</span><br/>";

            body += "<span style=\"font-size:20px;color:black;\">Your Validation Code is : <span style=\"font-size:30px;color:red;\">";

            for (int i = 0; i < 4; i++)
            {
                body += string.Format("{0}", ValidationCodeArray[i]) + "  ";
                ValidationCode += ValidationCodeArray[i];
            }
            body += "</span></span>";
            bodyBuilder.HtmlBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("testbingxiang@gmail.com", "a33456789");

                client.Send(message);

                client.Disconnect(true);
            }

            Account Item = this.AccountRepository.Get("WHERE Email = @Email", new { Email }).FirstOrDefault();
            Item.SendMailDate = DateTime.Now;
            Item.VerificationCode = ValidationCode;
            this.AccountRepository.Update(Item);
        }

        /// <summary>
        /// 產生四位驗證碼
        /// </summary>
        /// <returns></returns>
        private int[] CreateValidationCode()
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
    }
}
