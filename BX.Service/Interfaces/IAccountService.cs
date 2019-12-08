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
        /// <returns>確認結果</returns>
        Result ReSendEmailForReSetPassWord(AccountViewModel accountViewModel);

        /// <summary>
        /// 重置密碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="PassWord">密碼</param>
        /// <returns>重置結果</returns>
        Result ResetPassWord(string accountName, string passWord);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns>登出結果</returns>
        public Result SignOut(string account);

        /// <summary>
        /// 使用者 超過30分鐘沒有動作 則強制登出
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns>回傳結果</returns>
        Result CheckUserLoginTimeout(string account);
    }
}