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
                order.Code = DateTime.Now.ToString("yyMMddHHmmss"); // YYYYMMDDHHmmss
                _context.Order.Add(order);
                foreach (var item in request.Product)
                {
                    var variant = _context.ProductVariant.Where(x => x.ProductUuid == item.ProductUuid && x.SizeUuid == item.SizeUuid)
                                                         .FirstOrDefault();
                    var orderitem = new OrderItem()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        OrderUuid = order.Uuid,
                        ProductVariantUuid = variant.Uuid,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Status = 1
                    };
                    _context.OrderItem.Add(orderitem);
                    /*var cartItem = _context.CartItem.Where(x => x.ProductVariantUuid == variant.Uuid && )*/
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
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.ProductUu).ThenInclude(x => x.ColorUu)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.SizeUu)
                                             .Where(x => x.State == request.State && x.Status == 1)
                                             .Where(x => string.IsNullOrEmpty(request.Keyword) || EF.Functions.Like(x.AddressUu.Fullname, $"%{request.Keyword}"))
                                             .OrderByDescending(x => x.TimeCreated)
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
                                Address = order.AddressUu.Address,
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
                                ColorCategory = item.ProductVariantUu.ProductUu.ColorUu != null ? new ShortColorCategoryDTO
                                {
                                    Uuid = item.ProductVariantUu.ProductUu.ColorUu.Uuid,
                                    Name = item.ProductVariantUu.ProductUu.ColorUu.ColorName,
                                    Code = item.ProductVariantUu.ProductUu.ColorUu.Code,
                                    Status = item.ProductVariantUu.ProductUu.ColorUu.Status,
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
                            TimeUpdate = order.TimeUpdate,
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

        [HttpPost("detail-order")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<PageListOrderDTO>), description: "DetailOrder Response")]
        public async Task<IActionResult> DetailOrder(UuidRequest request)
        {
            var response = new BaseResponseMessage<PageListOrderDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var order = _context.Order.Include(x => x.AddressUu).ThenInclude(x => x.UserUu)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.MatpNavigation)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.MaqhNavigation)
                                             .Include(x => x.AddressUu).ThenInclude(t => t.Xa)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.ProductUu).ThenInclude(x => x.ProductImage)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.ProductUu).ThenInclude(x => x.ColorUu)
                                             .Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu).ThenInclude(x => x.SizeUu)
                                             .Where(x => x.Uuid == request.Uuid).FirstOrDefault(); ;
                if (order == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                else
                {
                    response.Data = new PageListOrderDTO()
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
                            Address = order.AddressUu.Address,
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
                            ColorCategory = item.ProductVariantUu.ProductUu.ColorUu != null ? new ShortColorCategoryDTO
                            {
                                Uuid = item.ProductVariantUu.ProductUu.ColorUu.Uuid,
                                Name = item.ProductVariantUu.ProductUu.ColorUu.ColorName,
                                Code = item.ProductVariantUu.ProductUu.ColorUu.Code,
                                Status = item.ProductVariantUu.ProductUu.ColorUu.Status,
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
                        TimeUpdate = order.TimeUpdate,
                        Status = order.Status,
                    };
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


        [HttpPost("update-order-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateOrderStatus Response")]
        public async Task<IActionResult> UpdateOrderStatus(UuidRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var order = _context.Order.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (order != null)
                {
                    order.Status = 0;
                    _context.SaveChanges();
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.NOT_FOUND);
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

        [HttpPost("change-order-state")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "ChangeOrderState Response")]
        public async Task<IActionResult> ChangeOrderState(UuidRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var order = _context.Order.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (order != null)
                {
                    if(order.State == 0)
                    {
                        order.State = 1;
                        order.TimeUpdate = DateTime.Now;
                    }
                    else if(order.State == 1)
                    {
                        order.State = 2;
                        order.TimeUpdate = DateTime.Now;
                    }
                    _context.SaveChanges();
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.NOT_FOUND);
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

        [HttpPost("cancel-order")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "CancelOrder Response")]
        public async Task<IActionResult> CancelOrder(UuidRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var order = _context.Order.Include(x => x.OrderItem).ThenInclude(x => x.ProductVariantUu)
                    .Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                
                if (order != null)
                {
                    if(order.State != 0)
                    {
                        throw new ErrorException(ErrorCode.CANT_CANCEL_ORDER);
                    }
                    else
                    {
                        order.State = 3;
                        order.TimeUpdate = DateTime.Now;
                        foreach (var item in order.OrderItem)
                        {
                            var variant = item.ProductVariantUu;
                            if (variant != null)
                            {
                                variant.Stock += item.Quantity;
                            }
                        };
                    }
                    _context.SaveChanges();
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.NOT_FOUND);
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
