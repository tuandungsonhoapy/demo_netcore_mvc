using demo_netcore_mvc.Configs.ChartConfig;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.Models.ColumnChart;
using demo_netcore_mvc.ObjectData;
using demo_netcore_mvc.RequestData;
using demo_netcore_mvc.UnitOfWork;
using demo_netcore_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace demo_netcore_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(Dashboard_Params queries)
        {
            var deTaiCountByKhoa = await this._unitOfWork.DeTaiRepository.GetDeTaiCountByKhoa(queries) ?? new List<ObjectData.DoughnutChartData>();

            var deTais = await this._unitOfWork.DeTaiRepository.GetAllAsync(new DeTai_GetAll_Param
            {
                MaKhoa = queries.MaKhoa,
                NamHoc = queries.NamHoc,
                HocKy = queries.HocKy
            });

            List<ObjectData.DoughnutChartData> donutChartData = new List<ObjectData.DoughnutChartData>
            {
                new ObjectData.DoughnutChartData { xValue = "Chrome", yValue = 37, DataLabel = "Chrome: 37%" },
                new ObjectData.DoughnutChartData { xValue = "Firefox", yValue = 25, DataLabel = "Firefox: 25%" },
                new ObjectData.DoughnutChartData { xValue = "Safari", yValue = 20, DataLabel = "Safari: 20%" },
                new ObjectData.DoughnutChartData { xValue = "Edge", yValue = 18, DataLabel = "Edge: 18%" }
            };

            ViewBag.Khoas = await this._unitOfWork.KhoaRepository.GetAllAsync(new object());

            ViewBag.NamHocList = await this._unitOfWork.DeTaiRepository.GetAllNamHoc();

            ViewBag.HocKyList = new List<object>
                {
                    new { Value = (byte)1, Text = "Học kỳ 1" },
                    new { Value = (byte)2, Text = "Học kỳ 2" },
                    new { Value = (byte)3, Text = "Học kỳ 3" }
                };

            var deTaiCountByKhoaConfig = new DonutChartConfig
            {
                DataSource = deTaiCountByKhoa,
                CenterLabelText = "Thống kê đề tài<br>theo khoa<br>",
                TooltipFormat = "<b>${point.x}</b><br>Số lượng: <b>${point.y}</b>",
                HoverTextFormat = "${point.x}<br>Số lượng<br>${point.y}",
            };

            var khoas = await this._unitOfWork.KhoaRepository.GetAllWithStudents(queries);

            List<PieChartData> pieChartData = khoas?.Select(khoa => new PieChartData
            {
                X = khoa.TenKhoa,
                Y = khoa.SinhViens.Count,
                Text = $"{khoa.TenKhoa}: {khoa.SinhViens.Count}"
            }).ToList();

            var pieChartConfig = new PieChartConfig
            {
                DataSource = pieChartData
            };

            List<ThongKeSoLuongSlotDeTaiKhoa> columnChartData = new List<ThongKeSoLuongSlotDeTaiKhoa>
            {
                new ThongKeSoLuongSlotDeTaiKhoa { TenKhoa = "Chile", SoLuongSVThamGia = 175000, TongSoSlot = 11300 },
                new ThongKeSoLuongSlotDeTaiKhoa { TenKhoa = "European Union", SoLuongSVThamGia = 140000, TongSoSlot = 135000 },
                new ThongKeSoLuongSlotDeTaiKhoa { TenKhoa = "Turkey", SoLuongSVThamGia = 67000, TongSoSlot = 24000 },
                new ThongKeSoLuongSlotDeTaiKhoa { TenKhoa = "India", SoLuongSVThamGia = 33000, TongSoSlot = 4200 },
                new ThongKeSoLuongSlotDeTaiKhoa { TenKhoa = "Australia", SoLuongSVThamGia = 12000, TongSoSlot = 154000 }
            };

            var seriesList = new List<SeriesInfo>
                {
                    new SeriesInfo { YName = "SoLuongSvThamGia", Name = "Số lượng sinh viên tham gia" },
                    new SeriesInfo { YName = "TongSoSlot", Name = "Tổng số slot đề tài" }
                };

            var seriesListTest = new List<SeriesInfo>
                                        {
                                        new SeriesInfo { YName = "SoLuongSVThamGia", Name = "Số lượng sinh viên tham gia" },
                                        new SeriesInfo { YName = "TongSoSlot", Name = "Tổng slot mở các đề tài của khoa" }
                                        };

            var columnChartDataForConfig = khoas?.Select(khoa => new ThongKeSoLuongSlotDeTaiKhoa
            {
                TenKhoa = khoa.TenKhoa,
                TongSoSlot = khoa.DeTais.Sum(dt => dt?.ToiDa ?? 0),
                SoLuongSVThamGia = khoa.DeTais.Sum(dt => dt.HuongDans?.Count ?? 0)
            });

            double maxValue = columnChartDataForConfig.Any()
        ? Math.Max(
            columnChartDataForConfig.Max(d => d.SoLuongSVThamGia),
            columnChartDataForConfig.Max(d => d.TongSoSlot))
        : 10;

            double maximum, interval;
            if (maxValue <= 100)
            {
                maximum = 100;
                interval = 10;
            }
            else if (maxValue <= 1000)
            {
                maximum = 1000;
                interval = 100;
            }
            else
            {
                maximum = maxValue * 1.2;
                interval = maxValue / 10;
            }

            var check = columnChartDataForConfig.ToList();

            var columnChartConfig = new ColumnChartConfig<ThongKeSoLuongSlotDeTaiKhoa>
            {
                DataSource = check,
                Series = seriesListTest,
                Title = "Thống kê sinh viên tham gia đề tài theo khoa",
                SubTitle = "",
                XName = "TenKhoa",
                PrimaryYAxis = new AxisConfig
                {
                    Minimum = 0,
                    Maximum = maximum,
                    Interval = interval
                }
            };

            return View(new DashboardViewModel
            {
                DoughnutChartData = donutChartData,
                DeTaiCountByKhoaConfig = deTaiCountByKhoaConfig,
                PieChartConfig = pieChartConfig,
                ColumnChartConfig = columnChartConfig,
                MaKhoa = queries.MaKhoa,
                NamHoc = queries.NamHoc,
                HocKy = queries.HocKy,
                TongKinhPhi = deTais?.Sum(dt => dt.KinhPhi) ?? 0,
                TongSVThamGia = deTais?.Sum(dt => dt.HuongDans.Count) ?? 0
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
