using FashionAPI.Models.DataInfo;
using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public class OrderItemDTO : BaseDTO
{
    public string ProductVariantUuid { get; set; } = null!;

    public int Quantity { get; set; }

    public double Price { get; set; }

    public sbyte Status { get; set; }

}
public class OrderItemForPageListOrderAdmin : BaseDTO
{
    public ShortProductDTO Product { get; set; }
    public ShortCategoryDTO SizeCategory { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public sbyte Status { get; set; }
}
