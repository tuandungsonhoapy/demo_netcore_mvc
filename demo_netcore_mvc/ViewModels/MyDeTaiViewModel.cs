using demo_netcore_mvc.Models;

namespace demo_netcore_mvc.ViewModels
{
    public class MyDeTaiViewModel
    {
        public int? MaSV { get; set; }
        public string? NamHoc { get; set; }
        public byte? HocKy { get; set; }
        public List<DeTai> DeTais { get; set; }
    }
}
