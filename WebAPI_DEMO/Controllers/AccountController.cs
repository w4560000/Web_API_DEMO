using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI_DEMO.Model;
using WebAPI_DEMO.Model.Table;
using WebAPI_DEMO.ViewModel;

namespace WebAPI_DEMO.Controllers
{
    
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
        public string SignupAccount(AccountData AccountData )
        {
            string ErrorMessage = "";
            if (this._AccountService.CheckAccountCanUse(AccountData.Account))
            {
                if (this._AccountService.CheckEmailCanUse(AccountData.Email))
                {
                    this._AccountService.SignupAccount(AccountData);
                    this._AccountService.SendMail(AccountData.Email, "signup");
                        
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
                    if (this._AccountService.CheckSignupFinish(request.Account))
                    {
                        return Ok(new
                        {
                            Message = "登入成功！",
                            JWT = this._AccountService.ResponseJWT(request.Account)
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Message = "您的信箱尚未完成驗證程序！",
                            JWT = ""
                        });
                    }
                }
            }
            return Ok(new
            {
                Message = "登入失敗！　帳號密碼錯誤！",
                JWT = ""
            });
        }

        [HttpPost("check_reset_PassWord_or_resendEmail")]
        public IActionResult check_reset_PassWord_or_resendEmail(SendMailViewModel data)
        {
            if (this._AccountService.CheckAccount_Email_for_reset_PassWord_or_resendEmail(data.Account, data.Email) == "正確")
            {
                this._AccountService.SendMail(data.Email, data.dosomething);
                if (data.dosomething == "reset_password")
                    return Ok("請輸入驗證碼以重設密碼！");
                else
                    return Ok("驗證信已重寄，請確認!");
            }
            else
                return Ok(this._AccountService.CheckAccount_Email_for_reset_PassWord_or_resendEmail(data.Account, data.Email));
        }


        /// <summary>
        /// 註冊成功後直接給JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns></returns>
        [HttpPost("ResponseJWT")]
        public IActionResult ResponseJWT(AccountData AccountData)
        {
            return Ok( new { jwt = this._AccountService.ResponseJWT(AccountData.Account) });
        }


        [HttpPost("ResetPassWord")]
        public IActionResult ResetPassWord(AccountData AccountData)
        {
            this._AccountService.ResetPassWord(AccountData.Account, AccountData.PassWord);
            return Ok("重設密碼成功！");
        }
        [HttpPost("LogOut")]
        public IActionResult LogOut(AccountData AccountData)
        {
            _AccountService.LogOut(AccountData.Account);
            return Ok("登出成功！");
        }

        [Authorize(Policy = "User")]
        [HttpPost("UpLoadImage")]
        public IActionResult UpLoadImage(UploadImage uploadImage)
        {
            return Ok(_AccountService.UpLoadImage(uploadImage.Account, uploadImage.base64data));
        }

        [Authorize(Policy = "User")]
        [HttpPost("GetImage")]
        public IActionResult GetImage(UploadImage uploadImage)
        {
            return Ok(_AccountService.GetImage(uploadImage.Account));
        }


    }
}
