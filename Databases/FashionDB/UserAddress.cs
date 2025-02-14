using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public partial class UserAddress
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string UserUuid { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Matp { get; set; } = null!;

    public string Maqh { get; set; } = null!;

    public string Xaid { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0 - không sử dụng, 1 - hoạt động
    /// </summary>
    public sbyte Status { get; set; }

    public bool IsDefault { get; set; }

    public virtual District MaqhNavigation { get; set; } = null!;

    public virtual Province MatpNavigation { get; set; } = null!;

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual User UserUu { get; set; } = null!;

    public virtual Ward Xa { get; set; } = null!;
}
