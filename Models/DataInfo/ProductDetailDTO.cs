using FashionAPI.Models.Request;

namespace FashionAPI.Models.DataInfo
{
    public class ProductDetailDTO : BaseDTO
    {
        public ShortCategoryDTO Category { get; set; }
        public ShortColorDTO Color { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public List<Variant> Variants { get; set; }
        public List<string>? ImagesPath { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
    public class Variant : BaseDTO
    {
        public string SizeUuid { get; set; }
        public string SizeName { get; set; }
        public int Stock { get; set; }
        public sbyte Status { get; set; }
    }
}
