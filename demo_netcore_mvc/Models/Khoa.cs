using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("Khoa")]
    public class Khoa
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public string? DienThoai { get; set; }

        public ICollection<GiangVien>? GiangViens { get; set; }
        public ICollection<SinhVien>? SinhViens { get; set; }
    }
}
