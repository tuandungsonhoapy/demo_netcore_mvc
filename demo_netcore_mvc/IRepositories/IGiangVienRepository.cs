using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.IRepositories
{
    public interface IGiangVienRepository : IBaseRepository<Models.GiangVien>
    {
        public Task<GiangVien?> GetByAccountId(int accountId);
    }
}
