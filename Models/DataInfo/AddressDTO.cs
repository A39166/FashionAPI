using FashionAPI.Databases.FashionDB;
using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class AddressDTO : BaseDTO
    {
        public string UserUuid { get; set; }
        public string Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public string Address { get; set; }
        public InfoCatalogDTO? TP { get; set; }

        public InfoCatalogDTO? QH { get; set; }

        public InfoCatalogDTO? XA { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
}
