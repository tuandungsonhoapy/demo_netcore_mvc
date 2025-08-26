using demo_netcore_mvc.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace demo_netcore_mvc.Controllers
{
    public class GiangVienController : Controller
    {
        private readonly IGiangVienRepository giangVienRepository;

        public GiangVienController(IGiangVienRepository giangVienRepository)
        {
            this.giangVienRepository = giangVienRepository;
        }

        // GET: GiangVienController
        public async Task<ActionResult> Index()
        {
            var list = await giangVienRepository.GetAllAsync(new object());
            return View(list);
        }

        // GET: GiangVienController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GiangVienController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GiangVienController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GiangVienController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GiangVienController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GiangVienController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GiangVienController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
