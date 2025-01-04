using ecommerce_final.Entities;
using ecommerce_final.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_final.Extensions
{
    public class OrderService
    {
        private readonly EcommerceFinalContext _context;

        public OrderService(EcommerceFinalContext context)
        {
            _context = context;
        }

        public List<OrderVM> GetOrderDetailsByUser(int userId)  // Sửa userId kiểu int cho đúng với kiểu dữ liệu trong DB
        {
            // Lấy danh sách đơn hàng của người dùng
            var orders = _context.Invoices
                .Where(o => o.UserId == userId)  // Dùng kiểu dữ liệu int cho UserId
                .ToList();

            // Lấy tất cả chi tiết đơn hàng và thông tin sản phẩm trong một truy vấn duy nhất
            var orderDetails = _context.InvoiceItems
                .Where(cthd => orders.Select(o => o.Id).Contains(cthd.InvoiceId))  // Sửa lại tên trường MaHd -> Id cho Invoice
                .Join(_context.Products,  // Thay ChiTietHds và HangHoas bằng InvoiceItems và Products
                      cthd => cthd.ProductId,
                      hh => hh.Id,
                      (cthd, hh) => new OrderDetailVM
                      {
                          Id = cthd.Id,
                          InvoiceId = cthd.InvoiceId,
                          ProductId = cthd.ProductId,
                          TenHh = hh.Name,
                          Discount = cthd.Discount,
                          Quantity = cthd.Quantity,
                          Price = cthd.Price,
                          CreatedAt = cthd.CreatedAt,
                          HinhAnh = hh.Img
                      })
                .ToList();

            // Map lại dữ liệu vào OrderVM
            var orderVMs = orders.Select(order => new OrderVM
            {
                Id = order.Id,
                PaymentMethod = order.PaymentMethod,
                FullName = order.FullName,
                PaymentStatus = order.PaymentStatus,
                Note = order.Note,
                AddressUser = order.AddressUser,
                ShipmentType = order.ShipmentType,
                CreatedAt = order.CreatedAt,
                OderDetails = orderDetails.Where(od => od.InvoiceId == order.Id).ToList()  // Lọc các chi tiết đơn hàng tương ứng
            }).ToList();

            return orderVMs;
        }
    }
}
