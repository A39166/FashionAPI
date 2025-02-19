﻿using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Category
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
