using ecommerce_final.Entities;

namespace ecommerce_final.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Summary { get; set; } = null!;

        public int Sold { get; set; }

        public int Quantity { get; set; }

        public string Img { get; set; } = null!;

        public bool? IsSoldOut { get; set; }

        public bool? IsNew { get; set; }

        public string? Sku { get; set; }

        public decimal Price { get; set; }

        public int TaxonomyId { get; set; }

        public int? DiscountId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDelete { get; set; }

        public virtual Discount? Discount { get; set; }
        public List<int> SelectedAttributes { get; set; } // Danh sách AttributeId đã chọn
    }

}
