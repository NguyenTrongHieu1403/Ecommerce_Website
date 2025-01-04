using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ecommerce_final.Models;
using ecommerce_final.Extensions;
using Microsoft.EntityFrameworkCore;
using ecommerce_final.Entities;
using System.Security.Claims;
using ecommerce_final.Extensions;
using ecommerce_final.Models;

namespace ecommerce_final.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly EcommerceFinalContext _context;

        public PaymentController(PaypalClient paypalClient, EcommerceFinalContext context)
        {
            _paypalClient = paypalClient;
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.PaypalClientId = _paypalClient.ClientId;

            return View();
        }

        const string CART_KEY = "CART";
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
        public IActionResult PaypalDemo()
        {
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View();
        }

      /*  [HttpPost]
        public async Task<IActionResult> PaypalOrder(CancellationToken cancellationToken)
        { // Lấy giỏ hàng từ Session
            var cartItems = HttpContext.Session.Get<List<CartItemVM>>("CART");

            // Kiểm tra xem giỏ hàng có tồn tại không
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Giỏ hàng rỗng hoặc không hợp lệ.");
            }
            // Kiểm tra giá trị từ giỏ hàng
            var totalAmount = cartItems.Sum(item => item.ThanhTien).ToString();
            var donViTienTe = "USD"; // Đơn vị tiền tệ là USD
            var orderIdref = "DH" + DateTime.Now.Ticks.ToString(); // Mã đơn hàng duy nhất

            // Lấy thông tin đơn hàng (có thể lấy từ Session hoặc bất kỳ nơi nào khác)
            var tongTien = "200000.0"; // Ví dụ số tiền thanh toán
            var donViTienTe = "USD"; // Mã tiền tệ
            var orderIdref = "DH" + DateTime.Now.Ticks.ToString(); // Tạo mã đơn hàng duy nhất
            var apiUrl = "https://api-m.paypal.com/v2/checkout/orders"; // PayPal API URL

            try
            {
                // Tạo đơn hàng thông qua client PayPal
                var response = await _paypalClient.CreateOrder(totalAmount, donViTienTe, orderIdref);

                // Trả về kết quả cho client (ID đơn hàng)
                return Ok(new { id = response.id });
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                // Trả về lỗi nếu có sự cố trong quá trình tạo đơn hàng
                return BadRequest(error);
            }
        }*/
        public async Task<IActionResult> PaypalOrder(CancellationToken cancellationToken)
        {
            // Tạo đơn hàng (thông tin lấy từ Session???)
            var tongTien = CartItems.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            // OrderId - mã tham chiếu (duy nhất)
            var orderIdref = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                // a. Create paypal order
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, orderIdref);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }



        public async Task<IActionResult> PaypalCapture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                //nhớ kiểm tra status complete
                if (response.status == "COMPLETED")
                {
                    var reference = response.purchase_units[0].reference_id;//mã đơn hàng mình tạo ở trên

                    // Put your logic to save the transaction here
                    // You can use the "reference" variable as a transaction key
                    // 1. Tạo và Lưu đơn hàng vô database
                    // TransactionId của Seller: response.payments.captures[0].id
                    var hoaDon = new Invoice
                    {
                        UserId = Convert.ToInt32(User.FindFirstValue("UserId")),
                        CreatedAt = DateTime.Now,
                        FullName = User.Identity.Name,
                        AddressUser = "N/A",//tự update
                        PaymentMethod = "Paypal",
                        PaymentStatus = "N/A",
                        ShipmentType = "Free",
                        Note = $"reference_id={reference}, transactionId={response.purchase_units[0].payments.captures[0].id}"
                    };
                    _context.Add(hoaDon);
                    _context.SaveChanges();
                    foreach (var item in CartItems)
                    {
                        var cthd = new InvoiceItem
                        {
                            InvoiceId = hoaDon.Id,
                            ProductId = item.MaHh,
                            Price = (decimal)item.DonGia,
                            Quantity = item.SoLuong,
                            Discount = 0,
                            CreatedAt = DateTime.Now
                        };
                        _context.Add(cthd);
                    }
                    _context.SaveChanges();
                    //2. Xóa session giỏ hàng
                    HttpContext.Session.Set(CART_KEY, new List<CartItemVM>());

                    return Ok(response);
                }
                else
                {
                    return BadRequest(new { Message = "Có lỗi thanh toán" });
                }
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }





        public IActionResult Success()
        {
            ClearCart();

            return View();
        }
        private void ClearCart()
        {
            // Xóa giỏ hàng khỏi Session hoặc database
            HttpContext.Session.Remove("CART");
        }


    }
}
