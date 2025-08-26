using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class GiangVienRepository : IGiangVienRepository
    {
        private readonly AppDbContext _context;

        public GiangVienRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GiangVien>> GetAllAsync(object requestData)
        {
            var raw = await _context.Set<GiangVienData>()
                .FromSqlRaw("EXEC SP_GiangVien_GetAll")
                .AsNoTracking()
                .ToListAsync();

            var result = raw.Select(gv => new GiangVien
            {
                MaGV = gv.MaGV,
                HoTenGV = gv.HoTenGV,
                Luong = gv.Luong,
                MaKhoa = gv.MaKhoa,
                Khoa = new Khoa
                {
                    MaKhoa = gv.MaKhoa,
                    TenKhoa = gv.TenKhoa,
                    DienThoai = gv.DienThoai
                }
            }).ToList();

            return result;
        }

        public Task<GiangVien?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(GiangVien obj)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(GiangVien obj)
        {
            throw new NotImplementedException();
        }
    }
}
