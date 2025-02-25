namespace FashionAPI.Models.DataInfo
{
    public class ShortCategoryDTO : BaseDTO
    {
        public string Name { get; set; }
        public sbyte Status { get; set; }
    }
    public class ShortColorCategoryDTO : ShortCategoryDTO
    {
        public string Code { get; set; }
    }
}
