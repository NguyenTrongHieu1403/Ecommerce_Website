using AspNetCoreHero.ToastNotification.Abstractions;
using ecommerce_final.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceFinal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaxonomyController : Controller
    {
        private readonly EcommerceFinalContext _context;
        private readonly INotyfService _notifyService;

        public TaxonomyController(EcommerceFinalContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Taxonomy
        public async Task<IActionResult> Index()
        {
            var taxonomies = await _context.Taxonomies
            .Where(t => t.IsDelete == false)
                .ToListAsync();
            return View(taxonomies);
        }


        // GET: Admin/Taxonomy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            // Truy vấn Taxonomy kèm Parent
            var taxonomy = await _context.Taxonomies
                .Include(t => t.Parent) // Load thông tin danh mục cha
                .FirstOrDefaultAsync(t => t.Id == id && t.IsDelete == false);

            if (taxonomy == null)
            {
                _notifyService.Error("Không tìm thấy mã giảm giá.");
                return NotFound();
            }

            return View(taxonomy);
        }


        // GET: Admin/Taxonomy/Create
        public IActionResult Create()
        {
            var activeTaxonomies = _context.Taxonomies.Where(t => t.IsDelete == false).ToList();
            ViewData["ParentId"] = new SelectList(activeTaxonomies, "Id", "Name");
            return View();
        }

        // POST: Admin/Taxonomy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ParentId")] Taxonomy taxonomy)
        {
            var existingTaxonomy = _context.Taxonomies
                .FirstOrDefault(t => t.Name == taxonomy.Name);

            if (existingTaxonomy != null)
            {
                if (existingTaxonomy.ParentId != taxonomy.ParentId)
                {
                    // Thông báo lỗi nếu chọn sai ParentId
                    ModelState.AddModelError("ParentId", $"Danh mục '{taxonomy.Name}' đã tồn tại trong danh mục cha khác.");
                    _notifyService.Error($"Danh mục '{taxonomy.Name}' đã tồn tại trong danh mục cha khác. Vui lòng chọn danh mục cha hợp lệ.");

                    // Cập nhật danh sách danh mục cha để hiển thị lại cho admin
                    ViewData["ParentId"] = new SelectList(
                        _context.Taxonomies.Where(t => t.IsDelete == false),
                        "Id", "Name", taxonomy.ParentId
                    );

                    // Trả về view với thông báo lỗi
                    return View(taxonomy);
                }

                if (existingTaxonomy.IsDelete == true)
                {
                    // Nếu danh mục bị xóa, kích hoạt lại
                    existingTaxonomy.IsDelete = false;
                    existingTaxonomy.UpdatedAt = DateTime.Now;

                    _context.Update(existingTaxonomy);
                    await _context.SaveChangesAsync();

                    _notifyService.Success($"Danh mục '{taxonomy.Name}' đã được kích hoạt lại.");
                    return RedirectToAction(nameof(Index));
                }

                // Nếu danh mục đã tồn tại và đang hoạt động, báo lỗi
                ModelState.AddModelError("Name", $"Danh mục '{taxonomy.Name}' đã tồn tại và đang hoạt động.");
                _notifyService.Error($"Danh mục '{taxonomy.Name}' đã tồn tại.");
                ViewData["ParentId"] = new SelectList(
                    _context.Taxonomies.Where(t => t.IsDelete == false),
                    "Id", "Name", taxonomy.ParentId
                );
                return View(taxonomy);
            }

            // Thêm mới danh mục nếu không tồn tại
            if (ModelState.IsValid)
            {
                taxonomy.CreatedAt = DateTime.Now;
                _context.Add(taxonomy);
                await _context.SaveChangesAsync();

                _notifyService.Success("Tạo mới danh mục thành công.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Tạo mới danh mục thất bại. Vui lòng kiểm tra lại thông tin.");
            ViewData["ParentId"] = new SelectList(
                _context.Taxonomies.Where(t => t.IsDelete == false),
                "Id", "Name", taxonomy.ParentId
            );
            return View(taxonomy);
        }
        // GET: Admin/Taxonomy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taxonomy = await _context.Taxonomies.FindAsync(id);
            if (taxonomy == null || (taxonomy.IsDelete.HasValue && taxonomy.IsDelete.Value))
            {
                return NotFound();
            }

            // Lấy tất cả các danh mục (không xóa)
            var allTaxonomies = await _context.Taxonomies
                .Where(t => t.IsDelete == null || t.IsDelete == false)
                .ToListAsync();

            // Danh sách chứa các danh mục cha
            var selectListItems = new List<SelectListItem>();

            // Duyệt qua các danh mục và phân loại theo cấp độ
            foreach (var t in allTaxonomies)
            {
                // Không hiển thị chính nó trong ParentId
                if (t.Id != taxonomy.Id)
                {
                    if (t.ParentId == null) // Cấp 1
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = t.Id.ToString(),
                            Text = $"Level 1 - {t.Name}"
                        });
                    }
                    else
                    {
                        var parent = allTaxonomies.FirstOrDefault(p => p.Id == t.ParentId);
                        if (parent != null)
                        {
                            if (parent.ParentId == null) // Cấp 2 (với ParentId là cấp 1)
                            {
                                selectListItems.Add(new SelectListItem
                                {
                                    Value = t.Id.ToString(),
                                    Text = $"Level 2 - {t.Name} (Parent: {parent.Name})"
                                });
                            }
                            else if (parent.ParentId != null) // Cấp 3 (với ParentId là cấp 2)
                            {
                                var grandParent = allTaxonomies.FirstOrDefault(p => p.Id == parent.ParentId);
                                if (grandParent != null)
                                {
                                    selectListItems.Add(new SelectListItem
                                    {
                                        Value = t.Id.ToString(),
                                        Text = $"Level 3 - {t.Name} (Parent: {parent.Name}, Grandparent: {grandParent.Name})"
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // Truyền danh sách vào ViewBag để hiển thị trong dropdown
            ViewBag.ParentId = new SelectList(selectListItems, "Value", "Text", taxonomy.ParentId);

            return View(taxonomy);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ParentId")] Taxonomy taxonomy)
        {
            if (id != taxonomy.Id)
            {
                return NotFound();
            }

            // Lấy danh mục hiện tại từ cơ sở dữ liệu
            var existingTaxonomy = await _context.Taxonomies.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existingTaxonomy == null)
            {
                return NotFound();
            }

            // Giữ nguyên giá trị IsDelete nếu không có thay đổi từ phía form
            taxonomy.IsDelete = existingTaxonomy.IsDelete ?? false;

            // Kiểm tra nếu ParentId là chính nó
            if (taxonomy.ParentId == taxonomy.Id)
            {
                ModelState.AddModelError("ParentId", "Danh mục không thể chọn chính nó làm parent.");
                return View(taxonomy);
            }

            // Kiểm tra sự thay đổi ParentId và đảm bảo không có vòng lặp hoặc sai lệch cấp độ
            if (taxonomy.ParentId != null)
            {
                var parentTaxonomy = await _context.Taxonomies.FirstOrDefaultAsync(t => t.Id == taxonomy.ParentId);
                if (parentTaxonomy != null)
                {
                    // Kiểm tra xem ParentId có hợp lệ không, không cho phép cấp 3 là parent của cấp 2
                    if (parentTaxonomy.ParentId != null) // Parent là cấp 2 hoặc cấp 3
                    {
                        // Kiểm tra nếu parent là cấp 2, không thể có ParentId là cấp 3 trở lên
                        if (parentTaxonomy.ParentId != null && parentTaxonomy.ParentId != existingTaxonomy.ParentId)
                        {
                            ModelState.AddModelError("ParentId", "Không thể chọn danh mục này làm Parent cho cấp 3 hoặc 4.");
                            return View(taxonomy);
                        }
                    }
                }
            }

            // Cập nhật thời gian và lưu lại
            if (ModelState.IsValid)
            {
                try
                {
                    taxonomy.UpdatedAt = DateTime.Now;

                    // Detach the conflicting entity if exists
                    _context.Entry(existingTaxonomy).State = EntityState.Detached;

                    // Cập nhật danh mục
                    _context.Update(taxonomy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxonomyExists(taxonomy.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại.");
                        return View(taxonomy);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Trả lại danh sách các danh mục cha phù hợp cho dropdown
            var allTaxonomies = await _context.Taxonomies
                .Where(t => t.IsDelete == null || t.IsDelete == false)
                .ToListAsync();

            ViewData["ParentId"] = new SelectList(allTaxonomies, "Id", "Name", taxonomy.ParentId);

            return View(taxonomy);
        }


        // GET: Admin/Taxonomy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Truy vấn Taxonomy kèm Parent
            var taxonomy = await _context.Taxonomies
                .Include(t => t.Parent) // Load thông tin danh mục cha
                .FirstOrDefaultAsync(t => t.Id == id && t.IsDelete == false);
            if (taxonomy == null)
            {
                return NotFound();
            }

            return View(taxonomy);
        }

        // POST: Admin/Taxonomy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taxonomy = await _context.Taxonomies.FindAsync(id);
            taxonomy.IsDelete = true;  // Soft delete
            _context.Update(taxonomy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaxonomyExists(int id)
        {
            return _context.Taxonomies.Any(e => e.Id == id);
        }
    }
}
