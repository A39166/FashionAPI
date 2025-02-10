using FashionAPI.Models.Request;
using FashionAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FashionAPI.Enums;
using FashionAPI.Databases.FashionDB;
using Microsoft.EntityFrameworkCore;
using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using FashionAPI.Extensions;
using FashionAPI.Configuaration;
using System.Drawing;
using ESDManagerApi.Models.Request;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Category Controller")]
    public class CategoryController : BaseController
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly DBContext _context;

        public CategoryController(DBContext context, ILogger<CategoryController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertCategory Response")]
        public async Task<IActionResult> UpsertCategory(UpsertCategoryRequest request)
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
                    
                    var category = new Category()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Name = request.CategoryName,
                        Path = request.Path,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Category.Add(category);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var category = _context.Category.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (category != null)
                    {
                        category.Name = request.CategoryName;
                        category.Path = request.Path;
                        _context.SaveChanges();
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
        [HttpPost("page-list-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<CategoryDTO>), description: "GetPageListCategory Response")]
        public async Task<IActionResult> GetPageListCategory(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<CategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstCategory = _context.Category.ToList();
                var totalcount = lstCategory.Count();

                if (lstCategory != null && lstCategory.Count > 0)
                {
                    var result = lstCategory.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<CategoryDTO>();
                    }
                    foreach (var cat in result)
                    {
                        var convertItemDTO = new CategoryDTO()
                        {
                            Uuid = cat.Uuid,
                            CategoryName = cat.Name,
                            Path = cat.Path,
                            TimeCreated = cat.TimeCreated,
                            Status = cat.Status,
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
        [HttpPost("category-detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(CategoryDTO), description: "GetCategoryDetail Response")]
        public async Task<IActionResult> GetCategoryDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<CategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var detail = _context.Category.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (detail != null)
                {
                    response.Data = new CategoryDTO()
                    {
                        Uuid = detail.Uuid,
                        CategoryName = detail.Name,
                        Path = detail.Path,
                        TimeCreated = detail.TimeCreated,
                        Status = detail.Status,
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
        [HttpPost("update-category-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateCategoryStatus Response")]
        public async Task<IActionResult> UpdateCategoryStatus(UuidRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var category = _context.Category.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (category != null)
                {
                    
                        if (category.Status == 1)
                        {
                            category.Status = 0;
                        }
                        else
                        {
                            category.Status = 1;
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

        [HttpPost("get-category-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ClientCategoryDTO>), description: "GetclientCategory Response")]
        public async Task<IActionResult> GetclientCategory()
        {
            var response = new BaseResponseMessageItem<ClientCategoryDTO>();

            try
            {
                var cat = _context.Category.Where(x => x.Status == 1)
                                                 .ToList();
                if (cat != null)
                {
                    response.Data = cat.Select(p => new ClientCategoryDTO
                    {
                        Uuid = p.Uuid,
                        CategoryName = p.Name,
                        Path = p.Path,
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
