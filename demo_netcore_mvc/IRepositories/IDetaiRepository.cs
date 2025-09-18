using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
using demo_netcore_mvc.RequestData;

namespace demo_netcore_mvc.IRepositories
{
    public interface IDetaiRepository : IBaseRepository<DeTai>
    {
        public Task<List<NamHocData>> GetAllNamHoc();

        public Task<List<DeTai>> GetSVThamGiaTheoKy(int MaSV, string NamHoc, byte HocKy);

        public Task<List<DeTai>> MyDeTai(DeTai_MyDeTai_Queries requestParams);

        public Task<List<DeTai>> GetLatestHocKyDeTais(Dashboard_Params pars);

        public Task<List<DoughnutChartData>> GetDeTaiCountByKhoa(Dashboard_Params pars);
    }
}
