using System.ComponentModel.DataAnnotations.Schema;

namespace demo_netcore_mvc.Models
{
    [Table("DeTai")]
    public class DeTai
    {
        public string MaDT { get; set; }
        public string TenDT { get; set; }
        public int KinhPhi { get; set; }
        public string NoiThucTap { get; set; }

        public ICollection<HuongDan> HuongDans { get; set; }
    }
}
