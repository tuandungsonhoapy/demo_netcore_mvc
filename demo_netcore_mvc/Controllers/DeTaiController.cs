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
    [Authorize]
    public class DeTaiController : Controller
    {
        private readonly IDetaiRepository _deTaiRepository;
        private readonly IGiangVienRepository _giangVienRepository;
        private readonly IKhoaRepository _khoaRepository;
        private readonly IHuongDanRepository _huongDanRepository;

        public DeTaiController(IDetaiRepository deTaiRepository, IGiangVienRepository giangVienRepository, IKhoaRepository khoaRepository, IHuongDanRepository huongDanRepository)
        {
            _deTaiRepository = deTaiRepository;
            _giangVienRepository = giangVienRepository;
            _khoaRepository = khoaRepository;
            _huongDanRepository = huongDanRepository;
        }

        // GET: DeTaiContrller
        public async Task<ActionResult> Index(DeTai_GetAll_Param requestData)
        {
            var list = await _deTaiRepository.GetAllAsync(requestData);

            var model = new DeTaiViewModel
            {
                DeTais = list,
                MaGV = requestData.MaGV,
                MaKhoa = requestData.MaKhoa,
                HocKy = requestData.HocKy,
                NamHoc = requestData.NamHoc
            };

            var giangViens = await this._giangVienRepository.GetAllAsync(new object());

            ViewBag.GiangVienList = new SelectList(giangViens, "MaGV", "HoTenGV", requestData.MaGV);

            var khoas = await this._khoaRepository.GetAllAsync(new object());

            ViewBag.KhoaList = new SelectList(khoas, "MaKhoa", "TenKhoa", requestData.MaKhoa);

            var namHocs = await this._deTaiRepository.GetAllNamHoc();

            ViewBag.NamHocList = new SelectList(namHocs, "NamHoc", "NamHoc", requestData.NamHoc);

            var hocKys = new List<byte> { 1, 2, 3 };

            ViewBag.HocKyList = new SelectList(hocKys, requestData.HocKy);

            return View(model);
        }

        public async Task<ActionResult> ChamDiem(int MaSV, string MaDT, decimal KetQua, int MaGV)
        {
            var dt = await this._deTaiRepository.GetByIdAsync(MaDT);

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
                await this._huongDanRepository.UpdateAsync(new HuongDan
                {
                    MaDT = MaDT,
                    MaSV = MaSV,
                    KetQua = KetQua
                });
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
            var list = await this._deTaiRepository.GetAllAsync(queryParams);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachDeTai");

                // Header
                worksheet.Cells[1, 1].Value = "Mã đề tài";
                worksheet.Cells[1, 2].Value = "Tên đề tài";
                worksheet.Cells[1, 3].Value = "Kinh phí";
                worksheet.Cells[1, 4].Value = "Nơi thực tập";
                worksheet.Cells[1, 5].Value = "Tham gia";
                worksheet.Cells[1, 6].Value = "Giảng viên";
                worksheet.Cells[1, 7].Value = "Khoa";
                worksheet.Cells[1, 8].Value = "HK/Năm học";

                // Data
                for (int i = 0; i < list.Count; i++)
                {
                    var dt = list[i];
                    worksheet.Cells[i + 2, 1].Value = dt.MaDT;
                    worksheet.Cells[i + 2, 2].Value = dt.TenDT;
                    worksheet.Cells[i + 2, 3].Value = dt.KinhPhi;
                    worksheet.Cells[i + 2, 4].Value = dt.NoiThucTap;
                    worksheet.Cells[i + 2, 5].Value = $"{dt.SoLuong}/{dt.ToiDa}";
                    worksheet.Cells[i + 2, 6].Value = dt.GiangVien?.HoTenGV;
                    worksheet.Cells[i + 2, 7].Value = dt.GiangVien?.Khoa?.TenKhoa;
                    worksheet.Cells[i + 2, 8].Value = $"{dt.HocKy}/{dt.NamHoc}";
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

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
            var deTai = await this._deTaiRepository.GetByIdAsync(MaDT);

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
            var list = await this._deTaiRepository.GetAllAsync(queryParams);

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
            var deTai = await this._deTaiRepository.GetByIdAsync(MaDT);

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
            var deTai = await this._deTaiRepository.GetByIdAsync(requestData.MaDT);

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

            var luongThamGiaDeTaiTheoKy = await this._deTaiRepository.GetSVThamGiaTheoKy(requestData.MaSV, deTai.NamHoc, (byte)deTai.HocKy);

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

            await this._huongDanRepository.InsertAsync(new Models.HuongDan
            {
                MaDT = requestData.MaDT,
                MaSV = requestData.MaSV,
                MaGV = requestData.MaGV
            });

            TempData["AlertMessage"] = "Đăng ký đề tài thành công.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> Unregister(DeTai_Unregister_Param requestData)
        {
            try
            {
                var deTai = await this._deTaiRepository.GetByIdAsync(requestData.MaDT);

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

                await this._huongDanRepository.DeleteAsync(requestData);

                TempData["AlertMessage"] = "Hủy đăng ký thành công!";
                TempData["AlertType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = ex.Message;
                TempData["AlertType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,GiangVien")]
        // GET: DeTaiContrller/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var deTai = await this._deTaiRepository.GetByIdAsync(id);

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
                await this._deTaiRepository.InsertAsync(model);

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
        public async Task<ActionResult> Edit(string id)
        {
            var deTai = await this._deTaiRepository.GetByIdAsync(id);

            if (deTai == null)
            {
                TempData["AlertMessage"] = "Đề tài không tồn tại!";
                TempData["AlertType"] = "error";

                return RedirectToAction(nameof(Index));
            }

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

                await this._deTaiRepository.UpdateAsync(model);

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
