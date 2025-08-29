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
        public int AccountId { get; set; }

        public Khoa? Khoa { get; set; }
        public ICollection<HuongDan>? HuongDans { get; set; }
        public Account Account { get; set; }
        public ICollection<DeTai>? DeTais { get; set; }
    }
}
