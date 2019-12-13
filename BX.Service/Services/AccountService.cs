using BX.Repository;
using BX.Repository.Entity;
using BX.Service.Model;
using BX.Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BX.Service
{
    /// <summary>
    /// 帳號服務
    /// </summary>
    public class AccountService : BaseService, IAccountService
    {
        private Result Result = new Result();

        /// <summary>
        /// 帳號儲存庫
        /// </summary>
        private readonly GenericRepository<Account> AccountRepository;

        /// <summary>
        /// 信件內容設定服務
        /// </summary>
        private readonly IMailInfoService MailInfoService;

        /// <summary>
        /// Redis服務
        /// </summary>
        private readonly IRedisService RedisService;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="repositoryFactory">基礎儲存庫</param>
        /// <param name="mailInfoService">信件內容設定服務</param>
        /// <param name="redisService">Redis服務</param>
        public AccountService(
            IRepositoryFactory repositoryFactory,
            IMailInfoService mailInfoService,
            IRedisService redisService) : base(repositoryFactory)
        {
            this.AccountRepository = this.CreateService<Account>();
            this.MailInfoService = mailInfoService;
            this.RedisService = redisService;
        }

        /// <summary>
        /// 註冊帳號流程
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns></returns>
        public Result SignupAccountProcess(AccountViewModel account)
        {
            (bool canUse, List<string> message) = this.CheckAccountCanUse(account);

            if (canUse)
            {
                this.SignupAccount(account);
                this.Result.SetMessage(ResponseMessageEnum.SignupSuccess);
            }
            else
            {
                this.Result.SetErrorMode();
                message.ForEach(x => this.Result.SetMessage(x));

                return this.Result;
            }

            return this.Result;
        }

        /// <summary>
        /// 驗證使用者輸入的驗證碼是否正確，完成註冊程序
        /// </summary>
        /// <param name="accountName">帳號名稱</param>
        /// <param name="verificationCode">四位驗證碼</param>
        /// <returns>驗證結果</returns>
        public Result CheckVerificationCode(string accountName, string verificationCode)
        {
            Account item = this.GetAccountData(accountName);

            if (item.VerificationCode.Equals(verificationCode))
            {
                item.SignupFinish = true;
                bool isUpdateSuccess = this.AccountRepository.Update(item);

                if (isUpdateSuccess)
                    this.Result.SetMessage(ResponseMessageEnum.ValidateSuccess);
                else
                    this.Result.SetErrorMode().SetMessage(ResponseMessageEnum.UpdateError);
            }
            else
                this.Result.SetErrorMode().SetMessage(ResponseMessageEnum.ValidateFail);

            return this.Result;
        }

        /// <summary>
        /// 帳號登入，驗證帳密
        /// </summary>
        /// <param name="account">帳號資訊</param>
        /// <returns>登入結果</returns>
        public Result Signin(AccountViewModel account)
        {
            Account item = this.GetAccountData(account.AccountName);

            try
            {
                if (item != null)
                {
                    if (Md5Hash.GetMd5Hash(account.PassWord).Equals(item.PassWord))
                    {
                        if (item.SignupFinish)
                        {
                            this.Result.SetMessage(ResponseMessageEnum.SigninSuccess);

                            return this.Result;
                        }
                        throw new AccountException(ResponseMessageEnum.EmailUnAuthentication.GetEnumDescription());
                    }
                }

                throw new AccountException(ResponseMessageEnum.SigninFail.GetEnumDescription());
            }
            catch (AccountException ex)
            {
                this.Result.SetErrorMode().SetMessage(ex.Message);

                return this.Result;
            }
        }

        /// <summary>
        /// 確認帳號與信箱是否是同一人所有
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Email">信箱</param>
        /// <returns>確認結果</returns>
        public Result ReSendEmailForReSetPassWord(AccountViewModel accountViewModel)
        {
            Account Item = this.GetAccountData(accountViewModel.AccountName);
            if (Item != null)
            {
                if (Item.Email == accountViewModel.Email)
                {
                    this.SendMail(accountViewModel, accountViewModel.ResendMailType);

                    Account entity = new Account()
                    {
                        Id = Item.Id,
                        AccountName = Item.AccountName,
                        PassWord = Item.PassWord,
                        Email = Item.Email,
                        SignupDate = Item.SignupDate,
                        SendMailDate = DateTime.Now,
                        VerificationCode = accountViewModel.VerificationCode
                    };

                    bool isUpdateSuccess = this.AccountRepository.Update(entity);

                    if (!isUpdateSuccess)
                    {
                        throw new Exception(ResponseMessageEnum.UpdateError.GetEnumDescription());
                    }

                    this.Result.SetMessage(ResponseMessageEnum.ReSetPassWordVerificationCode.GetEnumDescription());

                    return this.Result;
                }
            }

            this.Result.SetErrorMode().SetMessage(ResponseMessageEnum.AccountNameEmailNotExist.GetEnumDescription());

            return this.Result;
        }

        /// <summary>
        /// 重置密碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        /// <returns>重置結果</returns>
        public Result ResetPassWord(string accountName, string passWord)
        {
            Account item = this.GetAccountData(accountName);

            item.PassWord = Md5Hash.GetMd5Hash(passWord);

            bool isInsertSuccess = this.AccountRepository.Update(item);

            if (!isInsertSuccess)
            {
                throw new Exception(ResponseMessageEnum.UpdateError.GetEnumDescription());
            }

            this.Result.SetMessage(ResponseMessageEnum.ReSetPassWordSuccess.GetEnumDescription());

            return this.Result;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns>登出結果</returns>
        public Result SignOut(string account)
        {
            this.RedisService.DeleteAccountLoginDate(account);
            this.Result.SetMessage(ResponseMessageEnum.SignOutSucces.GetEnumDescription());

            return this.Result;
        }

        /// <summary>
        /// 使用者 超過30分鐘沒有動作 則強制登出
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns>回傳結果</returns>
        public Result CheckUserLoginTimeout(string account)
        {
            DateTime userLoginDate = this.RedisService.GetAccountLoginDate(account);

            if (!userLoginDate.Equals(new DateTime()))
            {
                if (DateTime.Compare(userLoginDate.AddMinutes(-30), DateTime.Now) == -1)
                {
                    this.Result.SetMessage(ResponseMessageEnum.LoginTimeout.GetEnumDescription());
                }
            }

            return this.Result;
        }

        /// <summary>
        /// 檢查帳號可否使用
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns>檢查結果</returns>
        private (bool, List<string>) CheckAccountCanUse(AccountViewModel account)
        {
            List<string> errorMessage = new List<string>();
            if (account != null)
            {
                bool isAccountExist = this.GetAccountData(account.AccountName) != null;

                bool isMailExist = this.AccountRepository
                                       .Get("WHERE Email = @Email", new { account.Email })
                                       .FirstOrDefault() != null;

                if (isAccountExist)
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
        /// <param name="account">帳號</param>
        private void SignupAccount(AccountViewModel account)
        {
            this.SendMail(account, MailEnum.AccountSignupVerificationCode);

            Account entity = new Account()
            {
                AccountName = account.AccountName,
                PassWord = Md5Hash.GetMd5Hash(account.PassWord),
                Email = account.Email,
                SignupDate = DateTime.Now,
                SendMailDate = DateTime.Now,
                VerificationCode = account.VerificationCode
            };

            bool isInsertSuccess = this.AccountRepository.Add(entity);

            if (!isInsertSuccess)
            {
                throw new Exception(ResponseMessageEnum.InsertError.GetEnumDescription());
            }
        }

        /// <summary>
        /// 取得帳號資料
        /// </summary>
        /// <param name="accountName">帳號名稱</param>
        /// <returns>帳號資料</returns>
        private Account GetAccountData(string accountName)
        {
            return this.AccountRepository
                       .Get("WHERE AccountName = @AccountName", new { AccountName = accountName }).FirstOrDefault();
        }

        /// <summary>
        /// 寄信
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <param name="mailEnum">信件類型</param>
        private void SendMail(AccountViewModel accountData, MailEnum mailEnum)
        {
            // 產生驗證碼
            accountData.VerificationCode = StringExtension.GeneratorAuthCode();

            // 設定信件標題
            EmailTemplateDto emailTemplateDto = new EmailTemplateDto
            {
                Title = MailEnum.AccountSignupVerificationCode.GetEmailValue("Title")
            };

            SendEmailDto emailDto = new SendEmailDto();
            emailDto.To.Add(accountData.Email);
            emailDto.ViewFile = mailEnum.GetEmailValue();

            this.MailInfoService.SendMail(emailTemplateDto, emailDto, accountData);
        }
    }
}