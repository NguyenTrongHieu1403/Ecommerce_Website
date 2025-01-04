using ecommerce_final.Entities;
using ecommerce_final.Models;
using ecommerce_final.Service;
using ecommerce_final.Extensions;
using ecommerce_final.Tool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using ecommerce_final.Models;

namespace ecommerce_final.Controllers
{
    public class AuthController : Controller
    {
        private readonly EcommerceFinalContext _context;
        private readonly OtpService _otpService;
        private readonly EmailService _emailService;
        private readonly OrderService _orderservice;

        public AuthController(EcommerceFinalContext context, OtpService otpService, EmailService emailService, OrderService orderservice)
        {
            _context = context;
            _otpService = otpService;
            _emailService = emailService;
            _orderservice = orderservice;
        }


        [Authorize]
        public IActionResult Index()
        {
            var hoTen = User.Claims.FirstOrDefault(c => c.Type == "HoTen")?.Value;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chuyển userId từ string sang int (nếu cần)
            if (!int.TryParse(userId, out int UserId))
            {
                // Nếu không thể chuyển đổi, trả về trang login hoặc hiển thị thông báo lỗi
                return RedirectToAction("Login", "Account");
            }

            // Lấy tất cả đơn hàng và chi tiết hóa đơn cho người dùng
            var orderDetails = _orderservice.GetOrderDetailsByUser(UserId);

            // Truyền dữ liệu vào ViewBag hoặc model
            ViewBag.HOTEN = hoTen;

            var model = new DashboardViewModel
            {
                HoTen = hoTen,
                OrderDetails = orderDetails // Danh sách chi tiết hóa đơn
            };

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra xem email đã tồn tại chưa
            var existingEmail = _context.Users.Any(u => u.Email == model.Email);
            if (existingEmail)
            {
                TempData["ErrorMessage"] = "Email này đã được đăng ký.";
                return View(model);
            }

            // Kiểm tra xem username đã tồn tại chưa
            var existingUsername = _context.Users.Any(u => u.Username == model.Username);
            if (existingUsername)
            {
                TempData["ErrorMessage"] = "Tên người dùng này đã được sử dụng.";
                return View(model);
            }
            if (model.PasswordHash != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu không trùng khớp.";
                return View(model);
            }

            int otpId; // Biến để lưu lại OtpId

            // Gửi OTP
            var otpSent = _otpService.GenerateAndSendOtp(model.Email, "Register", out otpId);
            if (!otpSent)
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng.";
                return View(model);
            }

            // Lưu thông tin tạm thời vào Session
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("PasswordHash", model.PasswordHash);
            HttpContext.Session.SetInt32("OtpId", otpId); // Lưu thêm OtpId


            return RedirectToAction("VerifyOtpRegister");
        }


        [HttpGet]
        public IActionResult verifyOtpRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtpRegister(string otpCode)
        {
            var email = HttpContext.Session.GetString("Email");
            var username = HttpContext.Session.GetString("Username");
            var passwordHash = HttpContext.Session.GetString("PasswordHash");

            // Lấy OtpId từ Session
            var otpId = HttpContext.Session.GetInt32("OtpId");

            if (otpId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin OTP. Vui lòng đăng ký lại.";
                return RedirectToAction("Register");
            }

            // Xác minh OTP
            var verified = _otpService.VerifyOtp(email, otpId.Value, otpCode, "Register", out var errorMessage);
            if (!verified)
            {
                // Kiểm tra nếu hết số lần thử
                if (errorMessage == "Bạn đã hết số lần thử.")
                {
                    // Reset session để cho phép người dùng nhập lại thông tin và yêu cầu OTP mới
                    HttpContext.Session.Clear();

                    TempData["ErrorMessage"] = "Bạn đã hết số lần thử. Vui lòng yêu cầu mã OTP mới.";
                    return RedirectToAction("Register");  // Quay lại trang đăng ký để người dùng nhập lại thông tin
                }

                TempData["ErrorMessage"] = errorMessage;
                return View();
            }

            try
            {
                // Tạo tài khoản người dùng
                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    CreatedAt = DateTime.Now,
                    RoleId = 2,  // Đặt quyền cho người dùng
                    EmailVerify = true,
                    Active = true, // Trạng thái tài khoản
                    HashSalt = GetRandom.Random() // Salt cho mật khẩu
                };

                newUser.PasswordHash = passwordHash.ToMd5Hash(newUser.HashSalt);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                // Sau khi tạo User, liên kết UserId với OtpRequest
                var otpRequest = _context.Otps
                    .Where(o => o.OtpCode == otpCode && o.Purpose == "Register")
                    .FirstOrDefault();

                if (otpRequest != null)
                {
                    otpRequest.IsUsed = true; // Đánh dấu là đã sử dụng
                    otpRequest.UserId = newUser.Id; // Gắn UserId sau khi tạo User
                    _context.Otps.Update(otpRequest);
                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                HttpContext.Session.Clear();

                return RedirectToAction("Login");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo tài khoản: {ex.Message}";
                return View();
            }
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                return View(model);
            }

            // Kiểm tra xem người dùng nhập Email hay Username
            var user = _context.Users
                            .FirstOrDefault(u => u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc Email không đúng.";
                return View(model);
            }

            if (user.PasswordHash != model.PasswordHash.ToMd5Hash(user.HashSalt))
            {
                ViewBag.ThongBaoLoi = "Đăng nhập không thành công";
                return View(model);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("HoTen", (user.FirstName+user.LastName).ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "User"),
                };

            var claimsIdentity = new ClaimsIdentity(
             claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimPrincipal);

            // Kiểm tra role, nếu là Admin thì chuyển hướng tới trang Admin
            if (user.RoleId == 1)
            {
                return RedirectToAction("Index", "Admin"); // Trang Admin Index
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Kiểm tra xem email đã tồn tại chưa
            var existingEmail = _context.Users.Any(u => u.Email == model.Email);
            if (!existingEmail)
            {
                TempData["ErrorMessage"] = "Email này đã được chưa được đăng ký.";
                return View(model);
            }

            int otpId; // Biến để lưu lại OtpId

            // Gửi OTP
            var otpSent = _otpService.GenerateAndSendOtp(model.Email, "Reset Password", out otpId);
            if (!otpSent)
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng.";
                return View(model);
            }

            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetInt32("OtpId", otpId); // Lưu OtpId vào Session
            TempData["SuccessMessage"] = "OTP đã được gửi tới email của bạn.";

            return RedirectToAction("VerifyOtpForgotPassword");

        }


        [HttpGet]
        public IActionResult VerifyOtpForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtpForgotPassword(string otpCode)
        {
            var email = HttpContext.Session.GetString("Email");
            var otpId = HttpContext.Session.GetInt32("OtpId");

            if (otpId == null || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin OTP. Vui lòng yêu cầu lại mã OTP.";
                return RedirectToAction("ForgotPassword");
            }

            // Xác minh OTP
            var verified = _otpService.VerifyOtp(email, otpId.Value, otpCode, "Reset Password", out var errorMessage);
            if (!verified)
            {
                // Kiểm tra nếu hết số lần thử
                if (errorMessage == "Bạn đã hết số lần thử.")
                {
                    // Reset session để cho phép người dùng nhập lại thông tin và yêu cầu OTP mới
                    HttpContext.Session.Clear();

                    TempData["ErrorMessage"] = "Bạn đã hết số lần thử. Vui lòng yêu cầu mã OTP mới.";
                    return RedirectToAction("Register");  // Quay lại trang đăng ký để người dùng nhập lại thông tin
                }

                TempData["ErrorMessage"] = errorMessage;
                return View();
            }

            try
            {
                // Tạo mật khẩu mới ngẫu nhiên
                var newPassword = GenerateRandomPassword();

                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                    return View();
                }

                // Cập nhật mật khẩu mới vào cơ sở dữ liệu
                user.PasswordHash = newPassword.ToMd5Hash(user.HashSalt);
                _context.Users.Update(user);
                _context.SaveChanges();

                // Gửi mật khẩu mới qua email
                var subject = "Mật khẩu mới của bạn";
                var body = $"Mật khẩu mới của bạn là: {newPassword}";

                _emailService.SendEmail(user.Email, subject, body);

                TempData["SuccessMessage"] = "Mật khẩu mới đã được gửi tới email của bạn.";
                HttpContext.Session.Clear();

                return RedirectToAction("Login");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo tài khoản: {ex.Message}";
                return View();
            }
        }

        private string GenerateRandomPassword()
        {
            var length = 6;
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }


        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }

}
