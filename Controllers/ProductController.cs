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
using Microsoft.VisualStudio.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Product Controller")]
    public class ProductController : BaseController
    {
        private readonly ILogger<ProductController> _logger;
        private readonly DBContext _context;

        public ProductController(DBContext context, ILogger<ProductController> logger)
        {

            _context = context;
            _logger = logger;
        }

        [HttpPost("upsert-product")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertProduct Response")]
        public async Task<IActionResult> UpsertProduct(UpsertProductRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                
                if (string.IsNullOrEmpty(request.Uuid))
                {
                    var check = _context.Product.Where(x => x.Code == request.Code).FirstOrDefault();
                    if (check != null)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATE_PRODUCT);
                    }
                    var product = new Product()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        CatUuid = request.CatUuid,
                        ColorUuid = request.ColorUuid,
                        Code = request.Code,
                        ProductName =request.ProductName,
                        ShortDescription = request.ShortDescription,
                        Description = request.Description,
                        Price = request.Price,
                        TimeCreated = DateTime.Now,
                        Status = 1,

                    };
                    _context.Product.Add(product);
                    

                    if (request.Variants != null && request.Variants.Count > 0)
                    {
                        foreach (var variant in request.Variants)
                        {
                            var productVariant = new ProductVariant()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                ProductUuid = product.Uuid,
                                SizeUuid = variant.SizeUuid,
                                Stock = variant.Stock,
                                Status = 1
                            };
                            _context.ProductVariant.Add(productVariant);
                        }
                        _context.SaveChanges();
                    }
                    if(request.ImagesPath != null)
                    {
                        bool isFirst = true;
                        foreach (var image in request.ImagesPath)
                        {
                            var imagepath = new ProductImage()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                ProductUuid = product.Uuid,
                                Path = image,
                                IsDefault = isFirst,
                                Status = 1
                            };
                            _context.ProductImage.Add(imagepath);
                            isFirst = false;
                        }
                    }
                    _context.SaveChanges();
                }
                else
                {
                    var product = _context.Product.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (product != null)
                    {
                        product.CatUuid = request.CatUuid;
                        product.Code = request.Code;
                        product.ColorUuid = request.ColorUuid;
                        product.ProductName = request.ProductName;
                        product.ShortDescription = request.ShortDescription;
                        product.Description = request.Description;
                        product.Price = request.Price;
                        _context.Update(product);
                        _context.SaveChanges();
                        if(request.Variants != null)
                        {
                            foreach(var variant in request.Variants)
                            {
                                var existVariant = _context.ProductVariant.Where(e => e.ProductUuid == product.Uuid 
                                                                                && e.SizeUuid == variant.SizeUuid).FirstOrDefault();
                                if(existVariant != null)
                                {
                                    existVariant.Stock = variant.Stock;
                                }
                                else
                                {
                                    var productVariant = new Databases.FashionDB.ProductVariant()
                                    {
                                        ProductUuid = product.Uuid,
                                        SizeUuid = variant.SizeUuid,
                                        Stock = variant.Stock,
                                        Status = 1
                                    };
                                    _context.ProductVariant.Add(productVariant);
                                }
                            }
                            _context.SaveChanges();
                        }
                        if (request.ImagesPath != null)
                        {
                            var oldImages = _context.ProductImage
                                .Where(img => img.ProductUuid == product.Uuid)
                                .ToList();
                            var imagesToDelete = oldImages
                                .Where(img => !request.ImagesPath.Contains(img.Path))
                                .ToList();
                            _context.ProductImage.RemoveRange(imagesToDelete);

                            bool isFirst = true;
                            foreach (var image in request.ImagesPath)
                            {
                                var existingImage = oldImages.FirstOrDefault(img => img.Path == image);
                                if (existingImage != null)
                                {
                                    existingImage.IsDefault = isFirst;
                                }
                                else
                                {
                                    var newImage = new ProductImage()
                                    {
                                        Uuid = Guid.NewGuid().ToString(),
                                        ProductUuid = product.Uuid,
                                        Path = image,
                                        IsDefault = isFirst,
                                        Status = 1
                                    };
                                    _context.ProductImage.Add(newImage);
                                }
                                isFirst = false;
                            }
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        response.error.SetErrorCode(ErrorCode.NOT_FOUND);
                    }
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
        [HttpPost("page-list-product")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<PageListProductDTO>), description: "GetPageListProduct Response")]
        public async Task<IActionResult> GetPageListProduct(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListProductDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstProduct = _context.Product.Include(x=> x.ProductVariant).Where(x => x.Status == 1).ToList();
                var totalcount = lstProduct.Count();
                if (lstProduct != null && lstProduct.Count > 0)
                {
                    var result = lstProduct.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListProductDTO>();
                    }
                    foreach (var product in result)
                    {
                        var convertItemDTO = new PageListProductDTO()
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
                            Category = _context.Category.Where(p => p.Uuid == product.CatUuid && p.Status == 1).Select(p => new ShortCategoryDTO
                            {
                                Uuid = p.Uuid,
                                Name = p.Name,
                                Status = p.Status
                            }).FirstOrDefault(),
                            Selled = _context.Order.Include(x => x.OrderItem)
                                                   .Where(x => x.State == 2) 
                                                   .SelectMany(x => x.OrderItem) 
                                                   .Where(x => x.ProductVariantUu.ProductUuid == product.Uuid)
                                                   .Sum(x => x.Quantity), 
                            Price = product.Price,
                            ImagesPath = _context.ProductImage.Where(x => x.ProductUuid == product.Uuid && x.IsDefault == true).Select(x => x.Path).FirstOrDefault(),
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

        [HttpPost("product-detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductDTO), description: "GetProductDetail Response")]
        public async Task<IActionResult> GetProductDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ProductDTO>();

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
                    response.Data = new ProductDTO()
                    {
                        Uuid = productdetail.Uuid,
                        CatUuid = productdetail.CatUuid,
                        ColorUuid = productdetail.ColorUuid,
                        Code = productdetail.Code,
                        ProductName = productdetail.ProductName,
                        ShortDescription = productdetail.ShortDescription,
                        Description = productdetail.Description,
                        Price = productdetail.Price,
                        TimeCreated = productdetail.TimeCreated,
                        Status = productdetail.Status,
                        Variants = _context.ProductVariant.Where(v => v.ProductUuid == productdetail.Uuid)
                        .Select(v => new Variant
                        {
                            Uuid = v.Uuid,
                            SizeUuid = v.SizeUuid,
                            SizeName = v.SizeUu.SizeName,
                            Stock = v.Stock,
                            Status = v.Status,
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

        [HttpPost("full-product-detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProductDetailDTO), description: "GetFullProductDetail Response")]
        public async Task<IActionResult> GetFullProductDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ProductDetailDTO>();

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
                    response.Data = new ProductDetailDTO()
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
                        TimeCreated = productdetail.TimeCreated,
                        Status = productdetail.Status,
                        Variants = _context.ProductVariant.Where(v => v.ProductUuid == productdetail.Uuid)
                        .Select(v => new Variant
                        {
                            Uuid = v.Uuid,
                            SizeUuid = v.SizeUuid,
                            SizeName = v.SizeUu.SizeName,
                            Stock = v.Stock,
                            Status = v.Status,
                            
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
        [HttpPost("update-product-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateProductStatus Response")]
        public async Task<IActionResult> UpdateProductStatus(UuidRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var product = _context.Product.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (product != null)
                {
                    if (product.Status == 1)
                    {
                        product.Status = 0;
                    }
                    else
                    {
                        product.Status = 1;
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
