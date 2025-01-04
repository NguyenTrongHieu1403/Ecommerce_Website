using ecommerce_final.Entities;
using ecommerce_final.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_final.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceFinalContext _ctx;

        public ProductController(EcommerceFinalContext ctx)
        {
            _ctx = ctx;
        }


        public IActionResult Detail(int id)
        {
            var hangHoaEntity = _ctx.Products.Find(id);
            var hangHoaVM = new HangHoaVM
            {
                MaHh = hangHoaEntity.Id,
                TenHh = hangHoaEntity.Name,
                DonGia = (double)hangHoaEntity.Price,
                MoTa = hangHoaEntity.Description,
                Hinh = hangHoaEntity.Img,
                // Thêm các thuộc tính cần thiết từ `HangHoa` vào `HangHoaVM`
            };

            var combinedViewModel = new CombinedViewModel
            {
                HangHoaEntity = hangHoaEntity,
                HangHoaVM = hangHoaVM
            };

            // Pass the scheme to the view
            ViewData["Scheme"] = Request.Scheme;

            return View(combinedViewModel);
        }

        // Danh sách sản phẩm
        public IActionResult Index(int? cateid)
        {
            // Lấy danh sách sản phẩm từ database
            var data = _ctx.Products.AsQueryable().Where(p => p.IsDelete == false);

            // Nếu có lọc theo category ID
            if (cateid.HasValue)
            {
                data = data.Where(p => p.TaxonomyId == cateid.Value);
            }

            // Chuyển dữ liệu sang ViewModel
            var result = data.Select(p => new HangHoaVM
            {
                MaHh = p.Id,
                TenHh = p.Name,
                DonGia = (double)p.Price,
                Hinh = p.Img,
                MoTa = p.Description
            }).ToList();

            // Trả về view với dữ liệu
            return View(result);
        }
    }
}
