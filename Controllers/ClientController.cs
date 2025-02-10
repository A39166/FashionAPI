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
using Microsoft.EntityFrameworkCore;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Client Controller")]
    public class ClientController : BaseController
    {
        private readonly ILogger<ClientController> _logger;
        private readonly DBContext _context;

        public ClientController(DBContext context, ILogger<ClientController> logger)
        {

            _context = context;
            _logger = logger;
        }

        [HttpPost("get-featured-list-product")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<FeaturedListProductDTO>), description: "GetFeaturedListProduct Response")]
        public async Task<IActionResult> GetFeaturedListProduct()
        {
            var response = new BaseResponseMessageItem<FeaturedListProductDTO>();
            try
            {
                var featured = _context.Product.Where(x => x.Status == 1).Take(8)
                                                 .ToList();
                response.Data = featured.Select(p => new FeaturedListProductDTO
                {
                    Uuid = p.Uuid,
                    ProductName = p.ProductName,
                    Code = p.Code,
                    Price = p.Price,
                    ImagesPath = _context.ProductImage.Where(x => x.ProductUuid == p.Uuid && x.IsDefault == true).Select(x => x.Path).FirstOrDefault(),
                    Status = p.Status,
                }).ToList();

                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }


    }
    
}
