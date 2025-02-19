﻿using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class Order
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string AddressUuid { get; set; } = null!;

    public double TotalPrice { get; set; }

    /// <summary>
    /// 0-Xác nhận đơn hàng, 1-Đang giao hàng,2-Giao thành công
    /// </summary>
    public sbyte State { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public virtual UserAddress AddressUu { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
}
