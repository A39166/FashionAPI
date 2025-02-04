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
using Microsoft.VisualStudio.Services.Users;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Size Controller")]
    public class SizeController : BaseController
    {
        private readonly ILogger<SizeController> _logger;
        private readonly DBContext _context;

        public SizeController(DBContext context, ILogger<SizeController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert-size")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertSize Response")]
        public async Task<IActionResult> UpsertSize(UpsertSizeRequest request)
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
                    var check = _context.Size.Where(x => x.SizeName.ToLower() == request.SizeName.ToLower()).FirstOrDefault();
                    if(check != null)
                    {
                        throw new ErrorException(ErrorCode.DUPLICATE_SIZE);
                    }
                    var size = new Size()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        SizeName = request.SizeName,
                        Description = request.Description,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.Size.Add(size);
                    _context.SaveChanges();
                }
                else
                //cập nhập dữ liệu
                {
                    var size = _context.Size.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (size != null)
                    {
                        size.SizeName = request.SizeName;
                        size.Description = request.Description;
                        size.Status = 1;
                        _context.Size.Update(size);
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
        [HttpPost("page-list-size")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessagePage<SizeDTO>), description: "GetPageListSize Response")]
        public async Task<IActionResult> GetPageListSize(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<SizeDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstSize = _context.Size.ToList();
                var totalcount = lstSize.Count();

                if (lstSize != null && lstSize.Count > 0)
                {
                    var result = lstSize.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<SizeDTO>();
                    }
                    foreach (var color in result)
                    {
                        var convertItemDTO = new SizeDTO()
                        {
                            Uuid = color.Uuid,
                            SizeName = color.SizeName,
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
        [HttpPost("size-detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(SizeDTO), description: "GetSizeDetail Response")]
        public async Task<IActionResult> GetColorDetail(UuidRequest request)
        {
            var response = new BaseResponseMessage<SizeDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                //TODO: Write code late

                var sizedetail = _context.Size.Where(x => x.Uuid == request.Uuid).SingleOrDefault();
                if (sizedetail != null)
                {
                    response.Data = new SizeDTO()
                    {
                        Uuid = sizedetail.Uuid,
                        SizeName = sizedetail.SizeName,
                        Description = sizedetail.Description,
                        TimeCreated = sizedetail.TimeCreated,
                        Status = sizedetail.Status,
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
        [HttpPost("update-size-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateSizeStatus Response")]
        public async Task<IActionResult> UpdateCastStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var size = _context.Size.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (size != null)
                {
                    if (size.Status == 1)
                    {
                        size.Status = 0;
                    }
                    else
                    {
                        size.Status = 1;
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
