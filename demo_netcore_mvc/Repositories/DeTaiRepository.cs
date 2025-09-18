using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
using demo_netcore_mvc.RequestData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class DeTaiRepository : IDetaiRepository
    {

        private readonly AppDbContext _context;

        public DeTaiRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DeTai>> GetAllAsync(object requestData)
        {
            var request = requestData as DeTai_GetAll_Param;

            var query = _context.DeTai
                .Include(dt => dt.GiangVien)
                    .ThenInclude(gv => gv.Khoa)
                .Include(dt => dt.HuongDans)
                    .ThenInclude(hd => hd.SinhVien)
                .AsQueryable();

            if (request?.MaGV != null)
                query = query.Where(dt => dt.GiangVien != null && dt.GiangVien.MaGV == request.MaGV);

            if (!string.IsNullOrEmpty(request?.MaKhoa))
                query = query.Where(dt => dt.GiangVien != null && dt.GiangVien.MaKhoa == request.MaKhoa);

            if (!string.IsNullOrEmpty(request?.NamHoc))
                query = query.Where(dt => dt.NamHoc == request.NamHoc);

            if (request?.HocKy != null)
                query = query.Where(dt => dt.HocKy == request.HocKy);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<NamHocData>> GetAllNamHoc()
        {
            var raw = await this._context.Set<NamHocData>()
                .FromSqlRaw("EXEC SP_DeTai_GetAllNamHoc")
                .AsNoTracking()
                .ToListAsync();

            return raw;
        }

        public async Task<DeTai?> GetByIdAsync(object id)
        {
            var maDT = id.ToString();

            return await _context.DeTai
                .Include(dt => dt.GiangVien)
                    .ThenInclude(gv => gv.Khoa)
                .Include(dt => dt.HuongDans)
                    .ThenInclude(hd => hd.SinhVien)
                        .ThenInclude(sv => sv.Khoa)
                .AsNoTracking()
                .FirstOrDefaultAsync(dt => dt.MaDT == maDT);
        }

        public async Task<List<ObjectData.DoughnutChartData>> GetDeTaiCountByKhoa(Dashboard_Params pars)
        {
            var latestDeTais = await GetAllAsync(new DeTai_GetAll_Param
            {
                NamHoc = pars.NamHoc,
                HocKy = pars.HocKy == 0 ? null : pars.HocKy,
                MaKhoa = pars.MaKhoa
            }) ?? new List<DeTai>();

            var result = latestDeTais
                .GroupBy(dt => new
                {
                    dt.GiangVien?.Khoa?.MaKhoa,
                    dt.GiangVien?.Khoa?.TenKhoa
                })
                .Select(g => new ObjectData.DoughnutChartData
                {
                    xValue = g.Key.TenKhoa ?? "Chưa xác định",
                    yValue = g.Count(),
                    DataLabel = $"{g.Key.TenKhoa ?? "Chưa xác định"}: {g.Count()}"
                })
                .ToList();

            // Chuẩn hóa yValue để tổng bằng 100 (nếu cần)
            if (!result.Any())
            {
                // Thêm dữ liệu mặc định nếu không có kết quả
                result.Add(new ObjectData.DoughnutChartData
                {
                    xValue = "Không có dữ liệu",
                    yValue = 100,
                    DataLabel = "Không có dữ liệu: 100%"
                });
            }

            return result;
        }

        public async Task<List<DeTai>> GetLatestHocKyDeTais(Dashboard_Params pars)
        {
            var query = _context.DeTai
                .Include(dt => dt.GiangVien)
                    .ThenInclude(gv => gv.Khoa)
                .Include(dt => dt.HuongDans)
                    .ThenInclude(hd => hd.SinhVien)
                .AsNoTracking();

            // Áp dụng lọc NamHoc nếu không null
            if (!string.IsNullOrEmpty(pars.NamHoc))
            {
                query = query.Where(dt => dt.NamHoc == pars.NamHoc);
            }

            // Áp dụng lọc HocKy nếu không null
            if (pars.HocKy.HasValue && pars.HocKy != 0)
            {
                query = query.Where(dt => dt.HocKy == pars.HocKy);
            }

            // Áp dụng lọc MaKhoa nếu không null
            if (!string.IsNullOrEmpty(pars.MaKhoa))
            {
                query = query.Where(dt => dt.GiangVien.Khoa.MaKhoa == pars.MaKhoa);
            }

            return await query.ToListAsync() ?? new List<DeTai>();
        }

        public async Task<List<DeTai>> GetSVThamGiaTheoKy(int MaSV, string NamHoc, byte HocKy)
        {
            return await _context.DeTai
                .Where(dt => dt.NamHoc == NamHoc
                          && dt.HocKy == HocKy
                          && dt.HuongDans.Any(hd => hd.MaSV == MaSV))
                .Include(dt => dt.GiangVien)
                    .ThenInclude(gv => gv.Khoa)
                .Include(dt => dt.HuongDans)
                    .ThenInclude(h => h.SinhVien)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task InsertAsync(DeTai obj)
        {
            await this._context.DeTai.AddAsync(obj);
        }

        public async Task<List<DeTai>> MyDeTai(DeTai_MyDeTai_Queries requestParams)
        {
            var query = this._context.DeTai
                .Include(dt => dt.GiangVien)
                    .ThenInclude(gv => gv.Khoa)
                .Include(dt => dt.HuongDans)
                    .ThenInclude(hd => hd.SinhVien)
                    .ThenInclude(sv => sv.Khoa)
                .Where(dt => dt.HuongDans.Any(hd => hd.MaSV == requestParams.MaSV))
                .AsQueryable();

            if (!string.IsNullOrEmpty(requestParams?.NamHoc))
                query = query.Where(dt => dt.NamHoc == requestParams.NamHoc);

            if (requestParams?.HocKy != null)
                query = query.Where(dt => dt.HocKy == requestParams.HocKy);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(DeTai obj)
        {
            var deTai = await this._context.DeTai.FindAsync(obj.MaDT);

            if (deTai != null)
            {
                deTai.TenDT = obj.TenDT;
                deTai.KinhPhi = obj.KinhPhi;
                deTai.NoiThucTap = obj.NoiThucTap;
                deTai.ToiDa = obj.ToiDa;
                deTai.NamHoc = obj.NamHoc;
                deTai.HocKy = obj.HocKy;
                deTai.NguoiHuongDan = obj.NguoiHuongDan;
                deTai.IsOpen = obj.IsOpen;
            }
        }
    }
}
