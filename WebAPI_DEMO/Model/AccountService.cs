using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Model
{
    public class AccountService :IAccountService
    {
        private FOR_VUEContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public AccountService(FOR_VUEContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<AccountData> GetAccountData()
        {
            var result = this._dbContext.AccountData.ToList();
            return result;
        }

        public void SignupAccount(AccountData AccountData)
        {
            var NEWAccountData = new AccountData()
            {
                Account = AccountData.Account,
                PassWord = AccountData.PassWord,
                Email = AccountData.Email,
                SignupDate = DateTime.Now
            };

            this._dbContext.AccountData.Add(NEWAccountData);
            this._dbContext.SaveChanges();
        }
        //註冊帳號時，先檢查帳號是否重複
        public bool CheckAccountCanUse(string Account)
        {
            if (Account != null || Account != "")
            {
                var result = this._dbContext.AccountData.Where(r => r.Account == Account).FirstOrDefault();

                if (result == null)
                    return true;
            }

            return false;
        }
        //註冊帳號時，先檢查mail是否重複
        public bool CheckEmailCanUse(string Email)
        {
            if (Email != null || Email != "")
            {
                var result = this._dbContext.AccountData.Where(r => r.Email == Email).FirstOrDefault();

                if (result == null)
                    return true;
            }

            return false;
        }
    }
}
