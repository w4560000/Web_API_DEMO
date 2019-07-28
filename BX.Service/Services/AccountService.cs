using BX.Repository;
using BX.Repository.Base;
using BX.Repository.Entity;

namespace BX.Service
{
    public class SAccountService : BaseService, ISAccountService
    {
        //private IGenericRepository<AccountData> GenericRepository;
        //private IAccountRepository AccountRepository;
        //public SAccountService(IGenericRepository<AccountData> genericRepository)
        //{
        //    this.GenericRepository = genericRepository;
        //}
        private GenericRepository<AccountData> GenericRepository;
        public SAccountService(IRepositoryFactory repositoryFactory, ISQLServerConnectionBase dbConnection) : base(repositoryFactory, dbConnection)
        {
            this.GenericRepository = this.CreateService<AccountData>();
        }

        public AccountData Test()
        {
            return this.GenericRepository.Get("WHERE Account = @Account", new { Account = "w4560000" });
        }
    }
}
