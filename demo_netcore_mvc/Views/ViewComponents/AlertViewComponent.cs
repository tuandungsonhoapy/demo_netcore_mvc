using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace demo_netcore_mvc.Views.ViewComponents
{
    public class AlertViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string type, string message)
        {
            var model = new AlertViewModel
            {
                Type = type,
                Message = message
            };
            return View(model);
        }
    }
}
