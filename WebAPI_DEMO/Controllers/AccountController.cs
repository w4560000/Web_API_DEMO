using BX.Service;
using BX.Service.ViewModel;
using BX.Web.Security;
using Microsoft.AspNetCore.Authorization;
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
        /// <param name="accountService">帳號服務</param>
        /// <param name="jwtService">Jwt服務</param>
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
        /// <param name="accountData">帳號資訊</param>
        /// <returns>註冊結果</returns>
        [HttpPost("SignupAccount")]
        public ApiResponseViewModel<List<string>> SignupAccount(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = new ApiResponseViewModel<List<string>>();
            try
            {
                apiResult = this.AccountService.SignupAccountProcess(accountData).CovertToApiResponse();

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
        /// <param name="accountData">帳號資訊</param>
        /// <returns>確認結果</returns>
        [HttpPost("CheckVerificationCodeForSignUp")]
        public ApiResponseViewModel<List<string>> CheckVerificationCodeForSignUp(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.CheckVerificationCode(accountData.AccountName, accountData.VerificationCode)
                                                                              .CovertToApiResponse();

            if (apiResult.IsSuccess)
            {
                apiResult.JWT = this.JwtService.ResponseJWT(accountData.AccountName);
            }

            return apiResult;
        }

        /// <summary>
        /// (重設密碼)確認驗證碼是否正確
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>確認結果</returns>
        [HttpPost("CheckVerificationCodeForReSetPassWord")]
        public ApiResponseViewModel<List<string>> CheckVerificationCodeForReSetPassWord(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.CheckVerificationCode(accountData.AccountName, accountData.VerificationCode)
                                                                              .CovertToApiResponse();

            return apiResult;
        }

        /// <summary>
        /// 登入驗證帳號密碼
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>登入結果</returns>
        [ReFreshJwtFilter]
        [HttpPost("Signin")]
        public ApiResponseViewModel<List<string>> Signin(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.Signin(accountData).CovertToApiResponse();

            return apiResult;
        }

        /// <summary>
        /// 重寄驗證信用以重設密碼
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>結果</returns>
        [HttpPost("ReSendEmailForReSetPassWord")]
        public ApiResponseViewModel<List<string>> ReSendEmailForReSetPassWord(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.ReSendEmailForReSetPassWord(accountData).CovertToApiResponse();

            return apiResult;
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>重設結果</returns>
        [HttpPost("ResetPassWord")]
        public ApiResponseViewModel<List<string>> ResetPassWord(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.ResetPassWord(accountData.AccountName, accountData.PassWord).CovertToApiResponse();

            return apiResult;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>登出結果</returns>
        [HttpPost("Logout")]
        public ApiResponseViewModel<List<string>> LogOut(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = this.AccountService.SignOut(accountData.AccountName).CovertToApiResponse();

            return apiResult;
        }

        /// <summary>
        /// FB登入的話直接給Jwt
        /// </summary>
        /// <param name="accountData">帳號資訊</param>
        /// <returns>Jwt</returns>
        [HttpPost("ResponseJwt")]
        public ApiResponseViewModel<List<string>> ResponseJwt(AccountViewModel accountData)
        {
            ApiResponseViewModel<List<string>> apiResult = new ApiResponseViewModel<List<string>>()
            {
                IsSuccess = true,
                JWT = this.JwtService.ResponseJWT(accountData.AccountName)
            };

            return apiResult;
        }

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