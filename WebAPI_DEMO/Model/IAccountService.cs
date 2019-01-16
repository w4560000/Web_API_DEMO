using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Model
{
    public interface IAccountService
    {

        //取得帳號基本資料
        List<AccountData> GetAccountData();

        //註冊時，確認帳號是否已被使用
        bool CheckAccountCanUse(string Account);

        //註冊時，確認Email是否已被使用
        bool CheckEmailCanUse(string Email);

        //註冊帳號
        void SignupAccount(AccountData AccountData);

        //註冊帳號成功後，寄送驗證碼
        void SendMail(string Email);

        //確認驗證碼是否正確
        string CheckVerificationCode(string Account, string ValidationCode);

        //建立驗證碼
        int[] CreateValidationCode();

        //登入驗證
        bool SigninValidation(string Account, string PassWord);

        //回傳JWT
        string ResponseJWT(string Account);

        //登出
        void LogOut(string Account);

    }
}
