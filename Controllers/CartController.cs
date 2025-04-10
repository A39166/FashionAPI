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
        public async Task<IActionResult> AddToCart(AddToCartRequest requests)
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
                    _context.Cart.Add(cart);
                    var item = new CartItem()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        CartUuid = cart.Uuid,
                        ProductVariantUuid = _context.ProductVariant.Where(x => x.ProductUuid == requests.ProductUuid && x.SizeUuid == requests.SizeUuid)
                                                                    .Select(v => v.Uuid).FirstOrDefault(),
                        Quantity = requests.quantity,
                        Status = 1,
                    };
                    _context.CartItem.Add(item);
                }
                else
                {
                    var variant = _context.CartItem.Include(x => x.ProductVariantUu)
                                                    .Where(x => x.CartUuid == existcart.Uuid)
                                                    .Where(x => x.ProductVariantUu.ProductUuid == requests.ProductUuid && x.ProductVariantUu.SizeUuid == requests.SizeUuid)
                                                    .FirstOrDefault();
                    if (variant != null)
                    {
                        variant.Quantity = requests.quantity + variant.Quantity;
                    }
                    else
                    {
                        var item = new CartItem()
                        {
                            Uuid = Guid.NewGuid().ToString(),
                            CartUuid = existcart.Uuid,
                            ProductVariantUuid = _context.ProductVariant.Where(x => x.ProductUuid == requests.ProductUuid && x.SizeUuid == requests.SizeUuid)
                                                                    .Select(v => v.Uuid).FirstOrDefault(),
                            Quantity = requests.quantity,
                            Status = 1,
                        };
                        _context.CartItem.Add(item);
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
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ProductForCartDTO>), description: "GetListCart Response")]
        public async Task<IActionResult> GetListCart()
        {
            var response = new BaseResponseMessageItem<ProductForCartDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var cart = _context.CartItem.Include(c => c.CartUu)
                                            .Include(p => p.ProductVariantUu).ThenInclude(p => p.ProductUu).ThenInclude(i => i.ProductImage)
                                            .Include(p => p.ProductVariantUu).ThenInclude(p => p.ProductUu).ThenInclude(c => c.ColorUu)
                                            .Include(p => p.ProductVariantUu).ThenInclude(p => p.SizeUu)
                                            .Where(x => x.CartUu.UserUuid == validToken.UserUuid).ToList();
                response.Data = cart.Select(c => new ProductForCartDTO
                {
                    Uuid = c.Uuid,
                    Quantity = c.Quantity,
                    Product = new ShortProductDTO
                    {
                        Uuid = c.ProductVariantUu.ProductUu.Uuid,
                        ProductName = c.ProductVariantUu.ProductUu.ProductName,
                        Code = c.ProductVariantUu.ProductUu.Code,
                        Price = c.ProductVariantUu.ProductUu.Price,
                        ImagesPath = c.ProductVariantUu.ProductUu.ProductImage.Where(x => x.IsDefault == true && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                    },
                    Color = new ShortColorCategoryDTO
                    {
                        Uuid = c.ProductVariantUu.ProductUu.ColorUu.Uuid,
                        Name = c.ProductVariantUu.ProductUu.ColorUu.ColorName,
                        Code = c.ProductVariantUu.ProductUu.ColorUu.Code,
                        Status = c.ProductVariantUu.ProductUu.ColorUu.Status,
                    },
                    Size = new ShortCategoryDTO
                    {
                        Uuid = c.ProductVariantUu.SizeUu.Uuid,
                        Name = c.ProductVariantUu.SizeUu.SizeName,
                        Status = c.ProductVariantUu.SizeUu.Status,
                    },


                }).ToList();

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

        [HttpPost("get-quantity-cart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<CartQuantityResponse>), description: "GetQuantityCart Response")]
        public async Task<IActionResult> GetQuantityCart()
        {
            var response = new BaseResponseMessage<CartQuantityResponse>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                int quantity = _context.CartItem
                    .Include(c => c.CartUu)
                    .Where(x => x.CartUu.UserUuid == validToken.UserUuid)
                    .Count();
                response.Data = new CartQuantityResponse { Quantity = quantity };
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

        [HttpPost("remove-item-from-cart")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "RemoveItemFromCart Response")]
        public async Task<IActionResult> RemoveItemFromCart(RemoveFromCartRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {

                var items = _context.CartItem.Where(x => request.Uuid.Contains(x.Uuid)).ToList();

                if (!items.Any())
                {
                    throw new ErrorException(ErrorCode.PRODUCT_NOTFOUND);
                }

                _context.CartItem.RemoveRange(items);
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



    }

}
