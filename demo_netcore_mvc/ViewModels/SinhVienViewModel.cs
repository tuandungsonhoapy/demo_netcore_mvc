using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.ViewModels
{
    public class SinhVienViewModel
    {
        public int MaGV { get; set; }

        public string? MaDT { get; set; }

        public string? MaKhoa { get; set; }

        public List<SinhVien> SinhViens { get; set; }
    }
}
