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
using System.Net;

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
                        Status = 1
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
        [HttpPost("page-list-order-admin")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<PageListOrderDTO>), description: "GetPageListOrderAdmin Response")]
        public async Task<IActionResult> GetPageListOrderAdmin(PageListOrderAdminRequest request)
        {
            var response = new BaseResponseMessagePage<PageListOrderDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstOrder = _context.Order.Include(x => x.AddressUu).ThenInclude(x => x.UserUu)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.MatpNavigation)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.MaqhNavigation)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.Xa)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.ProductUu).ThenInclude(x => x.ProductImage)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.SizeUu)
                                             .Where(x => x.State == request.State && x.Status == 1)
                                             .Where(x => string.IsNullOrEmpty(request.Keyword) || EF.Functions.Like(x.AddressUu.Fullname, $"%{request.Keyword}"))
                                             .ToList();
                var totalcount = lstOrder.Count;
                if (lstOrder != null && lstOrder.Count > 0)
                {
                    var result = lstOrder.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListOrderDTO>();
                    }
                    foreach (var order in result)
                    {
                        var convertItemDTO = new PageListOrderDTO()
                        {
                            Uuid = order.Uuid,
                            Code = order.Code,
                            User = new ShortCategoryDTO()
                            {
                                Uuid = order.AddressUu.UserUu.Uuid,
                                Name = order.AddressUu.UserUu.Fullname,
                                Status = order.AddressUu.UserUu.Status,
                            },
                            UserAddress = new ShortUserAddressDTO()
                            {
                                Uuid = order.AddressUu.Uuid,
                                UserUuid = order.AddressUu.UserUuid,
                                Fullname = order.AddressUu.Fullname,
                                PhoneNumber = order.AddressUu.PhoneNumber,
                                TP = order.AddressUu.MatpNavigation != null ? new InfoCatalogDTO
                                {
                                    Uuid = order.AddressUu.MatpNavigation.Matp,
                                    Name = order.AddressUu.MatpNavigation.Name

                                } : null,
                                QH = order.AddressUu.MaqhNavigation != null ? new InfoCatalogDTO
                                {
                                    Uuid = order.AddressUu.MaqhNavigation.Maqh,
                                    Name = order.AddressUu.MaqhNavigation.Name

                                } : null,
                                XA = order.AddressUu.Xa != null ? new InfoCatalogDTO
                                {
                                    Uuid = order.AddressUu.Xa.Xaid,
                                    Name = order.AddressUu.Xa.Name
                                } : null,
                            },
                            Items = order.OrderItem.Where(x => x.Status == 1).Select(item => new OrderItemForPageListOrderAdmin()
                            {
                                Uuid = item.Uuid,
                                Product = item.ProductVariantUu.ProductUu != null ? new ShortProductDTO()
                                {
                                    Uuid = item.ProductVariantUu.ProductUu.Uuid,
                                    ProductName = item.ProductVariantUu.ProductUu.ProductName,
                                    Code = item.ProductVariantUu.ProductUu.Code,
                                    ImagesPath = item.ProductVariantUu.ProductUu.ProductImage.Where(x => x.IsDefault == true).Select(p => p.Path).FirstOrDefault(),
                                } : null,
                                SizeCategory = item.ProductVariantUu.SizeUu != null ? new ShortCategoryDTO()
                                {
                                    Uuid = item.ProductVariantUu.SizeUu.Uuid,
                                    Name = item.ProductVariantUu.SizeUu.SizeName,
                                    Status = item.ProductVariantUu.SizeUu.Status,
                                } : null,
                                Price = item.Price,
                                Quantity = item.Quantity,
                                Status = item.Status,
                            }).ToList(),
                            TotalCount = order.OrderItem.Where(x => x.OrderUuid == order.Uuid && x.Status == 1).Count(),
                            TotalPrice = order.TotalPrice,
                            State = order.State,
                            Note = order.Note,
                            TimeCreated = order.TimeCreated,
                            Status = order.Status,
                        };
                        response.Data.Items.Add(convertItemDTO);
                    }
                    response.Data.Pagination = new Paginations()
                    {
                        TotalPage = result.TotalPages,
                        TotalCount = result.TotalCount,
                    };

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
