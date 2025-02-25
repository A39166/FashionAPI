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
        [HttpPost("page-list-product-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<PageListProductClientDTO>), description: "GetPageListProductClient Response")]
        public async Task<IActionResult> GetPageListProductClient(PageListProductClientRequest request)
        {
            var response = new BaseResponseMessagePage<PageListProductClientDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstProduct = _context.Product.Include(x => x.ProductVariant)
                    /*.Where(x => string.IsNullOrEmpty(request.Keyword) || EF.Functions.Like(x.Name + "" + x.Code, $"%{request.Keyword}%")*/
                    .Where(x => string.IsNullOrEmpty(request.CategoryUuid) || x.CatUuid == request.CategoryUuid)
                    .Where(x => string.IsNullOrEmpty(request.ColorUuid) || x.ColorUuid == request.ColorUuid)
                    .Where(x => x.Status == 1)
                    .ToList();
                var totalcount = lstProduct.Count();

                if (lstProduct != null && lstProduct.Count > 0)
                {
                    var result = lstProduct.TakePage(request.Page, request.PageSize);
                    if(request.Sorted == 1)
                    {
                        result.OrderBy(x => x.Price);
                    }
                    else if(request.Sorted == 2)
                    {
                        result.OrderByDescending(x => x.Price);
                    }
                    else
                    {
                        result.OrderByDescending(x => x.Id);
                    }
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListProductClientDTO>();
                    }
                    foreach (var product in result)
                    {
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
        public async Task<IActionResult> GetProductDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ProductDetailClientDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var productdetail = _context.Product.Include(p => p.ProductVariant)
                                                    .Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (productdetail != null)
                {
                    var productImages = _context.ProductImage.Where(img => img.ProductUuid == productdetail.Uuid && img.Status == 1)
                                                             .OrderByDescending(img => img.IsDefault)
                                                             .Select(img => img.Path)
                                                             .ToList();
                    response.Data = new ProductDetailClientDTO()
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
                        Size = _context.ProductVariant.Where(v => v.ProductUuid == productdetail.Uuid)
                        .Select(v => new ShortCategoryDTO
                        {
                            Uuid = v.SizeUu.Uuid,
                            Name = v.SizeUu.SizeName,
                            Status = v.SizeUu.Status,
                        })
                        .ToList(),
                        ImagesPath = productImages

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
