using FashionAPI.Databases.FashionDB;
using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class ListCartDTO : BaseDTO
    {
        public List<ProductForCartDTO> Items { get; set; }
        public sbyte Status { get; set; }

    }

    public class ProductForCartDTO : BaseDTO
    {
        public ShortColorCategoryDTO Color { get; set; }
        public ShortProductDTO Product { get; set; }
        public ShortCategoryDTO Size { get; set; }
        public int Quantity { get; set; }
    }
}
