using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Model
{
    public interface IAccountService
    {
        List<AccountData> GetAccountData();

        bool CheckAccountCanUse(string Account);

        bool CheckEmailCanUse(string Email);

        void SignupAccount(AccountData AccountData);

        void SendMail(string Email);

        string  SignupFinish(string Account, string ValidationCode);

        int[] CreateValidationCode();

    }
}
