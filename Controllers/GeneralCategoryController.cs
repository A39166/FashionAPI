using FashionAPI.Models.Request;
using FashionAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FashionAPI.Enums;
using FashionAPI.Databases.FashionDB;
using Microsoft.EntityFrameworkCore;
using FashionAPI.Models.BaseRequest;
using Microsoft.AspNetCore.Identity.Data;
using FashionAPI.Models.DataInfo;
using FashionAPI.Extensions;
using System.IO;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;
using FashionAPI.Configuaration;
using FashionAPI.Models.BaseRequest;
using FashionAPI.Utils;
using System.Linq;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("General Category Controller")]
    public class GeneralCategoryController : BaseController
    {
        private readonly ILogger<GeneralCategoryController> _logger;
        private readonly DBContext _context;

        public GeneralCategoryController(DBContext context, ILogger<GeneralCategoryController> logger)
        {

            _context = context;
            _logger = logger;
        }

        [HttpPost("get-color-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortCategoryDTO>), description: "GetColorCategory Response")]
        public async Task<IActionResult> GetColorCategory(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var color = _context.Color.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.ColorName + " ", $"%{request.Keyword}%"))
                                                 .Where(x => x.Status == 1)
                                                 .ToList();
                if (color != null)
                {
                    response.Data = color.Select(p => new ShortCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.ColorName,
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
        [HttpPost("get-size-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortCategoryDTO>), description: "GetSizeCategory Response")]
        public async Task<IActionResult> GetSizeCategory(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var color = _context.Size.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                        || EF.Functions.Like(x.SizeName + " ", $"%{request.Keyword}%"))
                                                 .Where(x => x.Status == 1)
                                                 .ToList();
                if (color != null)
                {
                    response.Data = color.Select(p => new ShortCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.SizeName,
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
        [HttpPost("get-category")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortCategoryDTO>), description: "GetCategory Response")]
        public async Task<IActionResult> GetCategory(BaseKeywordRequest request)
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
                                                 .ToList();
                if (parent != null)
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

    }
}
