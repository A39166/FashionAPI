using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Color
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string ColorName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<ProductVariant> ProductVariant { get; set; } = new List<ProductVariant>();
}
