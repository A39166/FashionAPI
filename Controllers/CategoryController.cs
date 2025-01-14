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
                        ParentUuid = string.IsNullOrEmpty(request.ParentUuid) ? null : request.ParentUuid,
                        Name = request.CategoryName,
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
                        category.ParentUuid = string.IsNullOrEmpty(request.ParentUuid) ? null : request.ParentUuid;
                        category.Name = request.CategoryName;
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
                var lstCategory = _context.Category.Where(x => x.Status == 1).ToList();
                var totalcount = lstCategory.Count();

                if (lstCategory != null && lstCategory.Count > 0)
                {
                    var sorted = BuildCategoryListByParent(lstCategory);
                    var result = sorted.TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<CategoryDTO>();
                    }
                    foreach (var cat in result)
                    {
                        var convertItemDTO = new CategoryDTO()
                        {
                            Uuid = cat.Uuid,
                            ParentUuid = cat.ParentUuid,
                            CategoryName = cat.Name,
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
                        ParentUuid = detail.ParentUuid,
                        CategoryName = detail.Name,
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
        public async Task<IActionResult> UpdateCategoryStatus(UpdateStatusRequest request)
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
                    if(!string.IsNullOrEmpty(category.ParentUuid))
                    {
                        category.Status = request.Status;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var lstChild = _context.Category.Where(x => x.ParentUuid == category.Uuid).ToList();
                        foreach(var child in lstChild)
                        {
                            child.Status = request.Status;
                        }
                        category.Status = request.Status;
                        _context.SaveChanges();
                    }    
                    
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

        [HttpPost("get-parent-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortCategoryDTO>), description: "GetParentCategory Response")]
        public async Task<IActionResult> GetParentCategory(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var parent = _context.Category.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.Name + " ", $"%{request.Keyword}%"))
                                              .Where(x => string.IsNullOrEmpty(x.ParentUuid))
                                                 .ToList();
                if(parent!= null)
                {
                    response.Data = parent.Select(p => new ShortCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.Name,
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


        private List<Category> BuildCategoryListByParent(List<Category> categories)
        {
            var result = new List<Category>();
            var categoryDict = categories.ToDictionary(c => c.Uuid); // Từ điển để tra cứu nhanh

            // Tìm các mục không có cha (cấp cao nhất)
            var roots = categories.Where(c => string.IsNullOrEmpty(c.ParentUuid) || !categoryDict.ContainsKey(c.ParentUuid)).ToList();

            // Duyệt qua các mục gốc và thêm vào danh sách theo thứ tự cha trước, con sau
            foreach (var root in roots)
            {
                AddCategoryAndChildren(root, categories, result);
            }

            return result;
        }

        private void AddCategoryAndChildren(Category category, List<Category> categories, List<Category> result)
        {
            result.Add(category);

            var children = categories.Where(c => c.ParentUuid == category.Uuid).OrderBy(c => c.Name).ToList();
            foreach (var child in children)
            {
                AddCategoryAndChildren(child, categories, result);
            }
        }
    }
}
