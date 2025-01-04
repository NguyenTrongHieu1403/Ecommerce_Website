using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public int Sold { get; set; }

    public int Quantity { get; set; }

    public string? Img { get; set; }

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

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Taxonomy Taxonomy { get; set; } = null!;
}
