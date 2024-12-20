using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Province
{
    public string Matp { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Slug { get; set; }

    public virtual ICollection<UserAddress> UserAddress { get; set; } = new List<UserAddress>();
}
