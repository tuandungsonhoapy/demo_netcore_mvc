using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.Helper;
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

            var maGV = DbHelper.ToDbValue(request?.MaGV);
            var maKhoa = DbHelper.ToDbValue(request?.MaKhoa);
            var namHoc = DbHelper.ToDbValue(request?.NamHoc);
            var hocKy = DbHelper.ToDbValue(request?.HocKy);

            var raw = await this._context.Set<DeTaiData>()
                .FromSqlRaw("EXEC SP_DeTai_GetAll @MaGV = {0}, @MaKhoa = {1}, @NamHoc = {2}, @HocKy = {3}", maGV, maKhoa, namHoc, hocKy)
                .AsNoTracking()
                .ToListAsync();

            var result = raw
                .GroupBy(dt => dt.MaDT)
                .Select(g =>
                {
                    var first = g.First();
                    return new DeTai
                    {
                        MaDT = first.MaDT,
                        TenDT = first.TenDT,
                        KinhPhi = first.KinhPhi,
                        NoiThucTap = first.NoiThucTap,
                        SoLuong = first.SoLuong,
                        ToiDa = first.ToiDa,
                        NamHoc = first.NamHoc,
                        HocKy = first.HocKy,
                        IsOpen = first.IsOpen ?? false,
                        GiangVien = first.MaGV.HasValue ? new GiangVien
                        {
                            MaGV = first.MaGV.Value,
                            HoTenGV = first.HoTenGV,
                            Khoa = first.MaKhoa != null ? new Khoa
                            {
                                MaKhoa = first.MaKhoa,
                                TenKhoa = first.TenKhoa
                            } : null
                        } : null,
                        HuongDans = g
                            .Where(x => x.MaGV != null && x.MaSV != null)
                            .Select(x => new HuongDan
                            {
                                MaDT = x.MaDT,
                                MaSV = x.MaSV ?? 0,
                                GiangVien = x.MaGV.HasValue ? new GiangVien
                                {
                                    MaGV = x.MaGV.Value,
                                    HoTenGV = x.HoTenGV,
                                    Khoa = x.MaKhoa != null ? new Khoa
                                    {
                                        MaKhoa = x.MaKhoa,
                                        TenKhoa = x.TenKhoa
                                    } : null
                                } : null,
                            }).ToList()
                    };
                });

            return result.ToList();
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

            var raw = await this._context.Set<DeTaiDetailData>()
                .FromSqlRaw("EXEC SP_DeTai_GetById @MaDT = {0}", maDT)
                .AsNoTracking()
                .ToListAsync();

            var result = raw
                .GroupBy(dt => dt.MaDT)
                .Select(g =>
                {
                    var first = g.First();
                    return new DeTai
                    {
                        MaDT = first.MaDT,
                        TenDT = first.TenDT,
                        KinhPhi = first.KinhPhi,
                        NoiThucTap = first.NoiThucTap,
                        SoLuong = first.SoLuong,
                        ToiDa = first.ToiDa,
                        NamHoc = first.NamHoc,
                        HocKy = first.HocKy,
                        IsOpen = first.IsOpen ?? false,
                        NguoiHuongDan = first.NguoiHuongDan ?? 0,
                        GiangVien = first.MaGV.HasValue ? new GiangVien
                        {
                            MaGV = first.MaGV.Value,
                            HoTenGV = first.HoTenGV,
                            Khoa = first.MaKhoa != null ? new Khoa
                            {
                                MaKhoa = first.MaKhoa,
                                TenKhoa = first.TenKhoa
                            } : null
                        } : null,
                        HuongDans = g
                            .Where(x => x.MaGV != null && x.MaSV.HasValue)
                            .Select(x => new HuongDan
                            {
                                MaDT = x.MaDT,
                                MaSV = x.MaSV.HasValue ? x.MaSV.Value : 0,
                                KetQua = x.KetQua,
                                GiangVien = x.MaGV.HasValue ? new GiangVien
                                {
                                    MaGV = x.MaGV.Value,
                                    HoTenGV = x.HoTenGV,
                                    Khoa = x.MaKhoa != null ? new Khoa
                                    {
                                        MaKhoa = x.MaKhoa,
                                        TenKhoa = x.TenKhoa
                                    } : null
                                } : null,
                                SinhVien = x.MaSV.HasValue ? new SinhVien
                                {
                                    MaSV = x.MaSV.Value,
                                    HoTenSV = x.HoTenSV,
                                    Khoa = x.MaKhoa != null ? new Khoa
                                    {
                                        MaKhoa = x.MaKhoa,
                                        TenKhoa = x.TenKhoa
                                    } : null
                                } : null
                            }).ToList()
                    };
                }).FirstOrDefault();

            return result;
        }

        public async Task<List<SinhVien_DeTai_HK_Data>> GetSVThamGiaTheoKy(int MaSV, string NamHoc, byte HocKy)
        {
            var raw = await this._context.Set<SinhVien_DeTai_HK_Data>()
                .FromSqlRaw("EXEC SP_DeTai_SVThamGiaTheoKy @MaSV = {0}, @NamHoc = {1}, @HocKy = {2}", MaSV, NamHoc, HocKy)
                .AsNoTracking()
                .ToListAsync();

            return raw;
        }

        public async Task InsertAsync(DeTai obj)
        {
            await this._context.DeTai.AddAsync(obj);
            await this._context.SaveChangesAsync();
        }

        public async Task<List<DeTai>> MyDeTai(DeTai_MyDeTai_Queries requestParams)
        {
            var raw = await this._context.Set<DeTaiData>()
                .FromSqlRaw("EXEC SP_DeTai_MyDeTai @MaSV = {0}, @NamHoc = {1}, @HocKy = {2}", requestParams.MaSV, requestParams.NamHoc, requestParams.HocKy)
                .AsNoTracking()
                .ToListAsync();

            var result = raw
                .GroupBy(dt => dt.MaDT)
                .Select(g =>
                {
                    var first = g.First();
                    return new DeTai
                    {
                        MaDT = first.MaDT,
                        TenDT = first.TenDT,
                        KinhPhi = first.KinhPhi,
                        NoiThucTap = first.NoiThucTap,
                        SoLuong = first.SoLuong,
                        ToiDa = first.ToiDa,
                        NamHoc = first.NamHoc,
                        HocKy = first.HocKy,
                        IsOpen = first.IsOpen ?? false,
                        GiangVien = first.MaGV.HasValue ? new GiangVien
                        {
                            MaGV = first.MaGV.Value,
                            HoTenGV = first.HoTenGV,
                            Khoa = first.MaKhoa != null ? new Khoa
                            {
                                MaKhoa = first.MaKhoa,
                                TenKhoa = first.TenKhoa
                            } : null
                        } : null,
                        HuongDans = g
                            .Where(x => x.MaGV != null && x.MaSV != null)
                            .Select(x => new HuongDan
                            {
                                MaDT = x.MaDT,
                                MaSV = x.MaSV ?? 0,
                                GiangVien = x.MaGV.HasValue ? new GiangVien
                                {
                                    MaGV = x.MaGV.Value,
                                    HoTenGV = x.HoTenGV,
                                    Khoa = x.MaKhoa != null ? new Khoa
                                    {
                                        MaKhoa = x.MaKhoa,
                                        TenKhoa = x.TenKhoa
                                    } : null
                                } : null,
                            }).ToList()
                    };
                });

            return result.ToList();
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
                await this._context.SaveChangesAsync();
            }
        }
    }
}
