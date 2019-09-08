using BX.Service.ViewModel;

namespace BX.Service
{
    public interface IAccountService
    {
        /// <summary>
        /// 註冊帳號流程
        /// </summary>
        /// <param name="AccountData"></param>
        /// <returns>註冊結果</returns>
        Result SignupAccountProcess(AccountViewModel account);

        // <summary>
        /// 驗證使用者輸入的驗證碼是否正確，完成註冊程序
        /// </summary>
        /// <param name="accountName">帳號</param>
        /// <param name="verificationCode">四位驗證碼</param>
        /// <returns>驗證結果</returns>
        Result CheckVerificationCode(string accountName, string verificationCode);

        /// <summary>
        /// 帳號登入，驗證帳密
        /// </summary>
        /// <param name="account">帳號資訊</param>
        /// <returns>登入結果</returns>
        Result Signin(AccountViewModel account);

        /// <summary>
        /// 確認帳號與信箱是否是同一人所有
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Email">信箱</param>
        /// <returns>結果</returns>
        Result ReSendEmailForReSetPassWord(AccountViewModel accountViewModel);

        /// <summary>
        /// 重置密碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        Result ResetPassWord(string accountName, string passWord);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Account">帳號</param>
        public Result SignOut(string account);

        ////上傳大頭照
        //string UpLoadImage(string Account, string base64data);

        ////回傳大頭照至前端
        //string GetImage(string Account);
    }
}