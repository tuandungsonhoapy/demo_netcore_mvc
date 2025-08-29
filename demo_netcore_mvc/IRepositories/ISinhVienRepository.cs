using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.IRepositories
{
    public interface ISinhVienRepository : IBaseRepository<Models.SinhVien>
    {
        public Task<SinhVien?> GetByAccountId(int accountId);
    }
}
