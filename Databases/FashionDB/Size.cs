using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Size
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string SizeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<ProductVariant> ProductVariant { get; set; } = new List<ProductVariant>();
}
