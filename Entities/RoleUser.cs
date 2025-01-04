using System;
using System.Collections.Generic;

namespace ecommerce_final.Entities;

public partial class RoleUser
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string RoleDetail { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
