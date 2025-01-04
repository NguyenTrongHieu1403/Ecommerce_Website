using ecommerce_final.Entities;

namespace ecommerce_final.Models
{
    public class OrderVM
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? FullName { get; set; }

        public string? PaymentStatus { get; set; }

        public string? Note { get; set; }

        public string? AddressUser { get; set; }

        public string? ShipmentType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<OrderDetailVM> OderDetails { get; set; } 

        public virtual User User { get; set; } = null!;
    }

    public class OrderDetailVM
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public string TenHh { get; set; } // Tên Hàng Hóa

        public decimal? Discount { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public string HinhAnh { get; set; } // Hình Ảnh Sản Phẩm

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
