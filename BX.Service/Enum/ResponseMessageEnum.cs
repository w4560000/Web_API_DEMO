using System.ComponentModel;

namespace BX.Service
{
    /// <summary>
    /// 回應錯誤訊息列舉
    /// </summary>
    public enum ResponseMessageEnum
    {
        /// <summary>
        /// 帳號驗證碼驗證成功
        /// </summary>
        [ResponseStatus("001")]
        [Description("驗證成功！")]
        ValidateSuccess,

        /// <summary>
        /// 帳號註冊成功
        /// </summary>
        [ResponseStatus("002")]
        [Description("註冊成功！")]
        SignupSuccess,

        /// <summary>
        /// 帳號登入成功
        /// </summary>
        [ResponseStatus("003")]
        [Description("登入成功！")]
        SigninSuccess,

        /// <summary>
        /// 帳號登出成功
        /// </summary>
        [ResponseStatus("004")]
        [Description("登出成功！")]
        SignOutSucces,

        /// <summary>
        /// 帳號重設密碼成功
        /// </summary>
        [ResponseStatus("005")]
        [Description("重設密碼成功！")]
        ReSetPassWordSuccess,

        /// <summary>
        /// 帳號驗證信重寄成功
        /// </summary>
        [ResponseStatus("006")]
        [Description("請輸入驗證碼後重設密碼。")]
        ReSetPassWordVerificationCode,

        /// <summary>
        /// 帳號登入失敗
        /// </summary>
        [ResponseStatus("011")]
        [Description("登入失敗！請確認帳號密碼是否正確！")]
        SigninFail,

        /// <summary>
        /// 帳號登入失敗
        /// </summary>
        [ResponseStatus("012")]
        [Description("登入失敗！信箱未認證！")]
        EmailUnAuthentication,

        /// <summary>
        /// 帳號註冊失敗
        /// </summary>
        [ResponseStatus("021")]
        [Description("該帳號名稱已被使用！")]
        AccountNameUsed,

        /// <summary>
        /// 帳號註冊失敗
        /// </summary>
        [ResponseStatus("022")]
        [Description("帳號名稱不能為空！")]
        AccountNameIsNull,

        /// <summary>
        /// 帳號註冊失敗
        /// </summary>
        [ResponseStatus("023")]
        [Description("該信箱已被使用！")]
        EmailUsed,

        /// <summary>
        /// 重寄驗證信失敗 帳號信箱錯誤
        /// </summary>
        [ResponseStatus("031")]
        [Description("帳號或信箱輸入錯誤！")]
        AccountNameEmailNotExist,

        /// <summary>
        /// 重寄驗證信失敗 帳號驗證碼驗證失敗
        /// </summary>
        [ResponseStatus("032")]
        [Description("驗證失敗！請確認驗證碼是否輸入正確！")]
        ValidateFail,

        /// <summary>
        /// 資料庫新增失敗
        /// </summary>
        [ResponseStatus("100")]
        [Description("新增失敗！")]
        InsertError,

        /// <summary>
        /// 資料庫更新失敗
        /// </summary>
        [ResponseStatus("100")]
        [Description("更新失敗！")]
        UpdateError,
    }
}