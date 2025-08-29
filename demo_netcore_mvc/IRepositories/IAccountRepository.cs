using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;

namespace demo_netcore_mvc.IRepositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        public Task<Account?> GetByUsername(string username);

        public Task Register(Account_Register_Body body);
    }
}
