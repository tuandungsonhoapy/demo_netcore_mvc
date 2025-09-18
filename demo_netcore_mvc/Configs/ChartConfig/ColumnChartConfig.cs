using demo_netcore_mvc.Models.ColumnChart;

namespace demo_netcore_mvc.Configs.ChartConfig
{
    public class ColumnChartConfig<T> where T : class
    {
        public List<T> DataSource { get; set; }
        public string XName { get; set; }
        public List<SeriesInfo> Series { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public AxisConfig PrimaryYAxis { get; set; }
    }

    public class AxisConfig
    {
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Interval { get; set; }
    }
}
