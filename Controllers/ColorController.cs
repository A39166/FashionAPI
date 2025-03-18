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

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Color Controller")]
    public class ColorController : BaseController
    {
        private readonly ILogger<ColorController> _logger;
        private readonly DBContext _context;

        public ColorController(DBContext context, ILogger<ColorController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert-color")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertColor Response")]
        public async Task<IActionResult> UpsertColor(UpsertColorRequest request)
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
                    var check = _context.Color.Where(x => x.ColorName.ToLower() == request.ColorName.ToLower() && x.Status == 1).FirstOrDefault();
                    if (check != null)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATE_COLOR);
                    }
                    var color = new Color()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        Code = request.Code,
                        ColorName = request.ColorName,
                        Description = request.Description,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Color.Add(color);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var color = _context.Color.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (color != null)
                    {
                        color.ColorName = request.ColorName;
                        color.Code = request.Code;
                        color.Description = request.Description;
                        color.Status = 1;
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
        [HttpPost("page-list-color")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<ColorDTO>), description: "GetPageListColor Response")]
        public async Task<IActionResult> GetPageListColor(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<ColorDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstColor = _context.Color.Where(x => x.Status == 1).ToList();
                var totalcount = lstColor.Count();

                if (lstColor != null && lstColor.Count > 0)
                {
                    var result = lstColor.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<ColorDTO>();
                    }
                    foreach (var color in result)
                    {
                        var convertItemDTO = new ColorDTO()
                        {
                            Uuid = color.Uuid,
                            ColorName = color.ColorName,
                            Code = color.Code,
                            Description = color.Description,
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
        [HttpPost("color-detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(ColorDTO), description: "GetColorDetail Response")]
        public async Task<IActionResult> GetColorDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<ColorDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {

                var colordetail = _context.Color.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (colordetail != null)
                {
                    response.Data = new ColorDTO()
                    {
                        Uuid = colordetail.Uuid,
                        ColorName = colordetail.ColorName,
                        Code = colordetail.Code,
                        Description = colordetail.Description,
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
        [HttpPost("update-color-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateColorStatus Response")]
        public async Task<IActionResult> UpdateColorStatus(UpdateStatusRequest request)
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
                    if (color.Status == 1)
                    {
                        color.Status = 0;
                    }
                    else
                    {
                        color.Status = 1;
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
