namespace FashionAPI.Models.DataInfo
{
    public class ClientCategoryDTO : BaseDTO
    {
        public string CategoryName { get; set; }
        public string Path { get; set; }
        public sbyte Status { get; set; }
    }
}
