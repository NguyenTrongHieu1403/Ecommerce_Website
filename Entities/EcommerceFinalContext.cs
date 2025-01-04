using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_final.Entities;

public partial class EcommerceFinalContext : DbContext
{
    public EcommerceFinalContext()
    {
    }

    public EcommerceFinalContext(DbContextOptions<EcommerceFinalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AttributeValue> AttributeValues { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<RoleUser> RoleUsers { get; set; }

    public virtual DbSet<Taxonomy> Taxonomies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=tcp:ecommercefinal.database.windows.net,1433;Initial Catalog=ecommerce_final;Persist Security Info=True;User ID=admin123;Password=password123@;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3213E83FE98F9E05");

            entity.ToTable("AttributeValue");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Value)
                .HasMaxLength(100)
                .HasColumnName("value");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discount__3213E83FA2E06F1A");

            entity.ToTable("Discount");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoice__3213E83FDFF9485E");

            entity.ToTable("Invoice", tb => tb.HasTrigger("trg_UpdateInvoice"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressUser)
                .HasMaxLength(255)
                .HasColumnName("address_user");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("payment_status");
            entity.Property(e => e.ShipmentType)
                .HasMaxLength(255)
                .HasColumnName("shipment_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Invoice__user_id__5224328E");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoice___3213E83F04C183A4");

            entity.ToTable("Invoice_Item", tb => tb.HasTrigger("trg_UpdateInvoice_Item"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TenHh).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__Invoice_I__invoi__58D1301D");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Invoice_I__produ__59C55456");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OTP__3213E83F57DC09A3");

            entity.ToTable("OTP");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttempsLeft)
                .HasDefaultValue(3)
                .HasColumnName("attemps_left");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ExpirationTime)
                .HasDefaultValueSql("(dateadd(minute,(5),getdate()))")
                .HasColumnType("datetime")
                .HasColumnName("expiration_time");
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false)
                .HasColumnName("is_used");
            entity.Property(e => e.IsValid)
                .HasDefaultValue(true)
                .HasColumnName("is_valid");
            entity.Property(e => e.OtpCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("otp_code");
            entity.Property(e => e.Purpose)
                .HasMaxLength(20)
                .HasColumnName("purpose");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Otps)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OTP__user_id__5070F446");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3213E83FDE580E83");

            entity.HasIndex(e => e.Sku, "UQ_Products_Sku")
                .IsUnique()
                .HasFilter("([Sku] IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.IsNew)
                .HasDefaultValue(false)
                .HasColumnName("is_new");
            entity.Property(e => e.IsSoldOut)
                .HasDefaultValue(false)
                .HasColumnName("is_sold_out");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Sku)
                .HasMaxLength(100)
                .HasColumnName("sku");
            entity.Property(e => e.Sold).HasColumnName("sold");
            entity.Property(e => e.Summary)
                .HasMaxLength(500)
                .HasColumnName("summary");
            entity.Property(e => e.TaxonomyId).HasColumnName("taxonomy_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Discount).WithMany(p => p.Products)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Products__discou__1332DBDC");

            entity.HasOne(d => d.Taxonomy).WithMany(p => p.Products)
                .HasForeignKey(d => d.TaxonomyId)
                .HasConstraintName("FK__Products__taxono__151B244E");
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductAttributes");

            entity.ToTable("ProductAttribute");

            entity.Property(e => e.AttributeValueId).HasColumnName("attribute_value_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.AttributeValue).WithMany(p => p.ProductAttributes)
                .HasForeignKey(d => d.AttributeValueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductAttributes_AttributeValue");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductAttributes_Product");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3213E83F7A226F48");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Reviews__product__160F4887");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__user_id__17036CC0");
        });

        modelBuilder.Entity<RoleUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role_Use__3213E83F571DCCE2");

            entity.ToTable("Role_Users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.RoleDetail).HasColumnName("role_detail");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Taxonomy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Taxonomy__3213E83F528E2420");

            entity.ToTable("Taxonomy");

            entity.HasIndex(e => e.Name, "UQ__Taxonomy__72E12F1BD3D57FCD").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Taxonomy__parent__17F790F9");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F37D07451");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616485B018BB").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572BC802F4F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.BirthOfDate).HasColumnName("birth_of_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerify)
                .HasDefaultValue(false)
                .HasColumnName("email_verify");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.HashSalt)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("hash_salt");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("modified_at");
            entity.Property(e => e.PasswordHash)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(2)
                .HasColumnName("role_id");
            entity.Property(e => e.Sex)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("Other")
                .HasColumnName("sex");
            entity.Property(e => e.Telephone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telephone");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__18EBB532");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
