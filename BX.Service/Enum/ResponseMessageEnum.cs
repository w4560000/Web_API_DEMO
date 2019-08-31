using System.ComponentModel;

namespace BX.Service
{
    /// <summary>
    /// 回應錯誤訊息列舉
    /// </summary>
    public enum ResponseMessageEnum
    {
        /// <summary>
        /// 帳號名稱已被使用
        /// </summary>
        [Description("該帳號名稱已被使用！")]
        AccountNameUsed,

        /// <summary>
        /// 帳號名稱為空
        /// </summary>
        [Description("帳號名稱不能為空！")]
        AccountNameIsNull,

        /// <summary>
        /// 信箱已被使用
        /// </summary>
        [Description("該信箱已被使用！")]
        EmailUsed,

        [Description("帳號或信箱輸入錯誤！")]
        AccountNameEmailNotExist,

        /// <summary>
        /// 資料庫新增失敗
        /// </summary>
        [Description("新增失敗！")]
        InsertError,

        /// <summary>
        /// 資料庫更新失敗
        /// </summary>
        [Description("更新失敗！")]
        UpdateError,

        /// <summary>
        /// 帳號註冊成功
        /// </summary>
        [Description("註冊成功！")]
        SignupSuccess,

        /// <summary>
        /// 帳號驗證碼驗證成功
        /// </summary>
        [Description("驗證成功！")]
        ValidateSuccess,

        /// <summary>
        /// 帳號驗證碼驗證失敗
        /// </summary>
        [Description("驗證失敗！請確認驗證碼是否輸入正確！")]
        ValidateFail,

        [Description("登入成功！")]
        SigninSuccess,

        [Description("登入失敗！請確認帳號密碼是否正確！")]
        SigninFail,

        [Description("登入失敗！信箱未認證！")]
        EmailUnAuthentication,

        [Description("驗證信已重寄，請輸入驗證碼後重設密碼。")]
        ReSetPassWordVerificationCode,

        [Description("重設密碼成功！")]
        ReSetPassWordSuccess,
    }
}