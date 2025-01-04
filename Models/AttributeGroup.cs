using ecommerce_final.Entities;

namespace ecommerce_final.Models
{
    public class AttributeGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; // Ví dụ: "Size", "Color"
        public virtual ICollection<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();
    }

}
