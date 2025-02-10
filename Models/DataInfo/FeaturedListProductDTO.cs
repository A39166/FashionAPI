using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class FeaturedListProductDTO : BaseDTO
    {
        public string ProductName { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public string ImagesPath { get; set; }
        public sbyte Status { get; set; }
    }
}
