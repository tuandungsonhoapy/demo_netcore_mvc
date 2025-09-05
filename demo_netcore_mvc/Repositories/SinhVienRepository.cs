using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class SinhVienRepository : ISinhVienRepository
    {
        private readonly AppDbContext _context;

        public SinhVienRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(object id)
        {
            int MaSV = (int)id;

            var existingSinhVien = await _context.SinhVien
                .FindAsync(MaSV);

            if (existingSinhVien != null)
            {
                var huongDans = _context.HuongDan.Where(hd => hd.MaSV == MaSV);
                _context.HuongDan.RemoveRange(huongDans);

                _context.SinhVien.Remove(existingSinhVien);
            }
            else
            {
                throw new Exception("Sinh viên không tồn tại.");
            }
        }
        public async Task<List<SinhVien>> GetAllAsync(object requestData)
        {
            var request = requestData as SinhVien_GetAll_Params;

            var query = this._context.SinhVien
                .Include(sv => sv.Khoa)
                .Include(sv => sv.HuongDans)
                    .ThenInclude(hd => hd.DeTai)
                        .ThenInclude(dt => dt.GiangVien)
                .AsQueryable();

            // Lọc theo MaGV (Giảng viên hướng dẫn)
            if (request?.MaGV != null && request?.MaGV != 0)
            {
                query = query.Where(sv => sv.HuongDans != null && sv.HuongDans.Any(hd => hd.DeTai != null && hd.DeTai.GiangVien != null && hd.DeTai.GiangVien.MaGV == request.MaGV));
            }

            // Lọc theo MaDT (Đề tài)
            if (request?.MaDT != null)
            {
                query = query.Where(sv => sv.HuongDans != null && sv.HuongDans.Any(hd => hd.MaDT == request.MaDT));
            }

            // Lọc theo MaKhoa (Khoa)
            if (request?.MaKhoa != null)
            {
                query = query.Where(sv => sv.MaKhoa == request.MaKhoa);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<SinhVien?> GetByAccountId(int accountId)
        {
            return await _context.SinhVien
                .Include(sv => sv.Khoa)
                .Include(sv => sv.HuongDans)
                    .ThenInclude(hd => hd.DeTai)
                        .ThenInclude(dt => dt.GiangVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(sv => sv.AccountId == accountId);
        }

        public async Task<SinhVien?> GetByIdAsync(object id)
        {
            return await _context.SinhVien
                .Include(sv => sv.Khoa)
                .Include(sv => sv.HuongDans)
                    .ThenInclude(hd => hd.DeTai)
                        .ThenInclude(dt => dt.GiangVien)
                .AsNoTracking()
                .FirstOrDefaultAsync(sv => sv.MaSV == (int)id);
        }

        public async Task InsertAsync(SinhVien obj)
        {
            await _context.SinhVien.AddAsync(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SinhVien obj)
        {
            // Tìm sinh viên theo MaSV
            var existingSinhVien = await _context.SinhVien
                .FirstOrDefaultAsync(sv => sv.MaSV == obj.MaSV);

            if (existingSinhVien != null)
            {
                // Cập nhật các trường thông tin
                existingSinhVien.HoTenSV = obj.HoTenSV;
                existingSinhVien.NamSinh = obj.NamSinh;
                existingSinhVien.QueQuan = obj.QueQuan;
                existingSinhVien.MaKhoa = obj.MaKhoa;
            }
            else
            {
                throw new Exception("Sinh viên không tồn tại.");
            }
        }
    }
}
