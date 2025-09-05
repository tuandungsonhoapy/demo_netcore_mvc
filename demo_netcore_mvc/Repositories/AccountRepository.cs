using demo_netcore_mvc.AppDBContext;
using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.IService;
using demo_netcore_mvc.Models;
using demo_netcore_mvc.RequestData;
using Microsoft.EntityFrameworkCore;

namespace demo_netcore_mvc.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;
        private readonly IHashingService _hashingService;

        public AccountRepository(AppDbContext context, IHashingService hashingService)
        {
            _context = context;
            _hashingService = hashingService;
        }

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Account>> GetAllAsync(object requestData)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<Account?> GetByUsername(string username)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(a => a.Username == username);

            return account;
        }

        public async Task InsertAsync(Account obj)
        {
            await _context.Set<Account>().AddAsync(obj);
            await _context.SaveChangesAsync();
        }

        public async Task Register(Account_Register_Body body)
        {
            if (body.Password != body.ConfirmPassword)
                throw new Exception("Mật khẩu và xác nhận mật khẩu không khớp.");

            var exists = await _context.Set<Account>()
                .AnyAsync(a => a.Username == body.Username);
            if (exists)
                throw new Exception("Username đã tồn tại.");

            var hashedPassword = _hashingService.HashPassword(body.Password);

            var account = new Account
            {
                Username = body.Username,
                Password = hashedPassword,
                Role = body.Role
            };
            await _context.Set<Account>().AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(Account obj)
        {
            throw new NotImplementedException();
        }
    }
}
