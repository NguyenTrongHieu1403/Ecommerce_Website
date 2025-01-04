namespace ecommerce_final.Models
{
    public class CartItemVM
    {
        public int MaHh { get; set; }
        public string TenHh { get; set; }
        public double DonGia { get; set; }
        public string? Hinh { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien => SoLuong * DonGia;
    }
}
