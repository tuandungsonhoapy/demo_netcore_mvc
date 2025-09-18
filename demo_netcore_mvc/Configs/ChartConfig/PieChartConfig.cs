using System.Collections;

namespace demo_netcore_mvc.Configs.ChartConfig
{
    public class PieChartConfig
    {
        public IEnumerable DataSource { get; set; }
        public string Title { get; set; } = "";
        public string TooltipFormat { get; set; } = "<b>${point.x}</b><br>Value: <b>${point.y}</b>";
    }
}
