namespace FashionAPI.Models.DataInfo
{
    public class CategoryDTO : BaseDTO
    {
        public string? ParentUuid { get; set; }
        public string CategoryName { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
