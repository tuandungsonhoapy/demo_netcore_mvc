using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace demo_netcore_mvc.Controllers
{
    [Authorize(Roles = "Admin,GiangVien")]
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

        public async Task<IActionResult> ExportExcel(SinhVien_GetAll_Params queryParams)
        {
            var list = await this.sinhVienRepository.GetAllAsync(queryParams);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachSinhVien");

                // Header
                worksheet.Cells[1, 1].Value = "MSSV";
                worksheet.Cells[1, 2].Value = "Họ tên";
                worksheet.Cells[1, 3].Value = "Khoa";
                worksheet.Cells[1, 4].Value = "Năm sinh";
                worksheet.Cells[1, 5].Value = "Quê quán";

                // Data
                for (int i = 0; i < list.Count; i++)
                {
                    var sv = list[i];
                    worksheet.Cells[i + 2, 1].Value = sv.MaSV;
                    worksheet.Cells[i + 2, 2].Value = sv.HoTenSV;
                    worksheet.Cells[i + 2, 3].Value = sv.Khoa?.TenKhoa;
                    worksheet.Cells[i + 2, 4].Value = sv.NamSinh.ToString("dd/MM/yyyy");
                    worksheet.Cells[i + 2, 5].Value = sv.QueQuan;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public async Task<IActionResult> ExportWord(SinhVien_GetAll_Params queryParams)
        {
            var list = await this.sinhVienRepository.GetAllAsync(queryParams);
            var htmlContent = "<html><head><meta charset='UTF-8'></head><body>";
            htmlContent += "<h2>Danh sách sinh viên</h2>";
            htmlContent += "<table border='1' cellpadding='5' cellspacing='0'>";
            htmlContent += "<tr><th>MSSV</th><th>Họ tên</th><th>Khoa</th><th>Năm sinh</th><th>Quê quán</th></tr>";
            foreach (var sv in list)
            {
                htmlContent += $"<tr>" +
                               $"<td>{sv.MaSV}</td>" +
                               $"<td>{sv.HoTenSV}</td>" +
                               $"<td>{sv.Khoa?.TenKhoa}</td>" +
                               $"<td>{sv.NamSinh:dd/MM/yyyy}</td>" +
                               $"<td>{sv.QueQuan}</td>" +
                               $"</tr>";
            }
            htmlContent += "</table></body></html>";
            var byteArray = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            var stream = new MemoryStream(byteArray);
            string fileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMddHHmmss}.doc";
            return File(stream.ToArray(),
                "application/msword",
                fileName);
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
