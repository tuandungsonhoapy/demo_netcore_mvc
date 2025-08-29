using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;

namespace demo_netcore_mvc.IRepositories
{
    public interface IDetaiRepository : IBaseRepository<DeTai>
    {
        public Task<List<NamHocData>> GetAllNamHoc();

        public Task<List<SinhVien_DeTai_HK_Data>> GetSVThamGiaTheoKy(int MaSV, string NamHoc, byte HocKy);
    }
}
