using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerce_final.Entities;
using ecommerce_final.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ecommerce_final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AttributeController : Controller
    {
        private readonly EcommerceFinalContext _context;
        private readonly INotyfService _notifyService;

        public AttributeController(EcommerceFinalContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Attribute
        public async Task<IActionResult> Index()
        {
            var attributes = await _context.AttributeValues
                .Where(a => a.IsDelete == false)
                .ToListAsync();
            return View(attributes);
        }

        // GET: Admin/Attribute/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var attribute = await _context.AttributeValues
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDelete == false);

            if (attribute == null)
            {
                _notifyService.Error("Không tìm thấy thuộc tính.");
                return NotFound();
            }

            return View(attribute);
        }


        // GET: Admin/Attribute/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Admin/Attribute/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Value,Description")] AttributeValue attributeValue)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu tên thuộc tính và giá trị đã tồn tại và bị xóa
                var existingAttribute = await _context.AttributeValues
                    .FirstOrDefaultAsync(a => a.Name == attributeValue.Name && a.Value == attributeValue.Value && a.IsDelete == true);

                if (existingAttribute != null)
                {
                    // Nếu tồn tại thuộc tính bị xóa, khôi phục lại thuộc tính đó
                    existingAttribute.IsDelete = false;
                    existingAttribute.Description = attributeValue.Description; // Cập nhật mô tả nếu cần
                    existingAttribute.UpdatedAt = DateTime.Now;

                    try
                    {
                        // Lưu thay đổi
                        _context.Update(existingAttribute);
                        await _context.SaveChangesAsync();
                        _notifyService.Success("Thuộc tính đã được khôi phục.");
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _notifyService.Error("Có lỗi khi khôi phục thuộc tính.");
                        return View(attributeValue);
                    }
                }

                // Nếu không tồn tại thuộc tính bị xóa, tạo mới thuộc tính
                attributeValue.CreatedAt = DateTime.Now;
                attributeValue.IsDelete = false; // Đảm bảo IsDelete là false khi tạo mới
                _context.Add(attributeValue);
                await _context.SaveChangesAsync();

                _notifyService.Success("Tạo mới thuộc tính thành công.");
                return RedirectToAction(nameof(Index));
            }
            return View(attributeValue);
        }



        // GET: Admin/Attribute/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var attributeValue = await _context.AttributeValues
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDelete == false);

            if (attributeValue == null)
            {
                _notifyService.Error("Không tìm thấy thuộc tính.");
                return NotFound();
            }

            return View(attributeValue);
        }

        // POST: Admin/Attribute/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Value,Description,UpdatedAt")] AttributeValue attributeValue)
        {
            if (id != attributeValue.Id)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            // Lấy thuộc tính từ cơ sở dữ liệu để đảm bảo các trường khác được giữ nguyên
            var existingAttribute = await _context.AttributeValues
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAttribute == null)
            {
                _notifyService.Error("Không tìm thấy thuộc tính.");
                return NotFound();
            }

            // Chỉ cập nhật các trường cần thiết, không thay đổi IsDelete
            existingAttribute.Name = attributeValue.Name;
            existingAttribute.Value = attributeValue.Value;
            existingAttribute.Description = attributeValue.Description;
            existingAttribute.UpdatedAt = DateTime.Now;

            // Lưu các thay đổi vào cơ sở dữ liệu
            try
            {
                _context.Update(existingAttribute);
                await _context.SaveChangesAsync();
                _notifyService.Success("Cập nhật thuộc tính thành công.");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttributeValueExists(attributeValue.Id))
                {
                    _notifyService.Error("Không tìm thấy thuộc tính.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }




        // GET: Admin/Attribute/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var attribute = await _context.AttributeValues
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDelete == false);

            if (attribute == null)
            {
                _notifyService.Error("Không tìm thấy thuộc tính.");
                return NotFound();
            }

            return View(attribute);
        }


        // POST: Admin/Attribute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attribute = await _context.AttributeValues
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDelete == false); // Lọc theo IsDelete = false để chỉ xóa các thuộc tính chưa bị xóa

            if (attribute == null)
            {
                _notifyService.Error("Không tìm thấy thuộc tính.");
                return NotFound();
            }

            // Thực hiện "soft delete" bằng cách đánh dấu IsDelete = true
            attribute.IsDelete = true;
            attribute.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi

            _context.Update(attribute); // Cập nhật thông tin vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            _notifyService.Success("Xóa thuộc tính thành công.");
            return RedirectToAction(nameof(Index)); // Trở lại danh sách thuộc tính
        }


        private bool AttributeValueExists(int id)
        {
            return _context.AttributeValues.Any(e => e.Id == id);
        }

    }
}
