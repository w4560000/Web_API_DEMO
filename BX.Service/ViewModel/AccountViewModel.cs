namespace BX.Service.ViewModel
{
    /// <summary>
    /// 帳號ViewModel
    /// </summary>
    public class AccountViewModel
    {
        /// <summary>
        /// 帳號名稱
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// 驗證信重寄的類型 Ex: 重設密碼 or 重寄驗證季
        /// </summary>
        public MailEnum ResendMailType { get; set; }
    }
}
