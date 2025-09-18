using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("DeTai")]
    public class DeTai
    {
        [Display(Name = "Mã đề tài")]
        public string MaDT { get; set; }

        [Display(Name = "Tên đề tài")]
        public string TenDT { get; set; }

        [Display(Name = "Kinh phí")]
        public int KinhPhi { get; set; }

        [Display(Name = "Nơi thực tập")]
        public string NoiThucTap { get; set; }

        [Display(Name = "Tối đa")]
        public int? ToiDa { get; set; }

        [Display(Name = "Năm học")]
        public string? NamHoc { get; set; }

        [Display(Name = "Học kỳ")]
        public byte? HocKy { get; set; }

        [Display(Name = "Người hướng dẫn")]
        public int NguoiHuongDan { get; set; }

        [Display(Name = "Mở đăng ký")]
        public bool IsOpen { get; set; } = false;


        public string MaKhoa { get; set; }

        public ICollection<HuongDan> HuongDans { get; set; }
        public GiangVien GiangVien { get; set; }
        public Khoa Khoa { get; set; }

        [NotMapped]
        public int SoLuong => HuongDans?.Count ?? 0;
    }
}
