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

    public int Matp { get; set; }

    public int Maqh { get; set; }

    public int Xaid { get; set; }

    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// 0 - không sử dụng, 1 - hoạt động
    /// </summary>
    public sbyte Status { get; set; }

    public virtual User UserUu { get; set; } = null!;
}
