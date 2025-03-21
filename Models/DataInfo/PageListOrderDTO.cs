﻿using FashionAPI.Models.DataInfo;
using FashionAPI.Models.Response;
using System;
using System.Collections.Generic;

namespace FashionAPI.Databases.FashionDB;

public class PageListOrderDTO : BaseDTO
{
    public string Code { get; set; } = null!;
    public ShortCategoryDTO User { get; set; }
    public ShortUserAddressDTO UserAddress { get; set; }
    public int TotalCount { get; set; }
    public List<OrderItemForPageListOrderAdmin> Items { get; set; }
    public double TotalPrice { get; set; }
    public sbyte State { get; set; }
    public string Note {  get; set; }
    public DateTime TimeCreated { get; set; }
    public DateTime? TimeUpdate { get; set; }
    public sbyte Status { get; set; }
}
