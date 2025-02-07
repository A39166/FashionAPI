using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class PageListProductDTO : BaseDTO
    {
        public string ProductName { get; set; }
        public ShortCategoryDTO Category { get; set; }
        public string Code { get; set; }
        public ShortColorDTO Color { get; set; }
        [DefaultValue(0)]
        public int Selled {  get; set; }
        public double Price { get; set; }
        public string ImagesPath { get; set; }
        public sbyte Status { get; set; }
    }
}
