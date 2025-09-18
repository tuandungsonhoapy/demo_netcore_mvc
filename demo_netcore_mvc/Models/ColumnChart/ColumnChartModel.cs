using demo_netcore_mvc.Configs.ChartConfig;
using System.Collections;

namespace demo_netcore_mvc.Models.ColumnChart
{
    public class ColumnChartModel
    {
        public IEnumerable DataSource { get; set; }
        public string XName { get; set; }
        public List<SeriesInfo> Series { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public AxisConfig PrimaryYAxis { get; set; }
    }
}
