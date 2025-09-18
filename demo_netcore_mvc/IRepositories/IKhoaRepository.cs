using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;

namespace demo_netcore_mvc.IRepositories
{
    public interface IKhoaRepository : IBaseRepository<Khoa>
    {
        public Task<List<Khoa>> GetAllWithStudents(Dashboard_Params pars);
    }
}
