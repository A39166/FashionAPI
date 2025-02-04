namespace FashionAPI.Models.DataInfo
{
    public class CategoryDTO : BaseDTO
    {
        public string CategoryName { get; set; }
        public string Path { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
