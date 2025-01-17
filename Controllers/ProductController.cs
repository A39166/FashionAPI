﻿using FashionAPI.Models.Request;
using FashionAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FashionAPI.Enums;
using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using FashionAPI.Extensions;
using FashionAPI.Configuaration;

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
                    var product = new Product()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        CatUuid = request.CatUuid,
                        Code = request.Code,
                        ProductName =request.ProductName,
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
                            var productVariant = new Databases.FashionDB.ProductVariant()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                ProductUuid = product.Uuid,
                                SizeUuid = variant.SizeUuid,
                                ColorUuid = variant.ColorUuid,
                                Stock = variant.Stock,
                            };
                            _context.ProductVariant.Add(productVariant);
                        }
                        _context.SaveChanges();
                    }
                    if(request.ImagesPath != null)
                    {
                        foreach (var image in request.ImagesPath)
                        {
                            var imagepath = new ProductImage()
                            {
                                Uuid = Guid.NewGuid().ToString(),
                                ProductUuid = product.Uuid,
                                Path = image,
                            };
                            _context.ProductImage.Add(imagepath);
                        }
                    }
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var product = _context.Product.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (product != null)
                    {
                        product.CatUuid = request.CatUuid;
                        product.Code = request.Code;
                        product.ProductName = request.ProductName;
                        product.Description = request.Description;
                        product.Price = request.Price;
                        _context.Update(product);
                        _context.SaveChanges();
                        if(request.Variants != null)
                        {
                            foreach(var variant in request.Variants)
                            {
                                var existVariant = _context.ProductVariant.Where(e => e.ProductUuid == product.Uuid && e.SizeUuid == variant.SizeUuid
                                                                                && e.ColorUuid == variant.ColorUuid).FirstOrDefault();
                                if(existVariant != null)
                                {
                                    variant.Stock = existVariant.Stock;
                                }
                                else
                                {
                                    var productVariant = new Databases.FashionDB.ProductVariant()
                                    {
                                        ProductUuid = product.Uuid,
                                        SizeUuid = variant.SizeUuid,
                                        ColorUuid = variant.ColorUuid,
                                        Stock = variant.Stock,
                                    };
                                    _context.ProductVariant.Add(productVariant);
                                }
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
                var lstColor = _context.Color.ToList();
                var totalcount = lstColor.Count();

                if (lstColor != null && lstColor.Count > 0)
                {
                    var result = lstColor.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListProductDTO>();
                    }
                    foreach (var color in result)
                    {
                        var convertItemDTO = new PageListProductDTO()
                        {
                            Uuid = color.Uuid,
                            ColorName = color.ColorName,
                            TimeCreated = color.TimeCreated,
                            Status = color.Status,
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
        [SwaggerResponse(statusCode: 200, type: typeof(ColorDTO), description: "GetProductDetail Response")]
        public async Task<IActionResult> GetProductDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ColorDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var colordetail = _context.Color.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (colordetail != null)
                {
                    response.Data = new ColorDTO()
                    {
                        Uuid = colordetail.Uuid,
                        ColorName = colordetail.ColorName,
                        TimeCreated = colordetail.TimeCreated,
                        Status = colordetail.Status,
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
        public async Task<IActionResult> UpdateProductStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var color = _context.Color.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (color != null)
                {
                    color.Status = request.Status;
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
