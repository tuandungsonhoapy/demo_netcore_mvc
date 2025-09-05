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
    [Authorize]
    public class DeTaiController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeTaiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: DeTaiContrller
        public async Task<ActionResult> Index(DeTai_GetAll_Param requestData)
        {
            var list = await _unitOfWork.DeTaiRepository.GetAllAsync(requestData);

            var model = new DeTaiViewModel
            {
                DeTais = list,
                MaGV = requestData.MaGV,
                MaKhoa = requestData.MaKhoa,
                HocKy = requestData.HocKy,
                NamHoc = requestData.NamHoc
            };

            var giangViens = await this._unitOfWork.GiangVienRepository.GetAllAsync(new object());

            ViewBag.GiangVienList = new SelectList(giangViens, "MaGV", "HoTenGV", requestData.MaGV);

            var khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", requestData.MaKhoa);

            var namHocs = await this._unitOfWork.DeTaiRepository.GetAllNamHoc();

            ViewBag.NamHocList = new SelectList(namHocs, "NamHoc", "NamHoc", requestData.NamHoc);

            var hocKys = new List<byte> { 1, 2, 3 };

            ViewBag.HocKyList = new SelectList(hocKys, requestData.HocKy);

            return View(model);
        }

        public async Task<ActionResult> MyDeTai(DeTai_MyDeTai_Queries requestData)
        {
            var list = await _unitOfWork.DeTaiRepository.MyDeTai(requestData);

            var model = new MyDeTaiViewModel
            {
                MaSV = requestData.MaSV,
                NamHoc = requestData.NamHoc,
                HocKy = requestData.HocKy,
                DeTais = list
            };

            var namHocs = await this._unitOfWork.DeTaiRepository.GetAllNamHoc();

            ViewBag.NamHocList = new SelectList(namHocs, "NamHoc", "NamHoc", requestData.NamHoc);

            var hocKys = new List<byte> { 1, 2, 3 };

            ViewBag.HocKyList = new SelectList(hocKys, requestData.HocKy);

            return View(model);
        }

        public async Task<ActionResult> DeTaiThamGia(DeTai_MyDeTai_Queries requestData)
        {

            var list = await _unitOfWork.DeTaiRepository.MyDeTai(requestData);

            var model = new MyDeTaiViewModel
            {
                MaSV = requestData.MaSV,
                NamHoc = requestData.NamHoc,
                HocKy = requestData.HocKy,
                DeTais = list
            };

            var namHocs = await this._unitOfWork.DeTaiRepository.GetAllNamHoc();

            ViewBag.NamHocList = new SelectList(namHocs, "NamHoc", "NamHoc", requestData.NamHoc);

            var hocKys = new List<byte> { 1, 2, 3 };

            ViewBag.HocKyList = new SelectList(hocKys, requestData.HocKy);

            return View(model);
        }

        public async Task<ActionResult> DeTaiHuongDan(DeTai_GetAll_Param requestData)
        {
            if (requestData.MaGV == null || requestData.MaGV == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var list = await this._unitOfWork.DeTaiRepository.GetAllAsync(requestData);

            var model = new DeTaiViewModel
            {
                DeTais = list,
                MaGV = requestData.MaGV,
                MaKhoa = requestData.MaKhoa,
                HocKy = requestData.HocKy,
                NamHoc = requestData.NamHoc
            };

            var namHocs = await this._unitOfWork.DeTaiRepository.GetAllNamHoc();

            ViewBag.NamHocList = new SelectList(namHocs, "NamHoc", "NamHoc", requestData.NamHoc);

            var hocKys = new List<byte> { 1, 2, 3 };

            ViewBag.HocKyList = new SelectList(hocKys, requestData.HocKy);

            return View(model);
        }

        public async Task<ActionResult> ChamDiem(int MaSV, string MaDT, decimal KetQua, int MaGV)
        {
            var dt = await this._unitOfWork.DeTaiRepository.GetByIdAsync(MaDT);

            if (dt == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại.";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            if (dt.NguoiHuongDan != MaGV)
            {
                TempData["AlertMessage"] = "Chỉ giảng viên hướng dẫn mới được phép chấm điểm đề tài này!";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            try
            {
                await this._unitOfWork.HuongDanRepository.UpdateAsync(new HuongDan
                {
                    MaDT = MaDT,
                    MaSV = MaSV,
                    KetQua = KetQua
                });

                await this._unitOfWork.SaveChangesAsync();
            }
            catch
            {
                TempData["AlertMessage"] = "Chấm điểm thất bại!";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Details", new { id = MaDT });
        }

        public async Task<ActionResult> ExportExcel(DeTai_GetAll_Param queryParams)
        {
            var list = await this._unitOfWork.DeTaiRepository.GetAllAsync(queryParams);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "DeTai_Template.xlsx");

            using (var package = new ExcelPackage(new FileInfo(templatePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    return BadRequest("Không tìm thấy sheet 'DanhSachDeTai' trong template.");
                }

                int colCount = worksheet.Dimension.End.Column;
                int startRow = 2; // Dữ liệu bắt đầu từ hàng thứ 2
                for (int i = 0; i < list.Count; i++)
                {
                    var dt = list[i];
                    int row = startRow + i;

                    for (int col = 1; col <= colCount; col++)
                    {
                        string header = worksheet.Cells[1, col].Text.Trim();

                        switch (header)
                        {
                            case "Mã đề tài":
                            case "Ma de tai":
                                worksheet.Cells[row, col].Value = dt.MaDT;
                                break;
                            case "Tên đề tài":
                            case "Ten de tai":
                                worksheet.Cells[row, col].Value = dt.TenDT;
                                break;
                            case "Kinh phí":
                            case "Kinh phi":
                                worksheet.Cells[row, col].Value = dt.KinhPhi;
                                break;
                            case "Nơi thực tập":
                            case "Noi thuc tap":
                                worksheet.Cells[row, col].Value = dt.NoiThucTap;
                                break;
                            case "Tham gia":
                            case "Tham gia/Toi da":
                                worksheet.Cells[row, col].Value = $"{dt.SoLuong}/{dt.ToiDa}";
                                break;
                            case "Giảng viên":
                            case "Giang vien":
                                worksheet.Cells[row, col].Value = dt.GiangVien?.HoTenGV;
                                break;
                            case "Khoa":
                                worksheet.Cells[row, col].Value = dt.GiangVien?.Khoa?.TenKhoa;
                                break;
                            case "Học kỳ":
                            case "Hoc ky":
                                worksheet.Cells[row, col].Value = dt.HocKy;
                                break;
                            case "Năm học":
                            case "Nam hoc":
                                worksheet.Cells[row, col].Value = dt.NamHoc;
                                break;
                            default:
                                // Nếu header không khớp với bất kỳ trường nào, giữ nguyên giá trị trong ô
                                break;
                        }
                    }
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DanhSachDeTai_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public async Task<ActionResult> ExportExcel_DSSV(string MaDT)
        {
            var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(MaDT);

            if (deTai == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại.";
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"DSSV_DeTai_{deTai.MaDT}");
                var list = deTai.HuongDans.ToList();

                // Title
                worksheet.Cells[1, 1].Value = $"Danh sách sinh viên tham gia đề tài: {deTai.TenDT} ({deTai.MaDT})";

                // Header
                worksheet.Cells[2, 1].Value = "Mã sinh viên";
                worksheet.Cells[2, 2].Value = "Họ tên";
                worksheet.Cells[2, 3].Value = "Khoa";
                worksheet.Cells[2, 4].Value = "Kết quả";

                // Data
                for (int i = 0; i < list.Count; i++)
                {
                    var hds = list[i];
                    worksheet.Cells[i + 3, 1].Value = hds.MaSV;
                    worksheet.Cells[i + 3, 2].Value = hds.SinhVien?.HoTenSV;
                    worksheet.Cells[i + 3, 3].Value = hds.SinhVien?.Khoa?.TenKhoa;
                    worksheet.Cells[i + 3, 4].Value = hds.KetQua;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DSSV_{deTai.MaDT.Trim()}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public async Task<ActionResult> ExportWord(DeTai_GetAll_Param queryParams)
        {
            var list = await this._unitOfWork.DeTaiRepository.GetAllAsync(queryParams);

            var htmlContent = "<html><head><meta charset='UTF-8'></head><body>";
            htmlContent += "<h2>Danh sách đề tài</h2>";
            htmlContent += "<table border='1' cellpadding='5' cellspacing='0'>";
            htmlContent += "<tr><th>Mã đề tài</th><th>Tên đề tài</th><th>Kinh phí</th>" +
                "<th>Nơi thực tập</th><th>Tham gia</th><th>Giảng viên</th><th>Khoa</th><th>HK/Năm</th></tr>";
            foreach (var dt in list)
            {
                htmlContent += $"<tr>" +
                               $"<td>{dt.MaDT}</td>" +
                               $"<td>{dt.TenDT}</td>" +
                               $"<td>{dt.KinhPhi}</td>" +
                               $"<td>{dt.NoiThucTap}</td>" +
                               $"<td>{dt.SoLuong}/{dt.ToiDa}</td>" +
                               $"<td>{dt.GiangVien?.HoTenGV}</td>" +
                               $"<td>{dt.GiangVien?.Khoa?.TenKhoa}</td>" +
                               $"<td>{dt.HocKy}/{dt.NamHoc}</td>" +
                               $"</tr>";
            }
            htmlContent += "</table></body></html>";
            var byteArray = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            var stream = new MemoryStream(byteArray);
            string fileName = $"DanhSachDeTai_{DateTime.Now:yyyyMMddHHmmss}.doc";
            return File(stream.ToArray(),
                "application/msword",
                fileName);
        }

        public async Task<ActionResult> ExportWord_DSSV(string MaDT)
        {
            var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(MaDT);

            if (deTai == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại.";
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var list = deTai.HuongDans.ToList();

            var htmlContent = "<html><head><meta charset='UTF-8'></head><body>";
            htmlContent += $"<h2>Danh sách sinh viên tham gia đề tài: {deTai.TenDT} ({deTai.MaDT})</h2>";
            htmlContent += "<table border='1' cellpadding='5' cellspacing='0'>";
            htmlContent += "<tr><th>Mã sinh viên</th><th>Họ tên</th><th>Khoa</th>" +
                "<th>Kết quả</th></tr>";
            foreach (var hd in list)
            {
                htmlContent += $"<tr>" +
                               $"<td>{hd.MaSV}</td>" +
                               $"<td>{hd.SinhVien?.HoTenSV}</td>" +
                               $"<td>{hd.SinhVien?.Khoa?.TenKhoa}</td>" +
                               $"<td>{hd.KetQua}</td>" +
                               $"</tr>";
            }
            htmlContent += "</table></body></html>";
            var byteArray = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            var stream = new MemoryStream(byteArray);
            string fileName = $"DSSV_{deTai.MaDT.Trim()}_{DateTime.Now:yyyyMMddHHmmss}.doc";
            return File(stream.ToArray(),
                "application/msword",
                fileName);
        }

        [HttpPost]
        public async Task<ActionResult> Register(DeTai_Register_Body requestData)
        {
            var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(requestData.MaDT);

            if (deTai == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại.";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            if (deTai.SoLuong >= deTai.ToiDa)
            {
                TempData["AlertMessage"] = "Đề tài đã đạt số lượng tối đa.";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            var luongThamGiaDeTaiTheoKy = await this._unitOfWork.DeTaiRepository.GetSVThamGiaTheoKy(requestData.MaSV, deTai.NamHoc, (byte)deTai.HocKy);

            if (luongThamGiaDeTaiTheoKy.Count > 0)
            {
                TempData["AlertMessage"] = "Sinh viên đã tham gia đề tài trong kỳ này.";
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            if (!deTai.IsOpen)
            {
                TempData["AlertMessage"] = "Đề tài này không mở đăng ký.";
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            await this._unitOfWork.HuongDanRepository.InsertAsync(new Models.HuongDan
            {
                MaDT = requestData.MaDT,
                MaSV = requestData.MaSV,
                MaGV = requestData.MaGV
            });

            await this._unitOfWork.SaveChangesAsync();

            TempData["AlertMessage"] = "Đăng ký đề tài thành công.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> Unregister(DeTai_Unregister_Param requestData, string returnAction)
        {
            try
            {
                var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(requestData.MaDT);

                if (deTai == null)
                {
                    TempData["AlertMessage"] = "Đề tài không tồn tại.";
                    TempData["AlertType"] = "error";

                    return RedirectToAction(nameof(Index));
                }

                if (!deTai.IsOpen)
                {
                    TempData["AlertMessage"] = "Đề tài này không mở đăng ký.";
                    TempData["AlertType"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                await this._unitOfWork.HuongDanRepository.DeleteAsync(requestData);

                await this._unitOfWork.SaveChangesAsync();

                TempData["AlertMessage"] = "Hủy đăng ký thành công!";
                TempData["AlertType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = ex.Message;
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(returnAction ?? nameof(Index), new { MaSV = requestData.MaSV });
        }

        [Authorize(Roles = "Admin,GiangVien")]
        // GET: DeTaiContrller/Details/5
        public async Task<ActionResult> Details(string id, string returnUrl)
        {
            var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(id);

            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index");

            return View(deTai);
        }

        [Authorize(Roles = "Admin,GiangVien")]
        // GET: DeTaiContrller/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,GiangVien")]
        // POST: DeTaiContrller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DeTai model)
        {
            try
            {
                await this._unitOfWork.DeTaiRepository.InsertAsync(model);

                TempData["AlertMessage"] = "Thêm mới đề tài thành công!";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Thêm mới đề thất bại! Lỗi: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi thêm sinh viên: " + ex.Message);
            }
            return View(model);
        }

        // GET: DeTaiContrller/Edit/5
        [Authorize(Roles = "Admin,GiangVien")]
        public async Task<ActionResult> Edit(string id, string returnUrl)
        {
            var deTai = await this._unitOfWork.DeTaiRepository.GetByIdAsync(id);

            if (deTai == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại!";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index");

            return View(deTai);
        }

        // POST: DeTaiContrller/Edit/5
        [Authorize(Roles = "Admin,GiangVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, DeTai model, int editedBy)
        {
            try
            {
                if (model.NguoiHuongDan != editedBy)
                {
                    TempData["AlertMessage"] = "Chỉ giảng viên hướng dẫn mới được phép sửa đề tài này!";
                    TempData["AlertType"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                await this._unitOfWork.DeTaiRepository.UpdateAsync(model);

                await this._unitOfWork.SaveChangesAsync();

                TempData["AlertMessage"] = "Cập nhật đề tài thành công!";
                TempData["AlertType"] = "success";


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Cập nhật đề tài thất bại! Lỗi: " + ex.Message;
                TempData["AlertType"] = "error";
                ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật đề tài: " + ex.Message);
            }

            return View(model);

        }

        // GET: DeTaiContrller/Delete/5
        [Authorize(Roles = "Admin,GiangVien")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DeTaiContrller/Delete/5
        [Authorize(Roles = "Admin,GiangVien")]
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
