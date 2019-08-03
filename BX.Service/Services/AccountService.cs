using BX.Repository;
using BX.Repository.Base;
using BX.Repository.Entity;

namespace BX.Service
{
    /// <summary>
    /// 帳號服務
    /// </summary>
    public class AccountService : BaseService, IAccountService
    {
        /// <summary>
        /// 帳號儲存庫
        /// </summary>
        private GenericRepository<AccountData> AccountDataRepository;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="repositoryFactory"></param>
        /// <param name="dbConnection"></param>
        public AccountService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
            this.AccountDataRepository = this.CreateService<AccountData>();
        }

        /// <summary>
        /// 測試
        /// </summary>
        /// <returns></returns>
        public AccountData Test()
        {
            return this.AccountDataRepository.Get("WHERE Account = @Account", new { Account = "w4560000" });
        }
    }
}
