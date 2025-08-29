using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("SinhVien")]
    public class SinhVien
    {
        public int MaSV { get; set; }

        [Display(Name = "Họ tên")]
        public string HoTenSV { get; set; }
        public string MaKhoa { get; set; }

        [Display(Name = "Năm sinh")]
        [DataType(DataType.Date)]
        public DateTime NamSinh { get; set; }

        [Display(Name = "Quê quán")]
        public string QueQuan { get; set; }

        public int AccountId { get; set; }

        public Khoa? Khoa { get; set; }
        public ICollection<HuongDan>? HuongDans { get; set; }
        public Account Account { get; set; }
    }
}
