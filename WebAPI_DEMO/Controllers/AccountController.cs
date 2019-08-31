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
        /// Jwt服務
        /// </summary>
        private readonly IJwtService JwtService;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="AccountService">帳號服務</param>
        public AccountController(
            IAccountService accountService,
            IJwtService jwtService)
        {
            this.AccountService = accountService;
            this.JwtService = jwtService;
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
                apiResult = ConvertHelper.CovertToApiResponse(this.AccountService.SignupAccountProcess(accountData));

                return apiResult;
            }
            catch (Exception ex)
            {
                apiResult.Result.Add(ex.Message);

                return apiResult;
            }
        }

        /// <summary>
        /// (註冊)確認驗證碼是否正確
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("CheckVerificationCodeForSignUp")]
        public ApiResponseViewModel<List<string>> CheckVerificationCodeForSignUp(AccountViewModel accountData)
        {
            Result result = this.AccountService.CheckVerificationCode(accountData.AccountName, accountData.VerificationCode);
            ApiResponseViewModel<List<string>> apiResult = ConvertHelper.CovertToApiResponse(result);

            if (apiResult.IsSuccess)
            {
                apiResult.JWT = this.JwtService.ResponseJWT(accountData.AccountName);
            }

            return apiResult;
        }

        /// <summary>
        /// (重設密碼)確認驗證碼是否正確
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("CheckVerificationCodeForReSetPassWord")]
        public ApiResponseViewModel<List<string>> CheckVerificationCodeForReSetPassWord(AccountViewModel accountData)
        {
            Result result = this.AccountService.CheckVerificationCode(accountData.AccountName, accountData.VerificationCode);
            ApiResponseViewModel<List<string>> apiResult = ConvertHelper.CovertToApiResponse(result);

            return apiResult;
        }

        /// <summary>
        /// 登入驗證帳號密碼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Signin")]
        public ApiResponseViewModel<List<string>> Signin(AccountViewModel accountData)
        {
            Result result = this.AccountService.Signin(accountData);
            ApiResponseViewModel<List<string>> apiResult = ConvertHelper.CovertToApiResponse(result);

            if (apiResult.IsSuccess)
            {
                apiResult.JWT = this.JwtService.ResponseJWT(accountData.AccountName);
            }

            return apiResult;
        }

        /// <summary>
        /// 重寄驗證信用以重設密碼
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("ReSendEmailForReSetPassWord")]
        public ApiResponseViewModel<List<string>> ReSendEmailForReSetPassWord(AccountViewModel accountData)
        {
            Result result = this.AccountService.ReSendEmailForReSetPassWord(accountData);
            ApiResponseViewModel<List<string>> apiResult = ConvertHelper.CovertToApiResponse(result);

            return apiResult;
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="accountData">帳戶資訊</param>
        /// <returns></returns>
        [HttpPost("ResetPassWord")]
        public ApiResponseViewModel<List<string>> ResetPassWord(AccountViewModel accountData)
        {
            Result result = this.AccountService.ResetPassWord(accountData);
            ApiResponseViewModel<List<string>> apiResult = ConvertHelper.CovertToApiResponse(result);

            return apiResult;
        }

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