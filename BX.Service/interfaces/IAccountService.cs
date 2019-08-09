using BX.Repository.Entity;
using BX.Service.ViewModel;

namespace BX.Service
{
    public interface IAccountService
    {

        /// <summary>
        /// 註冊帳號流程
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns></returns>
        Result<string> SignupAccountProcess(AccountViewModel account);

        ////取得帳號基本資料
        //AccountData GetAccountData(string Account);



        ////確認驗證碼是否正確
        //string CheckVerificationCode(string Account, string ValidationCode);

        ////建立驗證碼
        //int[] CreateValidationCode();

        ////登入驗證
        //bool SigninValidation(string Account, string PassWord);

        ////確認信箱是否認證過驗證碼
        //bool CheckSignupFinish(string Account);

        ////確認帳號與信箱是否是同一人所有
        //string CheckAccount_Email_for_reset_PassWord_or_resendEmail(string Account, string Email);

        ////重置密碼
        //void ResetPassWord(string Account, string PassWord);

        ////回傳JWT
        //string ResponseJWT(string Account);

        ////登出
        //void LogOut(string Account);

        ////上傳大頭照
        //string UpLoadImage(string Account, string base64data);

        ////回傳大頭照至前端
        //string GetImage(string Account);
    }
}
