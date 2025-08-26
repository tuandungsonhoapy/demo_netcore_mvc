using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class KhoaRepository : IKhoaRepository
    {
        private readonly AppDbContext _context;

        public KhoaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(object id)
        {
            var khoa = await _context.Khoa.FindAsync(id);
            if (khoa == null)
                throw new Exception($"Không tìm thấy khoa với mã {id}");

            // kiểm tra ràng buộc: còn sinh viên hoặc giảng viên trực thuộc không?
            bool hasChild = await _context.SinhVien.AnyAsync(sv => sv.MaKhoa == id.ToString())
                         || await _context.GiangVien.AnyAsync(gv => gv.MaKhoa == id.ToString());

            if (hasChild)
                throw new Exception("Không thể xóa Khoa vì còn dữ liệu liên quan (Sinh viên hoặc Giảng viên).");

            _context.Khoa.Remove(khoa);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Khoa>> GetAllAsync(object requestData)
        {
            return await _context.Khoa.FromSqlRaw("EXECUTE SP_Khoa_GetAll").ToListAsync();
        }

        public async Task<Khoa?> GetByIdAsync(object id)
        {
            var maKhoa = id.ToString();

            if (maKhoa == null)
                return null;

            var result = await _context.Khoa
                .FromSqlRaw("EXEC SP_Khoa_GetKhoaById @p0", parameters: maKhoa)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task InsertAsync(Khoa obj)
        {
            // kiểm tra trùng mã khoa
            var exists = await _context.Khoa.AnyAsync(k => k.MaKhoa == obj.MaKhoa || k.TenKhoa == obj.TenKhoa);
            if (exists)
                throw new Exception($"Khoa có mã hoặc tên đã tồn tại.");

            await _context.Khoa.AddAsync(obj);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Khoa obj)
        {
            var existing = await _context.Khoa.FindAsync(obj.MaKhoa);
            if (existing == null)
                throw new Exception($"Không tìm thấy khoa với mã {obj.MaKhoa}");

            // update fields
            existing.TenKhoa = obj.TenKhoa;
            existing.DienThoai = obj.DienThoai;

            _context.Khoa.Update(existing);
            await _context.SaveChangesAsync();
        }
    }
}
