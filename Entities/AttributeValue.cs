using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class AttributeValue
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
}
