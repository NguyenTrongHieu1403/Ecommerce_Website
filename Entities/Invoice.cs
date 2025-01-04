using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? FullName { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Note { get; set; }

    public string? AddressUser { get; set; }

    public string? ShipmentType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual User User { get; set; } = null!;
}
