using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.UnitOfWork;
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
        private readonly IUnitOfWork _unitOfWork;

        public SinhVienController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SinhVienController
        public async Task<ActionResult> Index(SinhVien_GetAll_Params queryParams)
        {
            var list = await this._unitOfWork.SinhVienRepository.GetAllAsync(queryParams);

            var model = new SinhVienViewModel
            {
                SinhViens = list,
                MaDT = queryParams.MaDT,
                MaGV = queryParams.MaGV,
                MaKhoa = queryParams.MaKhoa
            };

            var giangViens = await this._unitOfWork.GiangVienRepository.GetAllAsync(new GiangVien_GetAll_Param
            {
                MaKhoa = queryParams.MaKhoa
            });

            ViewBag.GiangVienList = new SelectList(giangViens, "MaGV", "HoTenGV", queryParams.MaGV);

            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", queryParams.MaKhoa);

            return View(model);
        }

        public async Task<ActionResult> ExportExcel(SinhVien_GetAll_Params queryParams)
        {
            var list = await this._unitOfWork.SinhVienRepository.GetAllAsync(queryParams);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "SinhVien_Template.xlsx");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    return BadRequest();
                }

                int colCount = worksheet.Dimension.End.Column;
                int startRow = 2; // Dữ liệu bắt đầu từ hàng thứ 2
                for (int i = 0; i < list.Count; i++)
                {
                    var sv = list[i];
                    int row = startRow + i;

                    for (int col = 1; col <= colCount; col++)
                    {
                        string header = worksheet.Cells[1, col].Text.Trim();

                        switch (header)
                        {
                            case "MSSV":
                            case "Mã số sinh viên":
                                worksheet.Cells[row, col].Value = sv?.MaSV;
                                break;
                            case "Họ tên":
                            case "Ho ten":
                                worksheet.Cells[row, col].Value = sv?.HoTenSV;
                                break;
                            case "Khoa":
                                worksheet.Cells[row, col].Value = sv?.Khoa?.TenKhoa;
                                break;
                            case "Năm sinh":
                            case "Nam sinh":
                                worksheet.Cells[row, col].Value = sv?.NamSinh.ToString("dd/MM/yyyy");
                                break;
                            case "Quê quán":
                            case "Que quan":
                                worksheet.Cells[row, col].Value = sv?.QueQuan;
                                break;
                            default:
                                break;
                        }
                    }
                }

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
            var list = await this._unitOfWork.SinhVienRepository.GetAllAsync(queryParams);
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
            var sv = await this._unitOfWork.SinhVienRepository.GetByIdAsync(id);

            if (sv == null)
            {
                return NotFound();
            }

            return View(sv);
        }

        // GET: SinhVienController/Create
        public async Task<ActionResult> Create()
        {
            var listKhoa = await this._unitOfWork.KhoaRepository.GetAllAsync(new Object());

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
                await this._unitOfWork.SinhVienRepository.InsertAsync(model);

                TempData["AlertMessage"] = "Thêm sinh viên thành công!";
                TempData["AlertType"] = "success";

                await this._unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi thêm sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm sinh viên: " + ex.Message);
            }

            var listKhoa = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());
            ViewBag.ListKhoa = listKhoa;

            return View(model);
        }

        // GET: SinhVienController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var sv = await this._unitOfWork.SinhVienRepository.GetByIdAsync(id);
                if (sv == null)
                {
                    return NotFound();
                }
                var listKhoa = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());
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
                await this._unitOfWork.SinhVienRepository.UpdateAsync(model);

                TempData["AlertMessage"] = "Cập nhật sinh viên thành công!";
                TempData["AlertType"] = "success";

                await this._unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi cập nhật sinh viên: " + ex.Message;
                TempData["AlertType"] = "error";
            }

            var listKhoa = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());
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
                await this._unitOfWork.SinhVienRepository.DeleteAsync(id);

                await this._unitOfWork.SaveChangesAsync();

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
