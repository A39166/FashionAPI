using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Product
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string CatUuid { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public double Price { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual Category CatUu { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariant { get; set; } = new List<ProductVariant>();
}
