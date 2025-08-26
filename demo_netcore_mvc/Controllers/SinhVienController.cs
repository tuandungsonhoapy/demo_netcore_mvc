using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace demo_netcore_mvc.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ISinhVienRepository sinhVienRepository;
        private readonly IKhoaRepository khoaRepository;
        private readonly IGiangVienRepository giangVienRepository;

        public SinhVienController(ISinhVienRepository sinhVienRepository, IKhoaRepository khoaRepository, IGiangVienRepository giangVienRepository)
        {
            this.sinhVienRepository = sinhVienRepository;
            this.khoaRepository = khoaRepository;
            this.giangVienRepository = giangVienRepository;
        }

        // GET: SinhVienController
        public async Task<ActionResult> Index(SinhVien_GetAll_Params queryParams)
        {
            var list = await this.sinhVienRepository.GetAllAsync(queryParams);

            var model = new SinhVienViewModel
            {
                SinhViens = list,
                MaDT = queryParams.MaDT,
                MaGV = queryParams.MaGV,
                MaKhoa = queryParams.MaKhoa
            };

            var giangViens = await this.giangVienRepository.GetAllAsync(new object());

            ViewBag.GiangVienList = new SelectList(giangViens, "MaGV", "HoTenGV", queryParams.MaGV);

            var khoas = await this.khoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", queryParams.MaKhoa);

            return View(model);
        }

        // GET: SinhVienController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var sv = await this.sinhVienRepository.GetByIdAsync(id);

            if (sv == null)
            {
                return NotFound();
            }

            return View(sv);
        }

        // GET: SinhVienController/Create
        public async Task<ActionResult> Create()
        {
            var listKhoa = await this.khoaRepository.GetAllAsync(new Object());

            ViewBag.ListKhoa = listKhoa;

            return View();
        }

        // POST: SinhVienController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SinhVien model)
        {
            try
            {
                await this.sinhVienRepository.InsertAsync(model);

                TempData["AlertMessage"] = "Thêm sinh viên thành công!";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi thêm sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm sinh viên: " + ex.Message);
            }

            var listKhoa = await this.khoaRepository.GetAllAsync(new object());
            ViewBag.ListKhoa = listKhoa;

            return View(model);
        }

        // GET: SinhVienController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var sv = await this.sinhVienRepository.GetByIdAsync(id);
                if (sv == null)
                {
                    return NotFound();
                }
                var listKhoa = await this.khoaRepository.GetAllAsync(new object());
                ViewBag.ListKhoa = listKhoa;
                return View(sv);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi tải thông tin sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi tải thông tin sinh viên: " + ex.Message);
            }

            return View();
        }

        // POST: SinhVienController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, SinhVien model)
        {
            try
            {
                await this.sinhVienRepository.UpdateAsync(model);

                TempData["AlertMessage"] = "Cập nhật sinh viên thành công!";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi cập nhật sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
            }

            var listKhoa = await this.khoaRepository.GetAllAsync(new object());
            ViewBag.ListKhoa = listKhoa;
            return View(model);
        }

        // GET: SinhVienController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: SinhVienController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await this.sinhVienRepository.DeleteAsync(id);

                TempData["AlertMessage"] = "Xóa sinh viên thành công!";
                TempData["AlertType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi xóa sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
