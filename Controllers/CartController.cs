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
        [HttpPost("add-to-cart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortCategoryDTO>), description: "AddToCart Response")]
        public async Task<IActionResult> AddToCart(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var color = _context.Color.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.ColorName + " ", $"%{request.Keyword}%"))
                                                 .Where(x => x.Status == 1)
                                                 .ToList();
                if (color != null)
                {
                    response.Data = color.Select(p => new ShortCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.ColorName,
                        Status = p.Status
                    }).ToList();
                }
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
