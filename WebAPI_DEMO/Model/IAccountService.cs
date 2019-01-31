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
        AccountData GetAccountData(string Account);

        //註冊時，確認帳號是否已被使用
        bool CheckAccountCanUse(string Account);

        //註冊時，確認Email是否已被使用
        bool CheckEmailCanUse(string Email);

        //註冊帳號
        void SignupAccount(AccountData AccountData);

        //取得MD5加密過的密碼
        string GetMD5PassWord(string PassWord);

        //註冊帳號成功後，寄送驗證碼
        void SendMail(string Email,string dosomeing);

        //確認驗證碼是否正確
        string CheckVerificationCode(string Account, string ValidationCode);

        //建立驗證碼
        int[] CreateValidationCode();

        //登入驗證
        bool SigninValidation(string Account, string PassWord);

        //確認信箱是否認證過驗證碼
        bool CheckSignupFinish(string Account);

        //確認帳號與信箱是否是同一人所有
        string CheckAccount_Email_for_reset_PassWord_or_resendEmail(string Account, string Email);

        //重置密碼
        void ResetPassWord(string Account, string PassWord);

        //回傳JWT
        string ResponseJWT(string Account);

        //登出
        void LogOut(string Account);

        //上傳大頭照
        string UpLoadImage(string Account, string base64data);

        //回傳大頭照至前端
        string GetImage(string Account);
    }
}
