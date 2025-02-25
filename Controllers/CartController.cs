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
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "AddToCart Response")]
        public async Task<IActionResult> AddToCart(List<AddToCartRequest> requests)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var existcart = _context.Cart.Where(x => x.UserUuid == validToken.UserUuid).FirstOrDefault();
                if (existcart == null)
                {
                    var cart = new Cart()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        UserUuid = validToken.UserUuid,
                        TimeCreated = DateTime.Now,
                        Status = 1
                    };
                    foreach(var request in requests)
                    {
                        var item = new CartItem()
                        {
                            Uuid = Guid.NewGuid().ToString(),
                            CartUuid = cart.Uuid,
                            ProductVariantUuid = _context.ProductVariant.Where(x => x.ProductUuid == request.ProductUuid && x.SizeUuid == request.SizeUuid)
                                                                        .Select(v => v.Uuid).FirstOrDefault(),
                            Quantity = request.quantity,
                            Status = 1,
                        };
                        _context.CartItem.Add(item);
                    }
                }
                else
                {
                    var cartitem = _context.CartItem.Include(p => p.ProductVariantUu).ToList();
                    foreach (var request in requests)
                    {
                        var existitem = cartitem.Where(x => x.ProductVariantUu.ProductUuid == request.ProductUuid
                                                 && x.ProductVariantUu.SizeUuid == request.SizeUuid
                                                 && x.CartUuid == existcart.Uuid).FirstOrDefault();
                        if (existitem != null)
                        {
                            existitem.Quantity = request.quantity;
                            _context.CartItem.Update(existitem);
                        }
                        else
                        {
                            var item = new CartItem()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                CartUuid = existcart.Uuid,
                                ProductVariantUuid = _context.ProductVariant.Where(x => x.ProductUuid == request.ProductUuid && x.SizeUuid == request.SizeUuid)
                                                                        .Select(v => v.Uuid).FirstOrDefault(),
                                Quantity = request.quantity,
                                Status = 1,
                            };
                            _context.CartItem.Add(item);
                        }
                    }
                }
                _context.SaveChanges();
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
        [HttpPost("get-list-cart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListAddressDTO>), description: "GetListCart Response")]
        public async Task<IActionResult> GetListCart()
        {
            var response = new BaseResponseMessageItem<PageListAddressDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstAddress = _context.UserAddress.Where(x => x.UserUuid == validToken.UserUuid && x.Status == 1).ToList();
                if (lstAddress != null)
                {
                    response.Data = lstAddress.Select(p => new PageListAddressDTO
                    {
                        Uuid = p.Uuid,
                        Fullname = p.Fullname,
                        PhoneNumber = p.PhoneNumber,
                        Address = p.Address,
                        IsDefault = p.IsDefault,
                        Status = p.Status,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
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
