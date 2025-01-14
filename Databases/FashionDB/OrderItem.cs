using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class OrderItem
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string OrderUuid { get; set; } = null!;

    public string ProductVariantUuid { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public sbyte Status { get; set; }

    public virtual Order OrderUu { get; set; } = null!;

    public virtual ProductVariant ProductVariantUu { get; set; } = null!;
}
