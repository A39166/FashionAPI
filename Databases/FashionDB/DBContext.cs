using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FashionAPI.Databases.FashionDB;

public partial class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Cart { get; set; }

    public virtual DbSet<CartItem> CartItem { get; set; }

    public virtual DbSet<Category> Category { get; set; }

    public virtual DbSet<Color> Color { get; set; }

    public virtual DbSet<District> District { get; set; }

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderItem> OrderItem { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<ProductImage> ProductImage { get; set; }

    public virtual DbSet<ProductVariant> ProductVariant { get; set; }

    public virtual DbSet<Province> Province { get; set; }

    public virtual DbSet<Sessions> Sessions { get; set; }

    public virtual DbSet<Size> Size { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<UserAddress> UserAddress { get; set; }

    public virtual DbSet<Ward> Ward { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cart")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.UserUuid, "fk_user_uuid_cart");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.UserUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("user_uuid");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.UserUu).WithMany(p => p.Cart)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.UserUuid)
                .HasConstraintName("fk_user_uuid_cart");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("cart_item")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.CartUuid, "kf_cart_uuid_pc");

            entity.HasIndex(e => e.ProductVariantUuid, "kf_product_variant_uuid_pc");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CartUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("cart_uuid");
            entity.Property(e => e.ProductVariantUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("product_variant_uuid");
            entity.Property(e => e.Quantity)
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.CartUu).WithMany(p => p.CartItem)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.CartUuid)
                .HasConstraintName("kf_cart_uuid_pc");

            entity.HasOne(d => d.ProductVariantUu).WithMany(p => p.CartItem)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ProductVariantUuid)
                .HasConstraintName("kf_product_variant_uuid_pc");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("category")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("color")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasColumnType("text")
                .HasColumnName("code");
            entity.Property(e => e.ColorName)
                .HasMaxLength(10)
                .HasColumnName("color_name");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Maqh).HasName("PRIMARY");

            entity
                .ToTable("district")
                .HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Maqh)
                .HasMaxLength(20)
                .HasColumnName("maqh")
                .UseCollation("utf8mb4_unicode_520_ci")
                .HasCharSet("utf8mb4");
            entity.Property(e => e.Matp)
                .HasMaxLength(5)
                .HasColumnName("matp")
                .UseCollation("utf8mb4_unicode_520_ci")
                .HasCharSet("utf8mb4");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("order")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.AddressUuid, "fk_address_uuid_order");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AddressUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("address_uuid");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.State)
                .HasComment("0-Chờ xác nhận, 1-Đang giao hàng,2-Giao thành công,3-Hủy đơn hàng")
                .HasColumnType("tinyint(4)")
                .HasColumnName("state");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.TimeUpdate)
                .HasColumnType("timestamp")
                .HasColumnName("time_update");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("double(11,2)")
                .HasColumnName("total_price");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.AddressUu).WithMany(p => p.Order)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.AddressUuid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_address_uuid_order");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("order_item")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.OrderUuid, "fk_order_uuid_oi");

            entity.HasIndex(e => e.ProductVariantUuid, "fk_pv_uuid_oi");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.OrderUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("order_uuid");
            entity.Property(e => e.Price)
                .HasColumnType("double(11,2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductVariantUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("product_variant_uuid");
            entity.Property(e => e.Quantity)
                .HasColumnType("int(11)")
                .HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.OrderUu).WithMany(p => p.OrderItem)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.OrderUuid)
                .HasConstraintName("fk_order_uuid_oi");

            entity.HasOne(d => d.ProductVariantUu).WithMany(p => p.OrderItem)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ProductVariantUuid)
                .HasConstraintName("fk_pv_uuid_oi");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("product")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.CatUuid, "fk_cat_uuid_product");

            entity.HasIndex(e => e.ColorUuid, "fk_color_uuid_product");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CatUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("cat_uuid");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.ColorUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("color_uuid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("double(10,2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .HasColumnName("product_name");
            entity.Property(e => e.ShortDescription).HasColumnName("short_description");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.CatUu).WithMany(p => p.Product)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.CatUuid)
                .HasConstraintName("fk_cat_uuid_product");

            entity.HasOne(d => d.ColorUu).WithMany(p => p.Product)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ColorUuid)
                .HasConstraintName("fk_color_uuid_product");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("product_image")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.ProductUuid, "fk_product_uuid_pi");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("product_uuid");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.ProductUu).WithMany(p => p.ProductImage)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ProductUuid)
                .HasConstraintName("fk_product_uuid_pi");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("product_variant")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.ProductUuid, "fk_product_uuid_pv");

            entity.HasIndex(e => e.SizeUuid, "fk_size_uuid_pv");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("product_uuid");
            entity.Property(e => e.SizeUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("size_uuid");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName(" status");
            entity.Property(e => e.Stock)
                .HasColumnType("int(11)")
                .HasColumnName("stock");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.ProductUu).WithMany(p => p.ProductVariant)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.ProductUuid)
                .HasConstraintName("fk_product_uuid_pv");

            entity.HasOne(d => d.SizeUu).WithMany(p => p.ProductVariant)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.SizeUuid)
                .HasConstraintName("fk_size_uuid_pv");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Matp).HasName("PRIMARY");

            entity
                .ToTable("province")
                .HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Matp)
                .HasMaxLength(5)
                .HasColumnName("matp")
                .UseCollation("utf8mb4_unicode_520_ci")
                .HasCharSet("utf8mb4");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Slug)
                .HasMaxLength(70)
                .HasColumnName("slug");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        modelBuilder.Entity<Sessions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("sessions")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.UserUuid, "fk_user_uuid_ss");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20)")
                .HasColumnName("id");
            entity.Property(e => e.Status)
                .HasComment("0: LogIn - 1: LogOut")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeLogin)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_login");
            entity.Property(e => e.TimeLogout)
                .HasColumnType("timestamp")
                .HasColumnName("time_logout");
            entity.Property(e => e.UserUuid)
                .HasMaxLength(50)
                .HasColumnName("user_uuid");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");

            entity.HasOne(d => d.UserUu).WithMany(p => p.Sessions)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.UserUuid)
                .HasConstraintName("fk_user_uuid_ss");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("size")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.SizeName)
                .HasMaxLength(5)
                .HasColumnName("size_name");
            entity.Property(e => e.Status)
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("user")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasComment("0-Nam , 1-Nữ , 2 - khác")
                .HasColumnType("tinyint(4)")
                .HasColumnName("gender");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'1'")
                .HasComment("0-Admin, 1-Client")
                .HasColumnType("tinyint(4)")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'1'")
                .HasComment("0 - đang khóa, 1 - hoạt động")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("user_address")
                .UseCollation("utf8mb4_unicode_520_ci");

            entity.HasIndex(e => e.Maqh, "fk_maqh_address");

            entity.HasIndex(e => e.Matp, "fk_matp_address");

            entity.HasIndex(e => e.UserUuid, "fk_user_uuid_ua");

            entity.HasIndex(e => e.Xaid, "fk_xaid_address");

            entity.HasIndex(e => e.Uuid, "unq_uuid").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.IsDefault).HasColumnName("isDefault");
            entity.Property(e => e.Maqh)
                .HasMaxLength(5)
                .HasColumnName("maqh");
            entity.Property(e => e.Matp)
                .HasMaxLength(5)
                .HasColumnName("matp");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.Status)
                .HasComment("0 - không sử dụng, 1 - hoạt động")
                .HasColumnType("tinyint(4)")
                .HasColumnName("status");
            entity.Property(e => e.TimeCreated)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("time_created");
            entity.Property(e => e.UserUuid)
                .HasMaxLength(36)
                .IsFixedLength()
                .HasColumnName("user_uuid");
            entity.Property(e => e.Uuid)
                .HasMaxLength(36)
                .HasDefaultValueSql("uuid()")
                .IsFixedLength()
                .HasColumnName("uuid");
            entity.Property(e => e.Xaid)
                .HasMaxLength(5)
                .HasColumnName("xaid");

            entity.HasOne(d => d.MaqhNavigation).WithMany(p => p.UserAddress)
                .HasForeignKey(d => d.Maqh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_maqh_address");

            entity.HasOne(d => d.MatpNavigation).WithMany(p => p.UserAddress)
                .HasForeignKey(d => d.Matp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_matp_address");

            entity.HasOne(d => d.UserUu).WithMany(p => p.UserAddress)
                .HasPrincipalKey(p => p.Uuid)
                .HasForeignKey(d => d.UserUuid)
                .HasConstraintName("fk_user_uuid_ua");

            entity.HasOne(d => d.Xa).WithMany(p => p.UserAddress)
                .HasForeignKey(d => d.Xaid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_xaid_address");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Xaid).HasName("PRIMARY");

            entity
                .ToTable("ward")
                .HasCharSet("latin1")
                .UseCollation("latin1_swedish_ci");

            entity.Property(e => e.Xaid)
                .HasMaxLength(20)
                .HasColumnName("xaid")
                .UseCollation("utf8mb4_unicode_520_ci")
                .HasCharSet("utf8mb4");
            entity.Property(e => e.Maqh)
                .HasMaxLength(5)
                .HasColumnName("maqh")
                .UseCollation("utf8mb4_unicode_520_ci")
                .HasCharSet("utf8mb4");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Type)
                .HasMaxLength(70)
                .HasColumnName("type")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
