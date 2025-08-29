using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace demo_netcore_mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GiangVienController : Controller
    {
        private readonly IGiangVienRepository giangVienRepository;
        private readonly IKhoaRepository khoaRepository;

        public GiangVienController(IGiangVienRepository giangVienRepository, IKhoaRepository khoaRepository)
        {
            this.giangVienRepository = giangVienRepository;
            this.khoaRepository = khoaRepository;
        }

        // GET: GiangVienController
        public async Task<ActionResult> Index(GiangVien_GetAll_Param requestParams)
        {
            var list = await giangVienRepository.GetAllAsync(requestParams);

            var model = new GiangVienViewModel
            {
                GiangViens = list,
                MaKhoa = requestParams.MaKhoa
            };

            var khoas = await this.khoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", requestParams.MaKhoa);

            return View(model);
        }

        // GET: GiangVienController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var giangVien = await this.giangVienRepository.GetByIdAsync(id);

            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
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
            var list = this.giangVienRepository.GetByIdAsync(id);
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
