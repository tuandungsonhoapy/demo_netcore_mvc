using demo_netcore_mvc.Configs.ChartConfig;
using demo_netcore_mvc.ObjectData;
using System.Collections;

namespace demo_netcore_mvc.ViewModels
{
    public class DashboardViewModel
    {
        public DonutChartConfig DeTaiCountByKhoaConfig { get; set; }
        public PieChartConfig PieChartConfig { get; set; }
        public ColumnChartConfig<ThongKeSoLuongSlotDeTaiKhoa> ColumnChartConfig { get; set; }
        public IEnumerable DoughnutChartData { get; set; }
        public string MaKhoa { get; set; }
        public string NamHoc { get; set; }
        public byte? HocKy { get; set; }
        public int TongKinhPhi { get; set; }
        public int TongSVThamGia { get; set; }
    }
}
