namespace ecommerce_final.Models
{
    public class ProductDTO
    {
        public int Id { get; set; } // Thêm Id vào DTO để dễ dàng truy cập và chỉnh sửa
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int TaxonomyId { get; set; }
        public int? DiscountId { get; set; }
        public string? Img { get; set; }
        public string? Sku { get; set; }
        public List<int> AttributeValueIds { get; set; } = new List<int>(); // Danh sách ID thuộc tính
    }

}
