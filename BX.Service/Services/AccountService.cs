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
        /// <summary>
        /// 帳號儲存庫
        /// </summary>
        private readonly GenericRepository<Account> AccountRepository;

        /// <summary>
        /// 信件內容設定服務
        /// </summary>
        private readonly IMailInfoService MailInfoService;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="repositoryFactory">儲存庫</param>
        /// <param name="mailInfoService">信件內容設定服務</param>
        public AccountService(
            IRepositoryFactory repositoryFactory,
            IMailInfoService mailInfoService) : base(repositoryFactory)
        {
            this.AccountRepository = this.CreateService<Account>();
            this.MailInfoService = mailInfoService;
        }

        /// <summary>
        /// 註冊帳號流程
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns></returns>
        public Result<string> SignupAccountProcess(AccountViewModel account)
        {
            Result<string> result = new Result<string>();

            (bool canUse, List<string> message) = this.CheckAccountCanUse(account);

            if (canUse)
            {
                this.SignupAccount(account);
                result.SetMessage(ResponseMessageEnum.SignupSuccess.GetEnumDescription());
            }
            else
            {
                message.ForEach(x => result.SetError(x));

                return result;
            }

            return result;
        }

        /// <summary>
        /// 驗證使用者輸入的驗證碼是否正確，完成註冊程序
        /// </summary>
        /// <param name="accountName">帳號名稱</param>
        /// <param name="verificationCode">四位驗證碼</param>
        /// <returns>驗證結果</returns>
        public Result<string> CheckVerificationCode(string accountName, string verificationCode)
        {
            Result<string> result = new Result<string>();
            var item = this.AccountRepository.Get("WHERE AccountName = @accountName", new { accountName }).FirstOrDefault();

            if (item.VerificationCode.Equals(verificationCode))
            {
                item.SignupFinish = true;
                bool isUpdateSuccess = this.AccountRepository.Update(item);

                if (isUpdateSuccess)
                    result.SetMessage(ResponseMessageEnum.ValidateSuccess.GetEnumDescription());
                else
                    result.SetError(ResponseMessageEnum.UpdateError.GetEnumDescription());
            }
            else
                result.SetError(ResponseMessageEnum.ValidateFail.GetEnumDescription());

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
                                     .Get("WHERE AccountName = @AccountName", new { account.AccountName })
                                     .FirstOrDefault() != null;

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
        /// <param name="AccountData"></param>
        private void SignupAccount(AccountViewModel account)
        {
            this.SendMail(account, MailEnum.AccountSignupVerificationCode);

            Account entity = new Account()
            {
                AccountName = account.AccountName,
                PassWord = Md5Hash.GetMd5Hash(account.PassWord),
                Email = account.Email,
                SignupDate = DateTime.Now,
                SendMailDate = DateTime.Now
            };

            bool isInsertSuccess = this.AccountRepository.Add(entity);

            if (!isInsertSuccess)
            {
                throw new Exception(ResponseMessageEnum.InsertError.GetEnumDescription());
            }
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
