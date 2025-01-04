using ecommerce_final.Entities;
using ecommerce_final.Extensions;
using ecommerce_final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_final.Controllers
{
    public class CartController : Controller
    {
        const string CART_KEY = "CART";
        private readonly EcommerceFinalContext _ctx;
        public CartController(EcommerceFinalContext ctx)
        {
            _ctx = ctx;
        }

        // Thuộc tính CartItems để lấy danh sách sản phẩm trong giỏ hàng từ Session
        public List<CartItemVM> CartItems
        {
            get
            {
                // Lấy dữ liệu từ Session và chuyển đổi sang List<CartItemVM>
                var carts = HttpContext.Session.Get<List<CartItemVM>>(CART_KEY);

                // Nếu Session rỗng, tạo danh sách mới để tránh lỗi null
                if (carts == null)
                {
                    carts = new List<CartItemVM>();
                }
                return carts;
            }
        }


        public IActionResult Index()
        {
            return View(CartItems);
        }

        [Authorize]
        public IActionResult AddToCart(int id, int qty = 1)
        {
            var gioHang = CartItems; // Sửa 'Carts' thành 'CartItems'

            // Kiểm tra id (MaHH) truyền qua đã nằm trong giỏ hàng hay chưa
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null) // Đã có
            {
                item.SoLuong += qty;
            }
            else
            {
                var hangHoa = _ctx.Products.SingleOrDefault(p => p.Id == id);
                if (hangHoa == null) // id không có trong Database
                {
                    return RedirectToAction("Index", "Products");
                }
                item = new CartItemVM
                {
                    MaHh = id,
                    SoLuong = qty,
                    TenHh = hangHoa.Name,
                    Hinh = hangHoa.Img,
                    DonGia = (double)hangHoa.Price
                };
                // Thêm vào giỏ hàng
                gioHang.Add(item);
            }

            // Cập nhật session
            HttpContext.Session.Set(CART_KEY, gioHang);
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = CartItems;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set("CART", gioHang);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, int qty)
        {
            var gioHang = CartItems;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);

            if (item != null && qty > 0)
            {
                item.SoLuong = qty;
            }
            else if (item != null && qty == 0)
            {
                gioHang.Remove(item);
            }

            HttpContext.Session.Set("CART", gioHang);

            var total = item != null ? item.SoLuong * item.DonGia : 0;

            return Json(new { total = total });
        }

        public IActionResult Checkout()
        {
            var cartItems = CartItems.ToList();
            // Tính tổng tiền từ danh sách cartItems
            double totalPrice = cartItems.Sum(item => item.DonGia * item.SoLuong);
            ViewBag.TotalPrice = totalPrice; // Truyền tổng tiền sang View
            return View(cartItems);
        }



    }
}
