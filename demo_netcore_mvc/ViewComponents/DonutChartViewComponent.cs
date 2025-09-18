using demo_netcore_mvc.Configs.ChartConfig;
using Microsoft.AspNetCore.Mvc;

namespace demo_netcore_mvc.ViewComponents
{
    public class DonutChartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DonutChartConfig config)
        {
            return View(config);
        }
    }
}
