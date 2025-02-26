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
using System.Net;
using System;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Province Controller")]
    public class ProvinceController : BaseController
    {
        private readonly ILogger<ProvinceController> _logger;
        private readonly DBContext _context;

        public ProvinceController(DBContext context, ILogger<ProvinceController> logger)
        {

            _context = context;
            _logger = logger;
        }

        [HttpPost("get-list-province")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<Province>), description: "GetListProvince Response")]
        public async Task<IActionResult> GetListProvince(BaseKeywordRequest request)
        {
            var response = new BaseResponseMessageItem<Province>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var province = _context.Province.Where(x => string.IsNullOrEmpty(request.Keyword) 
                                                  || EF.Functions.Like(x.Name + "", $"%{request.Keyword}%")).ToList();
                if (province != null)
                {
                    response.Data = province.Select(p => new Province
                    {
                        Matp = p.Matp,
                        Name = p.Name,
                        Type = p.Type,
                        Slug = p.Slug,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("get-list-district")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<District>), description: "GetListDistrict Response")]
        public async Task<IActionResult> GetListDistrict(FindProvinceRequest request)
        {
            var response = new BaseResponseMessageItem<District>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var province = _context.District.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                  || EF.Functions.Like(x.Name + "", $"%{request.Keyword}%"))
                                                .Where(x => x.Matp == request.IdParent).ToList();
                if (province != null)
                {
                    response.Data = province.Select(p => new District
                    {
                        Matp = p.Matp,
                        Name = p.Name,
                        Type = p.Type,
                        Maqh = p.Maqh,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("get-list-ward")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessageItem<Ward>), description: "GetListWard Response")]
        public async Task<IActionResult> GetListWard(FindProvinceRequest request)
        {
            var response = new BaseResponseMessageItem<Ward>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var province = _context.Ward.Where(x => string.IsNullOrEmpty(request.Keyword)
                                                  || EF.Functions.Like(x.Name + "", $"%{request.Keyword}%"))
                                            .Where(x => x.Maqh == request.IdParent).ToList();
                if (province != null)
                {
                    response.Data = province.Select(p => new Ward
                    {
                        Xaid = p.Xaid,
                        Name = p.Name,
                        Type = p.Type,
                        Maqh = p.Maqh,
                    }).ToList();
                }

                return Ok(response);
            }
            catch (ErrorException ex)
            {
                response.error.SetErrorCode(ex.Code);
                return BadRequest(response);
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