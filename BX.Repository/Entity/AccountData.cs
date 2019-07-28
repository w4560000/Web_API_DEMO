using System;
using static BX.Repository.Base.SimpleCRUD;
using Dapper;

namespace BX.Repository.Entity
{
    [Table("AccountData")]
    public class AccountData
    {
        [Key]
        [Column("Account_Id")]
        public int Account_Id { get; set; }

        [Column("Account")]
        public string Account { get; set; }

        [Column("PassWord")]
        public string PassWord { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("signup_date")]
        public DateTime SignupDate { get; set; }

        [Column("sendmail_date")]
        public DateTime? SendMailDate { get; set; }

        [Column("verification_code")]
        public string VerificationCode { get; set; }

        [Column("signup_finish")]
        public string SignupFinish { get; set; }

        [Column("signin_date")]
        public DateTime? SigninDate { get; set; }
    }
}
