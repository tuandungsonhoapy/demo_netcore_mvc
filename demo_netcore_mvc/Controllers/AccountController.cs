using demo_netcore_mvc.IRepositories;
using demo_netcore_mvc.IService;
using demo_netcore_mvc.RequestData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace demo_netcore_mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;
        private readonly IGiangVienRepository _giangVienRepository;
        private readonly ISinhVienRepository _sinhVienRepository;

        public AccountController(IAccountRepository accountRepository, IHashingService hashing, IGiangVienRepository giangVienRepository, ISinhVienRepository sinhVienRepository)
        {
            _accountRepository = accountRepository;
            _hashingService = hashing;
            _giangVienRepository = giangVienRepository;
            _sinhVienRepository = sinhVienRepository;
        }

        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AccountController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Login
        [HttpPost]
        public async Task<ActionResult> Login(Account_Login_Body body)
        {
            try
            {
                var account = await this._accountRepository.GetByUsername(body.Username);

                if (account == null)
                {
                    TempData["AlertMessage"] = "Invalid username or password";
                    TempData["AlertType"] = "error";
                    return View();
                }

                bool isPasswordValid = this._hashingService.VerifyPassword(body.Password, account.Password);

                if (!isPasswordValid)
                {
                    TempData["AlertMessage"] = "Invalid username or password";
                    TempData["AlertType"] = "error";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.Role, account.Role),
                    new Claim("AccountId", account.AccountId.ToString())
                };

                if (account.Role == "GiangVien")
                {
                    var giangVien = await this._giangVienRepository.GetByAccountId(account.AccountId);
                    if (giangVien != null)
                    {
                        claims.Add(new Claim("MaGV", giangVien.MaGV.ToString()));
                    }
                }

                if (account.Role == "SinhVien")
                {
                    var sinhVien = await this._sinhVienRepository.GetByAccountId(account.AccountId);
                    if (sinhVien != null)
                    {
                        claims.Add(new Claim("MaSV", sinhVien.MaSV.ToString()));
                    }
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["AlertMessage"] = "An error occurred during login. Please try again.";
                TempData["AlertType"] = "error";
                return View();
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: AccountController/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: AccountController/Register
        [HttpPost]
        public async Task<ActionResult> Register(Account_Register_Body body)
        {
            try
            {
                await this._accountRepository.Register(body);

                return Ok(new { message = "Registration successful." });
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccountController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
