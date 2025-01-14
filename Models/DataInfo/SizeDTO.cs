namespace FashionAPI.Models.DataInfo
{
    public class SizeDTO : BaseDTO
    {
        public string SizeName { get; set; }
        public string Description { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }
    }
}
