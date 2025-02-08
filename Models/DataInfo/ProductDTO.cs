using FashionAPI.Models.Request;

namespace FashionAPI.Models.DataInfo
{
    public class ProductDTO : BaseDTO
    {
        public string CatUuid { get; set; }
        public string ColorUuid { get; set; }
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
}
