using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ecommerce_final.Models;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.CodeAnalysis.Scripting;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.IdentityModel.Tokens;
using ecommerce_final.Entities;

namespace ecommerce_final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly EcommerceFinalContext _context;
        public INotyfService _notifyService { get; }

        public AccountController(EcommerceFinalContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index(string roleFilter, string statusFilter)
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.RoleUsers, "Id", "RoleName");
            ViewData["lsTrangThai"] = new List<SelectListItem>
            {
                new SelectListItem { Text = "Active", Value = "1" },
                new SelectListItem { Text = "Block", Value = "0" }
            };

            var usersQuery = _context.Users.Include(u => u.Role).AsQueryable().Where( p => p.IsDelete == false);

            if (!string.IsNullOrEmpty(roleFilter))
            {
                usersQuery = usersQuery.Where(u => u.RoleId.ToString() == roleFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isActive = statusFilter == "1";
                usersQuery = usersQuery.Where(u => u.Active == isActive);
            }

            var users = await usersQuery.ToListAsync();
            return View(users);
        }


        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)  // Nếu cần thông tin về Role, bao gồm thông tin Role của người dùng
                .FirstOrDefaultAsync(u => u.Id == id && u.IsDelete == false);

            if (user == null)
            {
                _notifyService.Error("Không tìm thấy người dùng.");
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.RoleUsers, "Id", "RoleName");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,PasswordHash,Email,Telephone,RoleId,Active")] User account)
        {
            // Kiểm tra trùng lặp Username và Email
            if (_context.Users.Any(a => a.Username == account.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                _notifyService.Error("Tên đăng nhập đã tồn tại.");
            }
            if (_context.Users.Any(a => a.Email == account.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                _notifyService.Error("Email đã tồn tại.");
            }

            // Kiểm tra định dạng Username
            if (!System.Text.RegularExpressions.Regex.IsMatch(account.Username, @"^[a-zA-Z0-9]+$"))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập chỉ được chứa ký tự không dấu và số.");
                _notifyService.Error("Tên đăng nhập chỉ được chứa ký tự không dấu và số.");
            }

            // Kiểm tra và mã hóa mật khẩu
            if (string.IsNullOrWhiteSpace(account.PasswordHash) || account.PasswordHash.Length < 5 || account.PasswordHash.Length > 15)
            {
                ModelState.AddModelError("PasswordHash", "Mật khẩu phải có độ dài từ 5 đến 15 ký tự.");
                _notifyService.Error("Mật khẩu phải có độ dài từ 5 đến 15 ký tự.");
            }
            else
            {
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(account.PasswordHash);
            }

            if (ModelState.IsValid)
            {
                account.CreatedAt = DateTime.Now;
                account.Active = true;

                _context.Add(account);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo tài khoản thành công.");
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoleId"] = new SelectList(_context.RoleUsers, "Id", "RoleName", account.RoleId);
            return View(account);
        }


        // GET: User/Edit/{id}
        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Chuyển thông tin của User thành UserUpdateDTO
            var userUpdateDTO = new UserUpdateDTO
            {
                Id = user.Id, // Chuyển đổi nullable int thành int với giá trị mặc định là 0
                Telephone = user.Telephone,
                Sex = user.Sex,
                BirthOfDate = user.BirthOfDate,
                Avatar = user.Avatar,
                RoleId = user.RoleId ?? 1, // Giá trị mặc định là 1 (Admin/User)
                Active = user.Active ?? false, // Giá trị mặc định là false
                IsDelete = user.IsDelete ?? false, // Giá trị mặc định là false
                FirstName = user.FirstName,
                LastName = user.LastName,
            };


            return View(userUpdateDTO);
        }


        // POST: Admin/AdminAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Telephone,Sex,BirthOfDate,Avatar,RoleId,Active,EmailVerify,IsDelete,FirstName,LastName")] User updatedUser, IFormFile? avatarFile)
        {
            if (id != updatedUser.Id)
            {
                _notifyService.Warning("ID không khớp.");
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsDelete == false);
            if (user == null)
            {
                _notifyService.Error("Không tìm thấy người dùng.");
                return NotFound();
            }

            try
            {
                // Cập nhật các trường cho phép chỉnh sửa
                user.Telephone = updatedUser.Telephone;
                user.Sex = updatedUser.Sex;
                user.BirthOfDate = updatedUser.BirthOfDate;
                user.RoleId = updatedUser.RoleId;
                user.Active = updatedUser.Active;
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;

                // Xử lý hình ảnh
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Định nghĩa thư mục lưu trữ
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
                    Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                    // Tạo tên tệp duy nhất
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Lưu file vào server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn trong database (chỉ lưu phần đường dẫn tương đối)
                    user.Avatar = Path.Combine("/images/avatars", fileName).Replace("\\", "/");
                }

                // Lưu thay đổi
                _context.Update(user);
                await _context.SaveChangesAsync();

                _notifyService.Success("Cập nhật thành công!");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    _notifyService.Error("Người dùng không tồn tại.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id && e.IsDelete == false);
        }


        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tìm tài khoản theo Id và bao gồm thông tin Role
            var account = await _context.Users
                .Include(u => u.Role)  // Bao gồm thông tin Role
                .FirstOrDefaultAsync(m => m.Id == id);  // Tìm tài khoản theo Id

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }


        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm tài khoản theo Id
            var account = await _context.Users.FindAsync(id);
            if (account != null)
            {
                account.IsDelete = true;
                _context.Users.Update(account);  // Xóa tài khoản
                await _context.SaveChangesAsync();  // Lưu thay đổi
                _notifyService.Success("Xóa thành công");
            }
            else
            {
                _notifyService.Error("Không tìm thấy tài khoản cần xóa");
            }

            return RedirectToAction(nameof(Index));  // Chuyển hướng về danh sách tài khoản
        }


        private bool AccountExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);  // Kiểm tra xem tài khoản có tồn tại không
        }

        private bool IsDuplicate(string username, string email, int? accountId = null)
        {
            return _context.Users.Any(a =>
                (a.Username == username || a.Email == email) &&
                (accountId == null || a.Id != accountId));
        }

    }
}
