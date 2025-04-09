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
                var featured = _context.Product
                                       .Include(x => x.ProductVariant)
                                       .Where(x => x.Status == 1)
                                       .ToList();

                // Nếu có hơn 4 sản phẩm thì random 4 cái
                var selectedProducts = featured.Count > 4
                    ? featured.OrderBy(x => Guid.NewGuid()).Take(4).ToList()
                    : featured;

                var responseData = selectedProducts.Select(p =>
                {
                    var activeVariants = p.ProductVariant.Where(v => v.Status == 1).ToList();
                    bool isSoldOut = activeVariants.Any() && activeVariants.All(v => v.Stock == 0);

                    return new FeaturedListProductDTO
                    {
                        Uuid = p.Uuid,
                        ProductName = p.ProductName,
                        Code = p.Code,
                        Price = p.Price,
                        ImagesPath = _context.ProductImage
                                             .Where(x => x.ProductUuid == p.Uuid && x.IsDefault == true)
                                             .Select(x => x.Path)
                                             .FirstOrDefault(),
                        isSoldOut = isSoldOut,
                        Status = p.Status,
                    };
                }).ToList();

                response.Data = responseData;

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

        [HttpPost("page-list-product-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<PageListProductClientDTO>), description: "GetPageListProductClient Response")]
        public async Task<IActionResult> GetPageListProductClient(PageListProductClientRequest request)
        {
            var response = new BaseResponseMessagePage<PageListProductClientDTO>();

            try
            {
                var lstProduct = _context.Product.Include(x => x.ProductVariant)
                    .Where(x => string.IsNullOrEmpty(request.CategoryUuid) || x.CatUuid == request.CategoryUuid)
                    .Where(x => string.IsNullOrEmpty(request.ColorUuid) || x.ColorUuid == request.ColorUuid)
                    .Where(x => x.Status == 1)
                    .ToList();
                var totalcount = lstProduct.Count();

                if (lstProduct != null && lstProduct.Count > 0)
                {
                    
                    if(request.Sorted == 1)
                    {
                        lstProduct = lstProduct.OrderBy(x => x.Price).ToList();
                    }
                    else if(request.Sorted == 2)
                    {
                        lstProduct = lstProduct.OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        lstProduct = lstProduct.OrderByDescending(x => x.Id).ToList();
                    }
                    var result = lstProduct.TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListProductClientDTO>();
                    }
                   
                    foreach (var product in result)
                    {
                        var activeVariants = product.ProductVariant.Where(v => v.Status == 1).ToList(); // Chỉ lấy các biến thể hợp lệ
                        bool isSoldOut = activeVariants.Any() && activeVariants.All(v => v.Stock == 0); // Kiểm tra hết hàng
                        var convertItemDTO = new PageListProductClientDTO()
                        {

                            Uuid = product.Uuid,
                            ProductName = product.ProductName,
                            Code = product.Code,
                            Color = _context.Color.Where(p => p.Uuid == product.ColorUuid && p.Status == 1).Select(p => new ShortColorDTO
                            {
                                Uuid = p.Uuid,
                                ColorName = p.ColorName,
                                Code = p.Code,
                                Status = p.Status
                            }).FirstOrDefault(),
                            Price = product.Price,
                            ImagesPath = _context.ProductImage.Where(x => x.ProductUuid == product.Uuid && x.IsDefault == true && x.Status == 1).Select(x => x.Path).FirstOrDefault(),
                            isSoldOut = isSoldOut,
                            Status = product.Status,
                        };
                        response.Data.Items.Add(convertItemDTO);
                    }
                    // trả về thông tin page
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

        [HttpPost("product-detail-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductDetailClientDTO), description: "GetProductDetailClient Response")]
        public async Task<IActionResult> GetProductDetailClient(UuidRequest request)
        {
            var response = new BaseResponseMessage<ProductDetailClientDTO>();

            try
            {
                var productdetail = _context.Product.Include(p => p.ProductVariant)
                                                    .Where(x => x.Uuid == request.Uuid && x.Status == 1).SingleOrDefault();
                bool isSoldOut = !_context.ProductVariant.Where(v => v.ProductUuid == request.Uuid && v.Status == 1).Any(v => v.Stock > 0);
                if (productdetail == null)
                {
                    throw new ErrorException(ErrorCode.PRODUCT_NOTFOUND);
                }
                var productImages = _context.ProductImage.Where(img => img.ProductUuid == productdetail.Uuid && img.Status == 1)
                                                             .OrderByDescending(img => img.IsDefault)
                                                             .Select(img => img.Path)
                                                             .ToList();
                var detail = new ProductDetailClientDTO()
                {
                    Uuid = productdetail.Uuid,
                    Color = _context.Color.Where(p => p.Uuid == productdetail.ColorUuid && p.Status == 1).Select(p => new ShortColorDTO
                    {
                        Uuid = p.Uuid,
                        ColorName = p.ColorName,
                        Code = p.Code,
                        Status = p.Status
                    }).FirstOrDefault(),
                    Category = _context.Category.Where(p => p.Uuid == productdetail.CatUuid && p.Status == 1).Select(p => new ShortCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.Name,
                        Status = p.Status
                    }).FirstOrDefault(),
                    Code = productdetail.Code,
                    ProductName = productdetail.ProductName,
                    ShortDescription = productdetail.ShortDescription,
                    Description = productdetail.Description,
                    Price = productdetail.Price,
                    Status = productdetail.Status,
                    Size = _context.ProductVariant.Where(v => v.ProductUuid == productdetail.Uuid && v.Status == 1)
                    .Select(v => new ShortSizeCategoryDTO
                    {
                        Uuid = v.SizeUu.Uuid,
                        Name = v.SizeUu.SizeName,
                        Stock = v.Stock,
                        Status = v.SizeUu.Status,
                    })
                    .ToList(),
                    ImagesPath = productImages

                };
                response.Data = detail;
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

        [HttpPost("page-list-order-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<PageListOrderDTO>), description: "GetPageListOrderClient Response")]
        public async Task<IActionResult> GetPageListOrderClient(PageListOrderClientRequest request)
        {
            var response = new BaseResponseMessageItem<PageListOrderDTO>();
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
                                             .Where(x => x.AddressUu.UserUuid == validToken.UserUuid)
                                             .OrderByDescending(x => x.TimeCreated)
                                             .ToList();
                if (lstOrder != null)
                {
                    response.Data = lstOrder.Select(order => new PageListOrderDTO
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
                                Status = item.ProductVariantUu.ProductUu.Status,
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
