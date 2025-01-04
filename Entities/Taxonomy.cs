using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class Taxonomy
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Taxonomy> InverseParent { get; set; } = new List<Taxonomy>();

    public virtual Taxonomy? Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
