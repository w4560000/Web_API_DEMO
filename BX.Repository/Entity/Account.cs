using System;
using static BX.Repository.Base.SimpleCRUD;

namespace BX.Repository.Entity
{
    /// <summary>
    /// 帳號Entity
    /// </summary>
    [Table("Account")]
    public class Account
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [Column("AccountId")]
        public int Id { get; set; }

        /// <summary>
        /// 帳號名稱
        /// </summary>
        [Column("AccountName")]
        public string AccountName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Column("PassWord")]
        public string PassWord { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        [Column("Email")]
        public string Email { get; set; }

        /// <summary>
        /// 註冊時間
        /// </summary>
        [Column("SigupDate")]
        public DateTime SignupDate { get; set; }

        /// <summary>
        /// 寄送驗證信時間
        /// </summary>
        [Column("SendMailDate")]
        public DateTime? SendMailDate { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        [Column("VerificationCode")]
        public string VerificationCode { get; set; }

        /// <summary>
        /// 是否註冊成功
        /// </summary>
        [Column("SignupFinish")]
        public bool SignupFinish { get; set; } = false;

        /// <summary>
        /// 登入時間
        /// </summary>
        [Column("SigninDate")]
        public DateTime? SigninDate { get; set; }
    }
}
