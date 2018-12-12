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
    }
}
