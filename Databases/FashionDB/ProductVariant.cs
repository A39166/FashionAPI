using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class ProductVariant
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string ProductUuid { get; set; } = null!;

    public string SizeUuid { get; set; } = null!;

    public string ColorUuid { get; set; } = null!;

    public int Stock { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();

    public virtual Color ColorUu { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();

    public virtual Product ProductUu { get; set; } = null!;

    public virtual Size SizeUu { get; set; } = null!;
}
