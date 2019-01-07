using System;
using System.Collections.Generic;

namespace WebAPI_DEMO.Model.Table
{
    public partial class AccountData
    {
        public string Account { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
        public DateTime SignupDate { get; set; }
        public DateTime? SendmailDate { get; set; }
        public string VerificationCode { get; set; }
        public string SignupFinish { get; set; }
        public DateTime? SigninDate { get; set; }
    }
}
