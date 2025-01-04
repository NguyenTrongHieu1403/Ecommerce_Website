using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ecommerce_final.Tool;
using Microsoft.EntityFrameworkCore;
using ecommerce_final.Entities;
using ecommerce_final.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecommerce_final.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly EcommerceFinalContext _context;
        private readonly INotyfService _notifyService;

        public ProductController(EcommerceFinalContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        public async Task<IActionResult> Filter(int idtaxonomy, int idattribute, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;

            var products = _context.Products.AsQueryable().Where(p => p.IsDelete == false);

            if (idtaxonomy > 0)
            {
                // Lấy tất cả thể loại con (bao gồm cả thể loại gốc) của idtaxonomy đã chọn
                var childTaxonomies = await _context.Taxonomies
                                                     .Where(t => t.ParentId == idtaxonomy || t.Id == idtaxonomy)
                                                     .Select(t => t.Id)
                                                     .ToListAsync();

                // Lọc sản phẩm theo TaxonomyId hoặc các thể loại con
                products = products.Where(p => childTaxonomies.Contains(p.TaxonomyId));
            }

            if (idattribute > 0)
            {
                // Cập nhật lại phần này nếu AttributeValue không còn liên quan đến ProductAttributes
                products = products.Where(p => p.ProductAttributes.Any(pa => pa.AttributeValueId == idattribute));
            }
            var pagedProducts = products.Include(p => p.Taxonomy)
                 .Include(p => p.ProductAttributes)
                     .ThenInclude(pa => pa.AttributeValue) // Include the AttributeValue associated with ProductAttributes
                 .OrderBy(p => p.Name)
                 .ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return PartialView("_ProductsTablePartialView", pagedProducts);
        }


        // GET: Admin/Product
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var products = _context.Products.AsNoTracking()
              .Where(p => p.IsDelete == false) // Lọc chỉ các sản phẩm chưa bị xóa
              .Include(x => x.Taxonomy)
              .Include(x => x.Discount)
              .Include(x => x.ProductAttributes)  // Include the ProductAttributes table
                  .ThenInclude(pa => pa.AttributeValue)  // Then include the AttributeValue
              .OrderByDescending(x => x.Id);

            PagedList<Product> models = new PagedList<Product>(products, pageNumber, pageSize);


            // Lấy danh sách Thể loại (Taxonomies) và Thuộc tính (AttributeValues)
            var danhMuc = _context.Taxonomies.ToList();
            var thuocTinh = _context.AttributeValues.ToList();

            // Kiểm tra danh sách có bị null hoặc trống không
            if (!danhMuc.Any() || !thuocTinh.Any())
            {
                // Ghi log hoặc gợi ý người dùng thêm dữ liệu
                ViewBag.ErrorMessage = "Không có dữ liệu Thể loại hoặc Thuộc tính. Vui lòng kiểm tra!";
            }

            ViewData["DanhMuc"] = new SelectList(danhMuc, "Id", "Name");
            ViewData["ThuocTinh"] = new SelectList(thuocTinh, "Id", "Value");

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var product = await _context.Products
           .Include(p => p.Taxonomy)
           .Include(p => p.Discount)
           .Include(p => p.ProductAttributes)
           .ThenInclude( p => p.AttributeValue)
           .FirstOrDefaultAsync(p => p.Id == id && p.IsDelete == false);

            if (product == null)
            {
                _notifyService.Error("Không tìm thấy sản phẩm.");
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Taxonomies, "Id", "Name");
            ViewData["Discount"] = new SelectList(_context.Discounts, "Id", "Name");
            ViewData["ThuocTinh"] = new SelectList(_context.AttributeValues, "Id", "Value");

            return View();
        }


        /*   [HttpPost]
           [ValidateAntiForgeryToken]
           public async Task<IActionResult> Create([Bind("Name,Description,Summary,Price,Quantity,TaxonomyId,DiscountId,Img,Sku,IsNew")] Product product,
                                            IFormFile ImageFile,
                                            List<int> AttributeValues)
           {
               // Kiểm tra ModelState ngay từ đầu
               if (!ModelState.IsValid)
               {
                   _notifyService.Error("Thông tin không hợp lệ. Vui lòng kiểm tra lại.");
                   return View(product);
               }

               // Kiểm tra trùng lặp tên sản phẩm
               if (_context.Products.Any(p => p.Name == product.Name))
               {
                   ModelState.AddModelError("Name", "Tên sản phẩm đã tồn tại.");
                   return View(product);
               }

               // Xử lý ảnh
               if (ImageFile != null && ImageFile.Length > 0)
               {
                   string fileName = Tool.UploadFileToFolder.Upload(ImageFile, "Products");
                   if (!string.IsNullOrEmpty(fileName))
                   {
                       product.Img = "/Images/Products/" + fileName;
                       ModelState.Remove("Img");
                   }
                   else
                   {
                       ModelState.AddModelError("Img", "Lỗi khi tải ảnh lên.");
                   }
               }
               else
               {
                   ModelState.AddModelError("Img", "Bạn phải chọn ảnh sản phẩm.");
               }

               // Nếu ModelState vẫn hợp lệ
               if (ModelState.IsValid)
               {
                   using var transaction = _context.Database.BeginTransaction();
                   try
                   {
                       // Lưu sản phẩm
                       product.CreatedAt = DateTime.Now;
                       product.Sold = 0;
                       _context.Products.Add(product);
                       await _context.SaveChangesAsync();

                       // Thêm thuộc tính
                       if (AttributeValues == null || AttributeValues.Count == 0)
                       {
                           _notifyService.Warning("Không có thuộc tính nào được gắn cho sản phẩm. Bạn có thể thêm thuộc tính sau.");
                       }
                       else
                       {
                           foreach (var attrId in AttributeValues)
                           {
                               var productAttribute = new ProductAttribute
                               {
                                   ProductId = product.Id,
                                   AttributeValueId = attrId
                               };
                               _context.ProductAttributes.Add(productAttribute);
                           }
                           await _context.SaveChangesAsync();
                       }

                       // Xác nhận transaction
                       transaction.Commit();
                       _notifyService.Success("Tạo mới sản phẩm thành công.");
                       return RedirectToAction(nameof(Index));
                   }
                   catch (Exception ex)
                   {
                       // Rollback nếu có lỗi
                       transaction.Rollback();
                       _notifyService.Error($"Lỗi khi lưu dữ liệu: {ex.Message}");
                       return View(product);
                   }

               }

               _notifyService.Error("Tạo mới sản phẩm thất bại. Vui lòng kiểm tra lại thông tin.");
               return View(product);
           }*/

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO productDto, IFormFile ImageFile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Bắt đầu giao dịch
            try
            {

                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return BadRequest(new { message = "Tên sản phẩm không được để trống." });

                if (productDto.Price <= 0)
                    return BadRequest(new { message = "Giá sản phẩm phải lớn hơn 0." });


                // Kiểm tra và lưu file ảnh
                string imagePath = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                    Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    imagePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Chỉ lưu đường dẫn cần thiết (VD: /images/products/...)
                    imagePath = Path.Combine("/images/products", fileName).Replace("\\", "/");
                }

                // Thêm sản phẩm
                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Summary = productDto.Summary,
                    Quantity = productDto.Quantity,
                    Price = productDto.Price,
                    TaxonomyId = productDto.TaxonomyId,
                    DiscountId = productDto.DiscountId,
                    Img = imagePath,
                    Sku = productDto.Sku,
                    Sold = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Thêm ProductAttributes liên quan
                if (productDto.AttributeValueIds?.Count > 0)
                {
                    var productAttributes = productDto.AttributeValueIds.Select(attrValueId => new ProductAttribute
                    {
                        ProductId = product.Id, // ID của sản phẩm vừa được thêm
                        AttributeValueId = attrValueId
                    }).ToList();

                    _context.ProductAttributes.AddRange(productAttributes);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync(); // Xác nhận giao dịch
                return Ok(new { message = "Sản phẩm và thuộc tính đã được thêm thành công!", productId = product.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Hủy giao dịch nếu có lỗi
                return BadRequest(new
                {
                    message = "Lỗi khi thêm sản phẩm.",
                    error = ex.InnerException?.Message ?? ex.Message // Lấy thông tin chi tiết lỗi
                });
            }

        }



        // GET: Admin/Product/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu
            var product = await _context.Products
                .Include(p => p.Taxonomy) // Đưa Taxonomy vào nếu cần
                .Include(p => p.Discount)  // Đưa Discount vào nếu cần
                .Include(p => p.ProductAttributes) // Nếu có thuộc tính sản phẩm
                .ThenInclude(pa => pa.AttributeValue) // Đưa giá trị thuộc tính vào
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Chuyển thông tin sản phẩm vào ViewModel (ProductDTO) để dễ chỉnh sửa
            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Summary = product.Summary,
                Price = product.Price,
                Quantity = product.Quantity,
                TaxonomyId = product.TaxonomyId,
                DiscountId = product.DiscountId,
                Sku = product.Sku,
                AttributeValueIds = product.ProductAttributes?.Select(pa => pa.AttributeValueId).ToList(),
                // Nếu có ảnh, lấy đường dẫn ảnh hiện tại
                Img = product.Img
            };

            // Lấy danh sách các lựa chọn cho Taxonomy, Discount, AttributeValues để điền vào select list
            ViewData["DanhMuc"] = new SelectList(_context.Taxonomies, "Id", "Name", product.TaxonomyId);
            ViewData["Discount"] = new SelectList(_context.Discounts, "Id", "Name", product.DiscountId);
            ViewData["ThuocTinh"] = new SelectList(_context.AttributeValues, "Id", "Value");

            return View(productDto);
        }


        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO productDto, IFormFile? ImageFile)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy sản phẩm cần chỉnh sửa từ database
                    var product = await _context.Products
                        .Include(p => p.ProductAttributes)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin cơ bản của sản phẩm
                    product.Name = productDto.Name;
                    product.Description = productDto.Description;
                    product.Summary = productDto.Summary;
                    product.Price = productDto.Price;
                    product.Quantity = productDto.Quantity;
                    product.TaxonomyId = productDto.TaxonomyId;
                    product.DiscountId = productDto.DiscountId;

                    // Kiểm tra và lưu file ảnh
                    string imagePath = null;
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        imagePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Chỉ lưu đường dẫn cần thiết (VD: /images/products/...)
                        imagePath = Path.Combine("/images/products", fileName).Replace("\\", "/");
                    }


                    // Xóa các thuộc tính hiện tại trong bảng ProductAttributes
                    var existingAttributes = product.ProductAttributes.ToList();
                    _context.ProductAttributes.RemoveRange(existingAttributes);

                    // Thêm các thuộc tính mới
                    if (productDto.AttributeValueIds != null && productDto.AttributeValueIds.Any())
                    {
                        foreach (var attributeValueId in productDto.AttributeValueIds)
                        {
                            var productAttribute = new ProductAttribute
                            {
                                ProductId = product.Id,
                                AttributeValueId = attributeValueId
                            };
                            product.ProductAttributes.Add(productAttribute);
                        }
                    }

                    // Cập nhật sản phẩm
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }

            // Truyền dữ liệu cần thiết về View để render lại form
            ViewBag.DanhMuc = new SelectList(_context.Taxonomies, "Id", "Name", productDto.TaxonomyId);
            ViewBag.Discount = new SelectList(_context.Discounts, "Id", "Name", productDto.DiscountId);
            ViewBag.ThuocTinh = _context.AttributeValues
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
                .ToList();

            return View(productDto);
        }


        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _notifyService.Warning("ID không hợp lệ.");
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDelete == false);

            if (product == null)
            {
                _notifyService.Error("Không tìm thấy sản phẩm.");
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDelete = true;
                product.UpdatedAt = DateTime.Now;

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    _notifyService.Success("Xóa sản phẩm thành công.");
                }
                catch (Exception)
                {
                    _notifyService.Error("Có lỗi khi xóa sản phẩm.");
                }
            }
            else
            {
                _notifyService.Warning("Không tìm thấy sản phẩm.");
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
