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
