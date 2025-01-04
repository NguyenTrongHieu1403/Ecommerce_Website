using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_final.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ecommerce_final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountController : Controller
    {
        private readonly EcommerceFinalContext _context;
        private readonly INotyfService _notifyService;

        public DiscountController(EcommerceFinalContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Discount
        public async Task<IActionResult> Index()
        {
            var discounts = await _context.Discounts
                .Where(d => d.IsDelete == false)
                .ToListAsync();
            return View(discounts);
        }

        // GET: Admin/Discount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDelete == false);
            if (discount == null)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            return View(discount);
        }

        // GET: Admin/Discount/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Discount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,DiscountPercent,Active")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp tên mã giảm giá
                if (_context.Discounts.Any(d => d.Name == discount.Name && d.IsDelete == false))
                {
                    ModelState.AddModelError("Name", "Tên mã giảm giá đã tồn tại.");
                    _notifyService.Warning("Tên mã giảm giá đã tồn tại.");
                    return View(discount);
                }

                discount.CreatedAt = DateTime.Now;
                discount.IsDelete = false;
                _context.Add(discount);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới mã giảm giá thành công.");
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // GET: Admin/Discount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null || discount.IsDelete == true)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            return View(discount);
        }

        // POST: Admin/Discount/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,DiscountPercent,Active")] Discount discount)
        {
            if (id != discount.Id)
            {
                _notifyService.Warning("ID không khớp.");
                return NotFound();
            }

            // Kiểm tra sự tồn tại của mã giảm giá
            var existingDiscount = await _context.Discounts
       .Where(d => d.Id == id && (d.IsDelete == null || d.IsDelete == false))
       .FirstOrDefaultAsync();


            if (existingDiscount == null)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            // Kiểm tra trùng lặp tên mã giảm giá (nếu cần)
            if (_context.Discounts.Any(d => d.Name == discount.Name && d.Id != id && (d.IsDelete == null || !d.IsDelete.Value)))
            {
                ModelState.AddModelError("Name", "Tên mã giảm giá đã tồn tại.");
                return View(discount);
            }

            // Cập nhật các trường
            existingDiscount.Name = discount.Name;
            existingDiscount.Description = discount.Description;
            existingDiscount.DiscountPercent = discount.DiscountPercent;
            existingDiscount.Active = discount.Active;
            existingDiscount.UpdatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingDiscount);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật mã giảm giá thành công.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.Id))
                    {
                        _notifyService.Error("Không tìm thấy mã giảm giá.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(discount);
        }


        // GET: Admin/Discount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDelete == false);
            if (discount == null)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            return View(discount);
        }

        // POST: Admin/Discount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDelete == false);  // Chỉ lấy các mã giảm giá chưa bị xóa (IsDelete = false)

            if (discount == null)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            discount.IsDelete = true; // Soft delete
            discount.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa mã giảm giá thành công.");
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(d => d.Id == id && d.IsDelete == false);
        }
    }
}
