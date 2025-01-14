namespace FashionAPI.Models.DataInfo
{
    public class ColorDTO : BaseDTO
    {
        public string ColorName { get; set; }
        public string Description { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
