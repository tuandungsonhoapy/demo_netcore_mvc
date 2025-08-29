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

            // Gom nhóm theo MaSV
            var result = raw
                .GroupBy(sv => sv.MaSV)
                .Select(g =>
                {
                    var first = g.First(); // lấy 1 record làm đại diện
                    return new SinhVien
                    {
                        MaSV = first.MaSV,
                        HoTenSV = first.HoTenSV,
                        NamSinh = first.NamSinh,
                        QueQuan = first.QueQuan,
                        MaKhoa = first.MaKhoa,
                        Khoa = new Khoa
                        {
                            MaKhoa = first.MaKhoa,
                            TenKhoa = first.TenKhoa
                        },
                        HuongDans = g
                            .Where(x => x.MaDT != null || x.MaGV != null)
                            .Select(x => new HuongDan
                            {
                                MaSV = x.MaSV,
                                MaDT = x.MaDT,
                                KetQua = x.KetQua,
                                GiangVien = x.MaGV.HasValue ? new GiangVien
                                {
                                    MaGV = x.MaGV.Value,
                                    HoTenGV = x.HoTenGV
                                } : null,
                                DeTai = x.MaDT != null ? new DeTai
                                {
                                    MaDT = x.MaDT,
                                    TenDT = x.TenDT
                                } : null
                            })
                            .ToList()
                    };
                })
                .ToList();

            return result;
        }

        public async Task<SinhVien?> GetByAccountId(int accountId)
        {
            var raw = await _context.Set<SinhVien>()
                .FromSqlRaw("EXEC SP_SinhVien_GetByAccountId @AccountId = {0}", accountId)
                .AsNoTracking()
                .ToListAsync();

            return raw.FirstOrDefault();
        }

        public async Task<SinhVien?> GetByIdAsync(object id)
        {
            int MaSV = (int)id;

            var raw = await this._context.Set<SinhVienData>()
                .FromSqlRaw("EXEC SP_SinhVien_GetById @MaSV = {0}", MaSV)
                .AsNoTracking()
                .ToListAsync();

            var sv = raw
                .GroupBy(s => s.MaSV)
                .Select(g =>
                {
                    var first = g.First(); // lấy 1 record làm đại diện
                    return new SinhVien
                    {
                        MaSV = first.MaSV,
                        HoTenSV = first.HoTenSV,
                        NamSinh = first.NamSinh,
                        QueQuan = first.QueQuan,
                        MaKhoa = first.MaKhoa,
                        Khoa = new Khoa
                        {
                            MaKhoa = first.MaKhoa,
                            TenKhoa = first.TenKhoa
                        },
                        HuongDans = g
                            .Where(x => x.MaDT != null || x.MaGV != null)
                            .Select(x => new HuongDan
                            {
                                MaSV = x.MaSV,
                                MaDT = x.MaDT,
                                KetQua = x.KetQua,
                                GiangVien = x.MaGV.HasValue ? new GiangVien
                                {
                                    MaGV = x.MaGV.Value,
                                    HoTenGV = x.HoTenGV
                                } : null,
                                DeTai = x.MaDT != null ? new DeTai
                                {
                                    MaDT = x.MaDT,
                                    TenDT = x.TenDT
                                } : null
                            })
                            .ToList()
                    };
                })
                .FirstOrDefault();

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
