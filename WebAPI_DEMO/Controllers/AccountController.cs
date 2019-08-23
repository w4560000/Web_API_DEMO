using BX.Service;
using BX.Service.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BX.Web.Controllers
{
    /// <summary>
    /// 帳號 API Controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 帳號服務
        /// </summary>
        private readonly IAccountService AccountService;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="AccountService">帳號服務</param>
        public AccountController(
            IAccountService AccountService)
        {
            this.AccountService = AccountService;
        }

        /// <summary>
        /// 註冊帳號並寄送驗證碼
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("SignupAccount")]
        public ApiResponseViewModel<List<string>> SignupAccount(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = new ApiResponseViewModel<List<string>>();
            try
            {
                Result<string> signupResult = this.AccountService.SignupAccountProcess(accountData);

                apiResult.Code = signupResult.Success;
                apiResult.Result = signupResult.Success.Equals((int)ResponseEnum.Success) ? signupResult.Data
                                                                                          : signupResult.ErrorMessage;

                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.Result.Add(ex.Message);

                return apiResult;
            }
        }

        /// <summary>
        /// 確認驗證碼是否正確
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("CheckVerificationCode")]
        public ApiResponseViewModel<List<string>> CheckVerificationCode(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = new ApiResponseViewModel<List<string>>();
            Result<string> result = this.AccountService.CheckVerificationCode(accountData.AccountName, accountData.VerificationCode);

            apiResult.Code = result.Success;
            apiResult.Result = result.Success.Equals((int)ResponseEnum.Success) ? result.Data
                                                                                : result.ErrorMessage;

            return apiResult;
        }

        [HttpGet("Test")]
        public string Test()
        {
            string RockPaperScissors(string first, string second)
            => (first, second) switch
            {
                ("rock", "paper") => "rock is covered by paper. Paper wins.",
                ("rock", "scissors") => "rock breaks scissors. Rock wins.",
                ("paper", "rock") => "paper covers rock. Paper wins.",
                ("paper", "scissors") => "paper is cut by scissors. Scissors wins.",
                ("scissors", "rock") => "scissors is broken by rock. Rock wins.",
                ("scissors", "paper") => "scissors cuts paper. Scissors wins.",
                (_, _) => "tie"
            };

            return RockPaperScissors(default, default);
        }

        ///// <summary>
        ///// 登入驗證帳號密碼
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost("SigninValidation")]
        //public IActionResult SigninValidation(AccountViewModel accountData)
        //{
        //    if (request != null)
        //    {
        //        if (this._AccountService.SigninValidation(request.Account, request.PassWord))
        //        {
        //            if (this._AccountService.CheckSignupFinish(request.Account))
        //            {
        //                return this.Ok(new
        //                {
        //                    Message = "登入成功！",
        //                    JWT = this._AccountService.ResponseJWT(request.Account)
        //                });
        //            }
        //            else
        //            {
        //                return this.Ok(new
        //                {
        //                    Message = "您的信箱尚未完成驗證程序！",
        //                    JWT = ""
        //                });
        //            }
        //        }
        //    }
        //    return this.Ok(new
        //    {
        //        Message = "登入失敗！　帳號密碼錯誤！",
        //        JWT = ""
        //    });
        //}

        //[HttpPost("check_reset_PassWord_or_resendEmail")]
        //public IActionResult Check_reset_PassWord_or_resendEmail(SendMailViewModel data)
        //{
        //    if (this._AccountService.CheckAccount_Email_for_reset_PassWord_or_resendEmail(data.Account, data.Email) == "正確")
        //    {
        //        this._AccountService.SendMail(data.Email, data.Dosomething);
        //        if (data.Dosomething == "reset_password")
        //            return this.Ok("請輸入驗證碼以重設密碼！");
        //        else
        //            return this.Ok("驗證信已重寄，請確認!");
        //    }
        //    else
        //        return this.Ok(this._AccountService.CheckAccount_Email_for_reset_PassWord_or_resendEmail(data.Account, data.Email));
        //}


        ///// <summary>
        ///// 註冊成功後直接給JWT
        ///// </summary>
        ///// <param name="Account">帳號</param>
        ///// <returns></returns>
        //[HttpPost("ResponseJWT")]
        //public IActionResult ResponseJWT(AccountData AccountData)
        //{
        //    return this.Ok( new { jwt = this._AccountService.ResponseJWT(AccountData.Account) });
        //}


        //[HttpPost("ResetPassWord")]
        //public IActionResult ResetPassWord(AccountData AccountData)
        //{
        //    this._AccountService.ResetPassWord(AccountData.Account, AccountData.PassWord);
        //    return this.Ok("重設密碼成功！");
        //}
        //[HttpPost("LogOut")]
        //public IActionResult LogOut(AccountData AccountData)
        //{
        //    this._AccountService.LogOut(AccountData.Account);
        //    return this.Ok("登出成功！");
        //}

        //[Authorize(Policy = "User")]
        //[HttpPost("UpLoadImage")]
        //public IActionResult UpLoadImage(UploadImage uploadImage)
        //{
        //    return this.Ok(this._AccountService.UpLoadImage(uploadImage.Account, uploadImage.Base64data));
        //}

        //[Authorize(Policy = "User")]
        //[HttpPost("GetImage")]
        //public IActionResult GetImage(UploadImage uploadImage)
        //{
        //    return this.Ok(this._AccountService.GetImage(uploadImage.Account));
        //}
    }
}
