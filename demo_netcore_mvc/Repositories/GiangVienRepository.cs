using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.Helper;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
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

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GiangVien>> GetAllAsync(object requestData)
        {
            var request = requestData as GiangVien_GetAll_Param;

            var MaKhoa = DbHelper.ToDbValue(request?.MaKhoa);

            var raw = await _context.Set<GiangVienData>()
                .FromSqlRaw("EXEC SP_GiangVien_GetAll @MaKhoa = {0}", MaKhoa)
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
                }
            }).ToList();

            return result;
        }

        public async Task<GiangVien?> GetByAccountId(int accountId)
        {
            var raw = await _context.Set<GiangVien>()
                .FromSqlRaw("EXEC SP_GiangVien_GetByAccountId @AccountId = {0}", accountId)
                .AsNoTracking()
                .ToListAsync();

            return raw.FirstOrDefault();
        }

        public async Task<GiangVien?> GetByIdAsync(object id)
        {
            int MaGV = (int)id;

            var raw = await this._context.Set<GiangVienDetailData>()
                .FromSqlRaw("EXEC SP_GiangVien_GetById @MaGV = {0}", MaGV)
                .AsNoTracking()
                .ToListAsync();

            var giangVien = raw.GroupBy(gv => gv.MaGV)
                .Select(GiangVienData => new GiangVien
                {
                    MaGV = GiangVienData.Key,
                    HoTenGV = GiangVienData.First().HoTenGV,
                    Luong = GiangVienData.First().Luong,
                    MaKhoa = GiangVienData.First().MaKhoa,
                    Khoa = new Khoa
                    {
                        MaKhoa = GiangVienData.First().MaKhoa,
                        TenKhoa = GiangVienData.First().TenKhoa
                    },
                    HuongDans = GiangVienData
                        .Where(x => x.MaDT != null)
                        .Select(x => new HuongDan
                        {
                            DeTai = x.MaDT != null ? new DeTai
                            {
                                MaDT = x.MaDT,
                                TenDT = x.TenDT,
                                ToiDa = x.ToiDa,
                                SoLuong = x.SoLuong
                            } : null
                        })
                        .ToList()
                }).FirstOrDefault();

            return giangVien;
        }

        public Task InsertAsync(GiangVien obj)
        {
            throw new NotImplementedException();
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

            await _context.SaveChangesAsync();
        }
    }
}
