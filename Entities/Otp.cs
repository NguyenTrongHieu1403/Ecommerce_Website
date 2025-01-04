using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class Otp
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public int? UserId { get; set; }

    public string OtpCode { get; set; } = null!;

    public bool? IsValid { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public string? Purpose { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? AttempsLeft { get; set; }

    public virtual User? User { get; set; }
}
