using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class Discount
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal DiscountPercent { get; set; }

    public bool Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
