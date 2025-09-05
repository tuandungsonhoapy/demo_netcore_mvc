using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;

namespace demo_netcore_mvc.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IDetaiRepository DeTaiRepository { get; }

        public IGiangVienRepository GiangVienRepository { get; }

        public IHuongDanRepository HuongDanRepository { get; }

        public IKhoaRepository KhoaRepository { get; }

        public ISinhVienRepository SinhVienRepository { get; }

        public UnitOfWork(
            IDetaiRepository deTaiRepository,
            IGiangVienRepository giangVienRepository,
            IHuongDanRepository huongDanRepository,
            IKhoaRepository khoaRepository,
            ISinhVienRepository sinhVienRepository,
            AppDbContext context
        )
        {
            DeTaiRepository = deTaiRepository;
            GiangVienRepository = giangVienRepository;
            HuongDanRepository = huongDanRepository;
            KhoaRepository = khoaRepository;
            SinhVienRepository = sinhVienRepository;
            _context = context;
        }

        public void Dispose()
        {
            this._context?.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
