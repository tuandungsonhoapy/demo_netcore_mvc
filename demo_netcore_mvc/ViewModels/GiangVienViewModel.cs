using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.ViewModels
{
    public class GiangVienViewModel
    {
        public string? MaKhoa { get; set; }

        public List<GiangVien> GiangViens { get; set; }
    }
}
