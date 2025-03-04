using System;
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
    /// 0-Chờ xác nhận, 1-Đang giao hàng,2-Giao thành công,3-Hủy đơn hàng
    /// </summary>
    public sbyte State { get; set; }

    public DateTime TimeCreated { get; set; }

    public sbyte Status { get; set; }

    public string? Note { get; set; }

    public string? TimeUpdate { get; set; }

    public virtual UserAddress AddressUu { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();
}
