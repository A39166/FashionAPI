using FashionAPI.Models.Request;

namespace FashionAPI.Models.DataInfo
{
    public class ProductDetailClientDTO : BaseDTO
    {
        public ShortCategoryDTO Category { get; set; }
        public ShortColorDTO Color { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public List<ShortCategoryDTO> Variants { get; set; }
        public List<string>? ImagesPath { get; set; }
        public sbyte Status { get; set; }
    }
}
