using ecommerce_final.Entities;
using ecommerce_final.Models;

namespace ecommerce_final.Models
{
    public class DashboardViewModel
    {
        public string HoTen { get; set; }
        public List<Invoice> HoaDonDtos { get; set; } // Danh sách đơn hàng

        public List<OrderVM> OrderDetails { get; set; } // Danh sách chi tiết hóa đơn
    }
}
