using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class HuongDanRepository : IHuongDanRepository
    {
        private readonly AppDbContext _context;

        public HuongDanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(object id)
        {
            var data = id as DeTai_Unregister_Param;

            if (data != null)
            {
                var existingHuongDan = await _context.HuongDan
                    .FirstOrDefaultAsync(hd => hd.MaDT == data.MaDT && hd.MaSV == data.MaSV);
                if (existingHuongDan != null)
                {
                    _context.HuongDan.Remove(existingHuongDan);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Hướng dẫn không tồn tại.");
                }
            }
            else
            {
                throw new Exception("Dữ liệu không hợp lệ.");
            }
        }

        public Task<List<HuongDan>> GetAllAsync(object requestData)
        {
            throw new NotImplementedException();
        }

        public Task<HuongDan?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<HuongDan> GetListByDeTai(int MaDT)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(HuongDan obj)
        {
            await this._context.HuongDan.AddAsync(obj);
        }

        public async Task UpdateAsync(HuongDan obj)
        {
            var hd = await this._context.HuongDan.FindAsync(obj.MaSV, obj.MaDT);

            if (hd != null)
            {
                hd.KetQua = obj.KetQua;
            }
        }
    }
}
