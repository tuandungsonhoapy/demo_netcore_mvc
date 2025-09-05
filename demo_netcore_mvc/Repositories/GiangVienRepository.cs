using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
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

        public async Task DeleteAsync(object id)
        {
            var giangVien = await this._context.GiangVien.
                Include(gv => gv.DeTais)
                    .ThenInclude(dt => dt.HuongDans)
                .FirstOrDefaultAsync(gv => gv.MaGV == (int)id);

            if (giangVien?.DeTais != null && giangVien?.DeTais.Where(dt => dt.IsOpen).Count() > 0)
            {
                throw new Exception("Giảng viên đang hướng dẫn đề tài, không thể xóa");
            }

            if (giangVien != null)
            {
                this._context.GiangVien.Remove(giangVien);
                this._context.Account.Remove(await this._context.Account.FindAsync(giangVien.AccountId));
            }
            else
            {
                throw new Exception("Giảng viên không tồn tại");
            }
        }

        public async Task<List<GiangVien>> GetAllAsync(object requestData)
        {
            var request = requestData as GiangVien_GetAll_Param;

            var result = this._context.GiangVien
                .Include(gv => gv.Khoa)
                .Include(gv => gv.DeTais)
                    .ThenInclude(dt => dt.HuongDans)
                        .ThenInclude(hd => hd.SinhVien)
                            .ThenInclude(sv => sv.Khoa)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request?.MaKhoa))
            {
                result = result.Where(gv => gv.MaKhoa == request.MaKhoa);
            }

            return await result.AsNoTracking().ToListAsync();
        }

        public async Task<GiangVien?> GetByAccountId(int accountId)
        {
            var result = await this._context.GiangVien
                .Include(gv => gv.Khoa)
                .Include(gv => gv.DeTais)
                    .ThenInclude(dt => dt.HuongDans)
                .FirstOrDefaultAsync(gv => gv.AccountId == accountId);

            return result;
        }

        public async Task<GiangVien?> GetByIdAsync(object id)
        {
            var query = await this._context.GiangVien
                .Include(gv => gv.Khoa)
                .Include(gv => gv.DeTais)
                    .ThenInclude(dt => dt.HuongDans)
                .FirstOrDefaultAsync(gv => gv.MaGV == (int)id);

            return query;
        }

        public async Task InsertAsync(GiangVien obj)
        {
            await this._context.GiangVien.AddAsync(obj);
        }

        public async Task UpdateAsync(GiangVien obj)
        {
            var giangVien = await this._context.GiangVien.FindAsync(obj.MaGV);

            if (giangVien == null)
            {
                throw new Exception("Giảng viên không tồn tại");
            }

            giangVien.Luong = obj.Luong;
            giangVien.HoTenGV = obj.HoTenGV;
            giangVien.MaKhoa = obj.MaKhoa;
        }
    }
}
