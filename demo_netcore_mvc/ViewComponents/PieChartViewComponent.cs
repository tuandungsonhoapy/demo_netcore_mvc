using demo_netcore_mvc.Configs.ChartConfig;
using Microsoft.AspNetCore.Mvc;

namespace demo_netcore_mvc.ViewComponents
{
    public class PieChartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(PieChartConfig config)
        {
            return View(config);
        }
    }
}
