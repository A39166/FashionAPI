﻿using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class CartItem
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string CartUuid { get; set; } = null!;

    public string ProductVariantUuid { get; set; } = null!;

    public int Quantity { get; set; }

    public sbyte Status { get; set; }

    public virtual Cart CartUu { get; set; } = null!;

    public virtual ProductVariant ProductVariantUu { get; set; } = null!;
}
