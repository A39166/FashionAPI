using FashionAPI.Models.Request;
using FashionAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FashionAPI.Enums;
using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using FashionAPI.Extensions;
using FashionAPI.Configuaration;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Cart Controller")]
    public class CartController : BaseController
    {
        private readonly ILogger<CartController> _logger;
        private readonly DBContext _context;

        public CartController(DBContext context, ILogger<CartController> logger)
        {

            _context = context;
            _logger = logger;
        }
        

        
    }
    
}
