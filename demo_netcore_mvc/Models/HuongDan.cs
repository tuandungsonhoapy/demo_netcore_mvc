using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("HuongDan")]
    public class HuongDan
    {
        public int MaSV { get; set; }
        public string MaDT { get; set; }
        public int MaGV { get; set; }
        public decimal? KetQua { get; set; }

        public SinhVien SinhVien { get; set; }
        public DeTai DeTai { get; set; }
        public GiangVien GiangVien { get; set; }
    }
}
