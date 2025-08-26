using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("GiangVien")]
    public class GiangVien
    {
        public int MaGV { get; set; }
        public string HoTenGV { get; set; }
        public decimal Luong { get; set; }
        public string MaKhoa { get; set; }

        public Khoa? Khoa { get; set; }
        public ICollection<HuongDan>? HuongDans { get; set; }
    }
}
