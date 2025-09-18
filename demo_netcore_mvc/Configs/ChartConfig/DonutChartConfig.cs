using demo_netcore_mvc.ObjectData;

namespace demo_netcore_mvc.Configs.ChartConfig
{
    public class DonutChartConfig
    {
        public List<DoughnutChartData> DataSource { get; set; }
        public string CenterLabelText { get; set; } = "Statistics";
        public string HoverTextFormat { get; set; } = "${point.x}<br>Share<br>${point.y}%";
        public string TooltipFormat { get; set; } = "<b>${point.x}</b><br>Share: <b>${point.y}%</b>";
        public string Radius { get; set; } = "70%"; // Mặc định cho desktop
        public string MobileRadius { get; set; } = "40%"; // Mặc định cho mobile
        public int StartAngle { get; set; } = 60; // Mặc định cho desktop
        public int MobileStartAngle { get; set; } = 70; // Mặc định cho mobile
        public string CenterLabelFontSize { get; set; } = "15px"; // Mặc định cho desktop
        public string MobileCenterLabelFontSize { get; set; } = "7px"; // Mặc định cho mobile
        public string DataLabelFontSize { get; set; } = "12px"; // Mặc định cho desktop
        public string MobileDataLabelFontSize { get; set; } = "8px"; // Mặc định cho mobile
        public string ConnectorLength { get; set; } = "20px"; // Mặc định cho desktop
        public string MobileConnectorLength { get; set; } = "10px"; // Mặc định cho mobile
    }
}
