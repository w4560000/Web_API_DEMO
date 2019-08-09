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

        /// <summary>
        /// 資料庫新增失敗
        /// </summary>
        [Description("新增失敗！")]
        InsertError,

        [Description("註冊成功！")]
        SignupSuccess,
    }
}
