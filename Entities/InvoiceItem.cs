using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class InvoiceItem
{
    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    public string? TenHh { get; set; }

    public decimal? Discount { get; set; }

    public int Quantity { get; set; }

    public string? HinhAnh { get; set; }

    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
