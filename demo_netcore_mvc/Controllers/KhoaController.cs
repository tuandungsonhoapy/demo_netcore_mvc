using demo_netcore_mvc.Models;
using demo_netcore_mvc.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace demo_netcore_mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KhoaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public KhoaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: KhoaController
        public async Task<IActionResult> Index()
        {
            var list = await _unitOfWork.KhoaRepository.GetAllAsync(new object());

            return View(list);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var list = await _unitOfWork.KhoaRepository.GetAllAsync(new object());

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachKhoa");
                // Header
                worksheet.Cells[1, 1].Value = "Mã khoa";
                worksheet.Cells[1, 2].Value = "Tên khoa";
                worksheet.Cells[1, 3].Value = "Điện thoại";
                // Data
                for (int i = 0; i < list.Count; i++)
                {
                    var khoa = list[i];
                    worksheet.Cells[i + 2, 1].Value = khoa.MaKhoa;
                    worksheet.Cells[i + 2, 2].Value = khoa.TenKhoa;
                    worksheet.Cells[i + 2, 3].Value = khoa.DienThoai;
                }
                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var excelData = package.GetAsByteArray();
                var fileName = $"DanhSachKhoa_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        public async Task<IActionResult> ExportWord()
        {
            var list = await _unitOfWork.KhoaRepository.GetAllAsync(new object());

            // Xuất file Word
            var htmlContent = "<html><head><meta charset='UTF-8'></head><body>";
            htmlContent += "<h2>Danh sách khoa</h2>";
            htmlContent += "<table border='1' cellpadding='5' cellspacing='0'>";
            htmlContent += "<tr><th>Mã khoa</th><th>Tên khoa</th><th>Điện thoại</th></tr>";

            foreach (var khoa in list)
            {
                htmlContent += $"<tr><td>{khoa.MaKhoa}</td><td>{khoa.TenKhoa}</td><td>{khoa.DienThoai}</td></tr>";
            }

            htmlContent += "</table></body></html>";

            var byteArray = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            var stream = new MemoryStream(byteArray);
            string fileName = $"DanhSachKhoa_{DateTime.Now:yyyyMMddHHmmss}.doc";

            return File(stream.ToArray(),
                "application/msword",
                fileName);
        }

        // GET: KhoaController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var khoa = await _unitOfWork.KhoaRepository.GetByIdAsync(id);
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
                await _unitOfWork.KhoaRepository.InsertAsync(model);

                TempData["AlertMessage"] = "Thêm khoa thành công!";
                TempData["AlertType"] = "success";

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi thêm khoa: " + ex.Message;
                TempData["AlertType"] = "error";

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: KhoaController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                var khoa = await _unitOfWork.KhoaRepository.GetByIdAsync(id.ToString());

                if (khoa == null)
                {
                    return NotFound();
                }

                return View(khoa);
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        // POST: KhoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Khoa model)
        {
            try
            {
                await this._unitOfWork.KhoaRepository.UpdateAsync(model);

                TempData["AlertMessage"] = "Cập nhật khoa thành công!";
                TempData["AlertType"] = "success";

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi cập nhật khoa: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật khoa: " + ex.Message);
            }

            return View();
        }

        // GET: KhoaController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: KhoaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, IFormCollection collection)
        {
            try
            {
                await this._unitOfWork.KhoaRepository.DeleteAsync(id);

                TempData["AlertMessage"] = "Xóa khoa thành công!";
                TempData["AlertType"] = "success";

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi xóa khoa: " + ex.Message;
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
