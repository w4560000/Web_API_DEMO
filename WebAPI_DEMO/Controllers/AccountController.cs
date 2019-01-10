using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI_DEMO.Model;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Controllers
{
    //[Authorize(Policy = "TrainedStaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IAccountService _AccountService;

        public AccountController(IAccountService AccountService)
        {
            this._AccountService = AccountService;
        }

        /// <summary>
        /// 註冊帳號並寄送驗證碼
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("SignupAccount")]
        public string SignupAccount(AccountData AccountData)
        {
            string ErrorMessage = "";
            if (this._AccountService.CheckAccountCanUse(AccountData.Account))
            {
                if (this._AccountService.CheckEmailCanUse(AccountData.Email))
                {
                    this._AccountService.SignupAccount(AccountData);
                    this._AccountService.SendMail(AccountData.Email);
                }
                else
                {
                    ErrorMessage = "該E-mail已有人使用！　請更改！";
                }
            }
            else
            {
                if (this._AccountService.CheckEmailCanUse(AccountData.Email))
                {
                    ErrorMessage = "該帳號已有人使用！　請更改！";
                }
                else
                {
                    ErrorMessage = "該帳號和E-mail已有人使用！　請更改！";
                }
            }

            if (ErrorMessage == string.Empty)
                return "註冊成功！";
            else
                return ErrorMessage;
        }

        /// <summary>
        /// 確認驗證碼是否正確
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        [HttpPost("CheckVerificationCode")]
        public string CheckVerificationCode(AccountData AccountData)
        {
            return this._AccountService.CheckVerificationCode(AccountData.Account, AccountData.VerificationCode);
        }

        /// <summary>
        /// 登入驗證帳號密碼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SigninValidation")]
         public IActionResult SigninValidation([FromBody]AccountData request)
         {
            if (request != null)
            {
                if (this._AccountService.SigninValidation(request.Account, request.PassWord))
                {
                    return Ok(new {
                        Message = "登入成功！",
                        JWT = this._AccountService.ResponseJWT(request.Account, false) });
                }
            }
                return Ok(new
                {
                    Message = "登入失敗！　帳號密碼錯誤！",
                    JWT=""
                });
            
        }

        /// <summary>
        /// 註冊成功後直接給JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        [HttpPost("ResponseJWT")]
        public IActionResult ResponseJWT(AccountData AccountData)
        {
            return Ok(this._AccountService.ResponseJWT(AccountData.Account, false));
        }

        
    }
}
