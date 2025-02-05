using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class ProductImage
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string ProductUuid { get; set; } = null!;

    public string Path { get; set; } = null!;

    public sbyte Status { get; set; }

    public bool? IsDefault { get; set; }

    public virtual Product ProductUu { get; set; } = null!;
}
