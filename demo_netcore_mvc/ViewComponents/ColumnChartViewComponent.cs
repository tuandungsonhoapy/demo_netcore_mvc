using demo_netcore_mvc.Configs.ChartConfig;
using demo_netcore_mvc.Models.ColumnChart;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace demo_netcore_mvc.ViewComponents
{
    public class ColumnChartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
                IEnumerable dataSource,
                string xName,
                List<SeriesInfo> series,
                string title,
                string subTitle = "",
                AxisConfig PrimaryYAxis = null
            )
        {
            var model = new ColumnChartModel
            {
                DataSource = dataSource,
                XName = xName,
                Series = series,
                Title = title,
                SubTitle = subTitle,
                PrimaryYAxis = PrimaryYAxis
            };
            return View(model);
        }
    }
}
