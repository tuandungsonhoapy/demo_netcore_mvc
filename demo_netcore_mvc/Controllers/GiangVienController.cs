using demo_netcore_mvc.IService;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.UnitOfWork;
using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace demo_netcore_mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GiangVienController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashingService _hashingService;

        public GiangVienController(IUnitOfWork unitOfWork, IHashingService hashingService)
        {
            _unitOfWork = unitOfWork;
            _hashingService = hashingService;
        }

        // GET: GiangVienController
        public async Task<ActionResult> Index(GiangVien_GetAll_Param requestParams)
        {
            var list = await _unitOfWork.GiangVienRepository.GetAllAsync(requestParams);

            var model = new GiangVienViewModel
            {
                GiangViens = list,
                MaKhoa = requestParams.MaKhoa
            };

            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", requestParams.MaKhoa);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcelSinhVienTheoTatCaGV(GiangVien_GetAll_Param requestParam)
        {
            var giangViens = await _unitOfWork.GiangVienRepository.GetAllAsync(requestParam);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "SinhVienTheoGV_Template.xlsx");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var wsTemplate = package.Workbook.Worksheets[0];
                var wsReport = package.Workbook.Worksheets["Report"]
                               ?? package.Workbook.Worksheets.Add("Report");

                if (wsTemplate == null || wsReport == null)
                    return BadRequest("Không tìm thấy worksheet Template/Report trong file Excel.");

                // Xác định block template (ví dụ: từ dòng 1 đến 5)
                int templateStart = 1;
                int templateEnd = 5;
                int blockHeight = templateEnd - templateStart + 1;
                int colCount = wsTemplate.Dimension.End.Column;

                int currentRow = 1;

                foreach (var gv in giangViens)
                {
                    // --- Copy block template từ Template sang Report ---
                    wsTemplate.Cells[templateStart, 1, templateEnd, colCount]
                              .Copy(wsReport.Cells[currentRow, 1]);

                    // --- Replace placeholders ---
                    wsReport.Cells[currentRow, 1].Value =
                        wsReport.Cells[currentRow, 1].Text.Replace("<<MaGV>>", gv.MaGV.ToString());
                    currentRow++;
                    wsReport.Cells[currentRow, 1].Value =
                        wsReport.Cells[currentRow, 1].Text.Replace("<<HoTenGV>>", gv.HoTenGV);
                    currentRow++;
                    wsReport.Cells[currentRow, 1].Value =
                        wsReport.Cells[currentRow, 1].Text.Replace("<<TenKhoa>>", gv.Khoa?.TenKhoa);

                    // Nhảy xuống header
                    currentRow += 2;

                    // --- Lấy header động ---
                    int headerRow = currentRow;
                    var headers = new Dictionary<int, string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        var headerText = wsReport.Cells[headerRow, col].Text.Trim();
                        if (!string.IsNullOrEmpty(headerText))
                            headers[col] = headerText;
                    }

                    currentRow++;

                    // --- Lấy danh sách sinh viên từ DeTai/HuongDan ---
                    var huongDans = gv.DeTais?.SelectMany(dt => dt.HuongDans).ToList()
                                     ?? new List<HuongDan>();

                    int stt = 1;
                    foreach (var hd in huongDans)
                    {
                        var sv = hd.SinhVien;

                        foreach (var kv in headers)
                        {
                            int col = kv.Key;
                            string header = kv.Value.ToLower();

                            switch (header)
                            {
                                case "stt": wsReport.Cells[currentRow, col].Value = stt; break;
                                case "mssv": wsReport.Cells[currentRow, col].Value = sv.MaSV; break;
                                case "họ tên":
                                case "ho ten": wsReport.Cells[currentRow, col].Value = sv.HoTenSV; break;
                                case "khoa": wsReport.Cells[currentRow, col].Value = sv.Khoa?.TenKhoa; break;
                                case "đề tài":
                                case "de tai": wsReport.Cells[currentRow, col].Value = hd.DeTai?.TenDT; break;
                            }
                        }

                        stt++;
                        currentRow++;
                    }

                    currentRow += 2; // cách 2 dòng trước khi tới giảng viên tiếp theo
                }

                // Xóa sheet Template trước khi xuất (nếu muốn)
                package.Workbook.Worksheets.Delete(wsTemplate);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"SinhVienTheoGV_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcelSinhVienTheoGiangVien(GiangVien_GetAll_Param requestParam)
        {
            var giangViens = await _unitOfWork.GiangVienRepository.GetAllAsync(requestParam);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "SinhVienTheoGV_Template.xlsx");

            // Khuyến nghị: đọc vào MemoryStream để tránh lock file ~$
            byte[] fileBytes = System.IO.File.ReadAllBytes(templatePath);
            using var mem = new MemoryStream(fileBytes);
            using (var package = new ExcelPackage(mem))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                    return BadRequest("Không tìm thấy sheet trong template.");

                int colCount = worksheet.Dimension.End.Column;
                int startRow = 2; // dữ liệu bắt đầu từ hàng 2
                int currentRow = startRow;

                // Xác định trước header (lowercase) cho nhanh
                var headers = Enumerable.Range(1, colCount)
                    .ToDictionary(c => c, c => (worksheet.Cells[1, c].Text ?? "").Trim().ToLower());

                // Tập cột thuộc nhóm Giảng viên (sẽ tự động được điền khi match các case GV)
                var gvCols = new HashSet<int>();

                foreach (var gv in giangViens)
                {
                    var huongDans = gv.DeTais?.SelectMany(dt => dt.HuongDans).ToList()
                                    ?? new List<HuongDan>();

                    int soSv = huongDans.Count > 0 ? huongDans.Count : 1;
                    int startGroupRow = currentRow;
                    int endGroupRow = currentRow + soSv - 1;

                    // Đổ dữ liệu (nếu không có SV vẫn tạo 1 dòng)
                    foreach (var hd in huongDans.DefaultIfEmpty())
                    {
                        var sv = hd?.SinhVien;

                        for (int col = 1; col <= colCount; col++)
                        {
                            string header = headers[col];

                            switch (header)
                            {
                                // ===== GIẢNG VIÊN =====
                                case "mã giảng viên":
                                case "ma giang vien":
                                case "mã gv":
                                case "ma gv":
                                    worksheet.Cells[currentRow, col].Value = gv.MaGV;
                                    gvCols.Add(col);
                                    break;

                                case "họ tên giảng viên":
                                case "ho ten giang vien":
                                case "họ tên gv":
                                case "ho ten gv":
                                    worksheet.Cells[currentRow, col].Value = gv.HoTenGV;
                                    gvCols.Add(col);
                                    break;

                                case "lương":
                                case "luong":
                                    worksheet.Cells[currentRow, col].Value = gv.Luong; // <-- fix
                                    gvCols.Add(col);
                                    break;

                                case "khoa gv":
                                case "khoa giang vien":
                                case "khoa giảng viên":
                                    worksheet.Cells[currentRow, col].Value = gv.Khoa?.TenKhoa;
                                    gvCols.Add(col);
                                    break;

                                // ===== SINH VIÊN =====
                                case "mssv":
                                case "ma sv":
                                    worksheet.Cells[currentRow, col].Value = sv?.MaSV;
                                    break;

                                case "họ tên sv":
                                case "ho ten sv":
                                    worksheet.Cells[currentRow, col].Value = sv?.HoTenSV;
                                    break;

                                case "năm sinh":
                                case "nam sinh":
                                    worksheet.Cells[currentRow, col].Value = sv?.NamSinh.ToString("dd/MM/yyyy");
                                    break;

                                case "quê quán":
                                case "que quan":
                                    worksheet.Cells[currentRow, col].Value = sv?.QueQuan;
                                    break;

                                case "khoa sv":
                                case "khoa sinh vien":
                                    worksheet.Cells[currentRow, col].Value = sv?.Khoa?.TenKhoa;
                                    break;

                                case "đề tài":
                                case "de tai":
                                    worksheet.Cells[currentRow, col].Value = hd?.DeTai?.TenDT;
                                    break;

                                default:
                                    break;
                            }
                        }

                        currentRow++;
                    }

                    // ===== MERGE các cột GIẢNG VIÊN cho nhóm này =====
                    if (soSv > 1 && gvCols.Count > 0)
                    {
                        foreach (var col in gvCols)
                        {
                            worksheet.Cells[startGroupRow, col, endGroupRow, col].Merge = true;
                            worksheet.Cells[startGroupRow, col, endGroupRow, col]
                                .Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        }
                    }
                }

                worksheet.Cells.AutoFitColumns();

                using var outStream = new MemoryStream();
                package.SaveAs(outStream);
                outStream.Position = 0;

                string fileName = $"SinhVienTheoGiangVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(outStream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        // GET: GiangVienController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var giangVien = await this._unitOfWork.GiangVienRepository.GetByIdAsync(id);

            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
        }

        // GET: GiangVienController/Create
        public async Task<ActionResult> Create()
        {
            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa");

            return View();
        }

        // POST: GiangVienController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountGiangVienData model)
        {
            try
            {
                var account = new Account
                {
                    Password = this._hashingService.HashPassword(model.Password),
                    Role = "GiangVien",
                    Username = model.Username
                };

                var giangVien = new GiangVien
                {
                    HoTenGV = model.HoTenGV,
                    Luong = model.Luong,
                    MaKhoa = model.MaKhoa,
                    Account = account
                };

                await this._unitOfWork.GiangVienRepository.InsertAsync(giangVien);

                await this._unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo giảng viên: {ex.Message}");
                TempData["AlertMessage"] = $"Lỗi khi tạo giảng viên: {ex.Message}";
                TempData["AlertType"] = "error";

                var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

                ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa");

                return View(model);
            }
        }

        // GET: GiangVienController/Edit/5
        public ActionResult Edit(int id)
        {
            var list = this._unitOfWork.GiangVienRepository.GetByIdAsync(id);
            return View();
        }

        // POST: GiangVienController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AccountGiangVienData model)
        {
            try
            {
                var account = new Account
                {
                    Password = model.Password,
                    Role = "GiangVien",
                    Username = model.Username
                };

                var giangVien = new GiangVien
                {
                    HoTenGV = model.HoTenGV,
                    Luong = model.Luong,
                    MaKhoa = model.MaKhoa,
                    Account = account
                };

                await this._unitOfWork.GiangVienRepository.InsertAsync(giangVien);

                await this._unitOfWork.SaveChangesAsync();

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
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await this._unitOfWork.GiangVienRepository.DeleteAsync(id);

                await this._unitOfWork.SaveChangesAsync();

                TempData["AlertMessage"] = "Xoá giảng viên thành công!";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Lỗi khi xoá giảng viên: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi xoá giảng viên: " + ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
