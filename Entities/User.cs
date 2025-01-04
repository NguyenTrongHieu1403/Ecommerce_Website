using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? HashSalt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Sex { get; set; }

    public DateOnly? BirthOfDate { get; set; }

    public string? Avatar { get; set; }

    public bool? Active { get; set; }

    public int? RoleId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Telephone { get; set; }

    public bool? EmailVerify { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual RoleUser? Role { get; set; }
}
