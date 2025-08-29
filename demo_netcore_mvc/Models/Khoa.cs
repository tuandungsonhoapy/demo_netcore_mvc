using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("Khoa")]
    public class Khoa
    {
        [Display(Name = "Mã khoa")]
        public string MaKhoa { get; set; }

        [Display(Name = "Tên khoa")]
        public string TenKhoa { get; set; }

        [Display(Name = "Điện thoại")]
        public string? DienThoai { get; set; }

        public ICollection<GiangVien>? GiangViens { get; set; }
        public ICollection<SinhVien>? SinhViens { get; set; }
    }
}
