using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.IRepositories
{
    public interface IHuongDanRepository : IBaseRepository<Models.HuongDan>
    {
        public Task<HuongDan> GetListByDeTai(int MaDT);
    }
}
