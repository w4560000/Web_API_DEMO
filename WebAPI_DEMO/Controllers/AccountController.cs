using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        // GET api/values
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
    }
}
