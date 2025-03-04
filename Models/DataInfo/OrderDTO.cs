using FashionAPI.Models.DataInfo;
using FashionAPI.Models.Response;
using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public class OrderDTO : BaseDTO
{
    public string Code { get; set; } = null!;


    public double TotalPrice { get; set; }

    /// <summary>
    /// 0-Xác nhận đơn hàng, 1-Đang giao hàng,2-Giao thành công
    /// </summary>
    public sbyte State { get; set; }
    public DateTime TimeCreated { get; set; }
    public sbyte Status { get; set; }
}
