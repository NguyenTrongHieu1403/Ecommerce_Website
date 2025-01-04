using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class ProductAttribute
{
    public int ProductId { get; set; }

    public int AttributeValueId { get; set; }

    public int Id { get; set; }

    public virtual AttributeValue AttributeValue { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
