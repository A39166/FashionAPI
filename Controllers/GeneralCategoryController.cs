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
using ESDManagerApi.Models.Request;

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
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortColorCategoryDTO>), description: "GetColorCategory Response")]
        public async Task<IActionResult> GetColorCategory(CategoryRequest request)
        {
            var response = new BaseResponseMessageItem<ShortColorCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var color = _context.Color.Where(x => x.Status == 1)
                                          .ToList();
                if (color != null)
                {
                    response.Data = color.Select(p => new ShortColorCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.ColorName,
                        Code = p.Code,
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

        [HttpPost("get-color-category-client")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<ShortColorCategoryDTO>), description: "GetColorCategoryClient Response")]
        public async Task<IActionResult> GetColorCategoryClient()
        {
            var response = new BaseResponseMessageItem<ShortColorCategoryDTO>();
            try
            {
                var color = _context.Color.Where(x => x.Status == 1)
                                          .ToList();
                if (color != null)
                {
                    response.Data = color.Select(p => new ShortColorCategoryDTO
                    {
                        Uuid = p.Uuid,
                        Name = p.ColorName,
                        Code = p.Code,
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
        public async Task<IActionResult> GetSizeCategory(CategoryRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var size = _context.Size.Where(x => x.Status == 1).ToList();
                if (size != null)
                {
                    response.Data = size.Select(p => new ShortCategoryDTO
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
        public async Task<IActionResult> GetCategory(CategoryRequest request)
        {
            var response = new BaseResponseMessageItem<ShortCategoryDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var cat = _context.Category.Where(x => x.Status == 1)
                                                 .ToList();
                if (cat != null)
                {
                    response.Data = cat.Select(p => new ShortCategoryDTO
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
