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
    [SwaggerTag("Order Controller")]
    public class OrderController : BaseController
    {
        private readonly ILogger<OrderController> _logger;
        private readonly DBContext _context;

        public OrderController(DBContext context, ILogger<OrderController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("create-order")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "CreateOrder Response")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var order = new Order()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    AddressUuid = request.AddressUuid,
                    TotalPrice = request.TotalPrice,
                    Note = request.Note,
                    State = 0,
                    TimeCreated = DateTime.Now,
                    Status = 1
                };
                order.Code = "ORD" + order.Id;
                _context.Order.Add(order);
                foreach(var item in request.Product)
                {
                    var orderitem = new OrderItem()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        OrderUuid = order.Uuid,
                        ProductVariantUuid = _context.ProductVariant.Where(x => x.ProductUuid == item.ProductUuid && x.SizeUuid == item.SizeUuid)
                                                                    .Select(v => v.Uuid).FirstOrDefault(),
                        Quantity = item.Quantity,
                        Price = item.Price,
                    };
                    _context.OrderItem.Add(orderitem);
                    var variant = _context.ProductVariant.Where(x => x.ProductUuid == item.ProductUuid && x.SizeUuid == item.SizeUuid)
                                                         .FirstOrDefault();
                    variant.Stock = variant.Stock - item.Quantity;
                    _context.ProductVariant.Update(variant);
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
        

    }
    
}
