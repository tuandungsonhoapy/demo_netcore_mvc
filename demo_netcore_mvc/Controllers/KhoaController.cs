using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace demo_netcore_mvc.Controllers
{
    public class KhoaController : Controller
    {
        private readonly IKhoaRepository _khoaRepository;

        public KhoaController(IKhoaRepository khoaRepository)
        {
            _khoaRepository = khoaRepository;
        }

        // GET: KhoaController
        public async Task<IActionResult> Index()
        {
            var list = await _khoaRepository.GetAllAsync(new object());

            return View(list);
        }

        // GET: KhoaController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var khoa = await _khoaRepository.GetByIdAsync(id);
            if (khoa == null)
            {
                return NotFound();
            }
            return View(khoa);
        }

        // GET: KhoaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khoa model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _khoaRepository.InsertAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: KhoaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KhoaController/Edit/5
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

        // GET: KhoaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KhoaController/Delete/5
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
