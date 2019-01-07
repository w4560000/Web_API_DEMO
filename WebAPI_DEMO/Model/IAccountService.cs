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

        void SignupAccount(AccountData AccountData);

        bool CheckAccountCanUse(string Account);

        bool CheckEmailCanUse(string Email);
    }
}
