using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.ViewModels
{
    public class DeTaiViewModel
    {
        public int? MaGV { get; set; }
        public string? MaKhoa { get; set; }
        public string? NamHoc { get; set; }
        public byte? HocKy { get; set; }
        public List<DeTai> DeTais { get; set; }
    }
}
