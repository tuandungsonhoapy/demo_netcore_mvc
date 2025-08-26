using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.Helper;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.ObjectData;
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
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Sinh viên không tồn tại.");
            }
        }

        public async Task<List<SinhVien>> GetAllAsync(object requestData)
        {
            var request = requestData as SinhVien_GetAll_Params;

            var maGV = DbHelper.ToDbValue(request?.MaGV);
            var maDT = DbHelper.ToDbValue(request?.MaDT);
            var maKhoa = DbHelper.ToDbValue(request?.MaKhoa);

            var raw = await this._context.Set<SinhVienData>()
                .FromSqlRaw("EXEC SP_SinhVien_GetAll @MaGV = {0}, @MaDT = {1}, @MaKhoa = {2}", maGV, maDT, maKhoa)
                .AsNoTracking()
                .ToListAsync();

            var result = raw.Select(static sv => new SinhVien
            {
                MaSV = sv.MaSV,
                HoTenSV = sv.HoTenSV,
                NamSinh = sv.NamSinh,
                QueQuan = sv.QueQuan,
                MaKhoa = sv.MaKhoa,
                Khoa = new Khoa
                {
                    MaKhoa = sv.MaKhoa,
                    TenKhoa = sv.TenKhoa
                },
                HuongDans = new List<HuongDan>
                {
                    new HuongDan
                    {
                        MaSV = sv.MaSV,
                        MaDT = sv.MaDT,
                        KetQua = sv.KetQua,
                        GiangVien = sv.MaGV.HasValue ? new GiangVien
                        {
                            MaGV = sv.MaGV.Value,
                            HoTenGV = sv.HoTenGV
                        } : null,
                        DeTai = sv.MaDT != null ? new DeTai
                        {
                            MaDT = sv.MaDT,
                            TenDT = sv.TenDT
                        } : null
                    }
                }
            }).ToList();

            return result;
        }

        public async Task<SinhVien?> GetByIdAsync(object id)
        {
            int MaSV = (int)id;

            var raw = await this._context.Set<SinhVienData>()
                .FromSqlRaw("EXEC SP_SinhVien_GetById @MaSV = {0}", MaSV)
                .AsNoTracking()
                .ToListAsync();

            var sv = raw.Select(static sv => new SinhVien
            {
                MaSV = sv.MaSV,
                HoTenSV = sv.HoTenSV,
                NamSinh = sv.NamSinh,
                QueQuan = sv.QueQuan,
                MaKhoa = sv.MaKhoa,
                Khoa = new Khoa
                {
                    MaKhoa = sv.MaKhoa,
                    TenKhoa = sv.TenKhoa
                },
                HuongDans = new List<HuongDan>
                {
                    new HuongDan
                    {
                        MaSV = sv.MaSV,
                        MaDT = sv.MaDT,
                        KetQua = sv.KetQua,
                        GiangVien = sv.MaGV.HasValue ? new GiangVien
                        {
                            MaGV = sv.MaGV.Value,
                            HoTenGV = sv.HoTenGV
                        } : null,
                        DeTai = sv.MaDT != null ? new DeTai
                        {
                            MaDT = sv.MaDT,
                            TenDT = sv.TenDT
                        } : null
                    }
                }
            }).FirstOrDefault();

            return sv;
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

                // existingSinhVien.Khoa = obj.Khoa;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Sinh viên không tồn tại.");
            }
        }
    }
}
