using demo_netcore_mvc.Dto;
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
using Syncfusion.EJ2.Base;
using Syncfusion.XlsIO;
using System.Data;

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

        [HttpPost]
        public async Task<IActionResult> GetGiangViens([FromBody] DataManagerRequest dm)
        {
            var list = await _unitOfWork.GiangVienRepository.GetAllAsync(new GiangVien_GetAll_Param());

            var result = new List<GiangVienDto>();

            foreach (var item in list)
            {
                result.Add(new GiangVienDto
                {
                    MaGV = item.MaGV,
                    HoTenGV = item.HoTenGV,
                    Luong = item.Luong,
                    MaKhoa = item.MaKhoa,
                    TenKhoa = item.Khoa?.TenKhoa
                });
            }

            return Json(new { result = result, count = result.Count });
        }

        // GET: GiangVienController
        public async Task<ActionResult> Index(GiangVien_GetAll_Param requestParams)
        {
            var list = await _unitOfWork.GiangVienRepository.GetAllAsync(requestParams);

            var result = new List<GiangVienDto>();

            foreach (var item in list)
            {
                result.Add(new GiangVienDto
                {
                    MaGV = item.MaGV,
                    HoTenGV = item.HoTenGV,
                    Luong = item.Luong,
                    MaKhoa = item.MaKhoa,
                    TenKhoa = item.Khoa?.TenKhoa
                });
            }

            var model = new GiangVienViewModel
            {
                GiangViens = result,
                MaKhoa = requestParams.MaKhoa
            };

            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", requestParams.MaKhoa);

            ViewBag.Khoas = khoas;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(GiangVien_GetAll_Param requestParam)
        {
            var data = await _unitOfWork.GiangVienRepository.GetAllAsync(requestParam);

            // DataTable gốc, có đủ các cột
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã giảng viên");
            dt.Columns.Add("Họ tên giảng viên");
            dt.Columns.Add("Lương");
            dt.Columns.Add("Khoa");

            foreach (var g in data)
            {
                dt.Rows.Add(g.MaGV, g.HoTenGV, g.Luong, g.Khoa?.TenKhoa);
            }

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                using FileStream inputStream = new FileStream("wwwroot/templates/GiangVien_Template.xlsx", FileMode.Open, FileAccess.Read);
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                IWorksheet sheet = workbook.Worksheets[0];

                int headerRow = 1; // dòng chứa header trong template
                int startRow = headerRow + 1;
                int startCol = 1;
                int colCount = sheet.UsedRange.LastColumn;

                // Tạo DataTable theo đúng thứ tự cột trong template
                DataTable orderedDt = new DataTable();

                for (int col = 1; col <= colCount; col++)
                {
                    string header = sheet[headerRow, col].Text.Trim();

                    // Nếu DataTable gốc có cột này thì thêm vào orderedDt
                    if (dt.Columns.Contains(header))
                    {
                        orderedDt.Columns.Add(header);
                    }
                }

                // Copy dữ liệu theo đúng thứ tự cột
                foreach (DataRow row in dt.Rows)
                {
                    var newRow = orderedDt.NewRow();
                    foreach (DataColumn col in orderedDt.Columns)
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    orderedDt.Rows.Add(newRow);
                }

                // Đổ orderedDt vào Excel theo thứ tự cột template
                sheet.ImportDataTable(orderedDt, false, startRow, startCol);

                using MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "GiangVien.xlsx");
            }
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

            ViewBag.Khoas = khoas;

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

                ViewBag.Khoas = khoas;

                return View(model);
            }
        }

        // POST: GiangVienController/Insert
        //[HttpPost]
        //public async Task<ActionResult> Insert([FromBody] CRUDModel<GiangVien> value)
        //{
        //    if (value.Value != null)
        //    {
        //        var model = value.Value;

        //        var account = new Account
        //        {
        //            Username = model.Username, // nhớ map Username từ Grid hoặc bỏ nếu bạn không cho nhập
        //            Password = _hashingService.HashPassword(model.Password ?? "123456"), // default nếu Grid không có field
        //            Role = "GiangVien"
        //        };

        //        var gv = new GiangVien
        //        {
        //            HoTenGV = model.HoTenGV,
        //            Luong = model.Luong,
        //            MaKhoa = model.MaKhoa,
        //            Account = account
        //        };

        //        await _unitOfWork.GiangVienRepository.InsertAsync(gv);
        //        await _unitOfWork.SaveChangesAsync();

        //        return Json(gv); // trả lại object vừa thêm để grid cập nhật
        //    }
        //    return Json(null);
        //}

        // GET: GiangVienController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var model = await this._unitOfWork.GiangVienRepository.GetByIdAsync(id);

            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.ListKhoa = khoas;
            ViewBag.Khoas = khoas;

            return View(model);
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
