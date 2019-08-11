﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// Email 列舉 
    /// </summary>
    public enum MailEnum
    {
        [EmailInfo(Title = "Test Mail From 鄭秉庠", Href = "Signup.txt,ValidationCode.txt")]
        AccountSignupVerificationCode,

        [EmailInfo(Title = "Test Mail From 鄭秉庠", Href = "ReSend.txt,ValidationCode.txt")]
        ReSendVerificationCode,

        [EmailInfo(Title = "Test Mail From 鄭秉庠", Href = "ReSetPassWord.txt,ValidationCode.txt")]
        ReSetPassWordVerificationCode,
    }
}
