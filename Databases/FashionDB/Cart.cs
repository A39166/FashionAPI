using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Cart
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string UserUuid { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual ICollection<CartItem> CartItem { get; set; } = new List<CartItem>();

    public virtual User UserUu { get; set; } = null!;
}
