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
    [SwaggerTag("Address Controller")]
    public class AddressController : BaseController
    {
        private readonly ILogger<AddressController> _logger;
        private readonly DBContext _context;

        public AddressController(DBContext context, ILogger<AddressController> logger)
        {

            _context = context;
            _logger = logger;
        }
        [HttpPost("upsert-address")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpsertAddress Response")]
        public async Task<IActionResult> UpsertAddress(UpsertAddressRequest request)
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
                    var address = new UserAddress()
                    {
                        Uuid = Guid.NewGuid().ToString(),
                        UserUuid = validToken.UserUuid,
                        Fullname = request.FullName,
                        PhoneNumber = request.PhoneNumber,
                        Address = request.Address,
                        Maqh = string.IsNullOrEmpty(request.Maqh) ? null : request.Maqh,
                        Matp = string.IsNullOrEmpty(request.Matp) ? null : request.Matp,
                        Xaid = string.IsNullOrEmpty(request.Xaid) ? null : request.Xaid,
                        TimeCreated = DateTime.Now,
                        Status = 1,
                    };
                    _context.UserAddress.Add(address);
                    _context.SaveChanges();
                }
                else
                {
                    var address = _context.UserAddress.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
                    if (address != null)
                    {
                        address.Fullname = request.FullName;
                        address.PhoneNumber = request.PhoneNumber;
                        address.Address = request.Address;
                        address.Maqh = string.IsNullOrEmpty(request.Maqh) ? null : request.Maqh;
                        address.Matp = string.IsNullOrEmpty(request.Matp) ? null : request.Matp;
                        address.Xaid = string.IsNullOrEmpty(request.Xaid) ? null : request.Xaid;
                        _context.UserAddress.Update(address);
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

        [HttpPost("page-list-address")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PageListAddressDTO>), description: "GetPageListAddress Response")]
        public async Task<IActionResult> GetPageListAddress(DpsPagingParamBase request)
        {
            var response = new BaseResponseMessagePage<PageListAddressDTO>();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var lstAddress = _context.UserAddress.Where(x => x.UserUuid == validToken.UserUuid && x.Status == 1).ToList();
                var totalcount = lstAddress.Count();

                if (lstAddress != null && lstAddress.Count > 0)
                {
                    var result = lstAddress.OrderByDescending(x => x.Id).TakePage(request.Page, request.PageSize);
                    if (result != null && result.Count > 0)
                    {
                        response.Data.Items = new List<PageListAddressDTO>();
                    }
                    foreach (var address in result)
                    {
                        var convertItemDTO = new PageListAddressDTO()
                        {
                            Uuid = address.Uuid,
                            Fullname = address.Fullname,
                            PhoneNumber = address.PhoneNumber,
                            Address = address.Address,
                            Status = address.Status,
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
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.error.SetErrorCode(ErrorCode.BAD_REQUEST, ex.Message);
                _logger.LogError(ex.Message);

                return BadRequest(response);
            }
        }

        [HttpPost("detail-address")]
        [SwaggerResponse(statusCode: 200, type: typeof(AddressDTO), description: "DetailAddress Response")]
        public async Task<IActionResult> DetailAddress(UuidRequest request)
        {
            var response = new BaseResponseMessage<AddressDTO>();
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
            try
            {
                var address = _context.UserAddress.Include(t => t.MatpNavigation).Include(t => t.MaqhNavigation).Include(t => t.Xa)
                    .Where(x => x.Uuid == validToken.UserUuid).SingleOrDefault(); ;
                if (address == null)
                {
                    throw new ErrorException(ErrorCode.USER_NOTFOUND);
                }
                else
                {
                    response.Data = new AddressDTO()
                    {
                        Uuid = address.Uuid,
                        UserUuid = address.UserUuid,
                        Fullname = address.Fullname,
                        PhoneNumber = address.PhoneNumber,
                        Address = address.Address,
                        TP = address.MatpNavigation != null ? new InfoCatalogDTO
                        {
                            Uuid = address.MatpNavigation.Matp,
                            Name = address.MatpNavigation.Name 
                            
                        } : null,
                        QH = address.MaqhNavigation != null ? new InfoCatalogDTO
                        {
                            Uuid = address.MaqhNavigation.Maqh,
                            Name = address.MaqhNavigation.Name

                        } : null,
                        XA = address.Xa != null ? new InfoCatalogDTO
                        {
                            Uuid = address.Xa.Xaid,
                            Name = address.Xa.Name

                        } : null,
                        TimeCreated = address.TimeCreated,
                        Status = address.Status,
                    };
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
        /*[HttpPost("update-address-status")]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponse), description: "UpdateAddressStatus Response")]
        public async Task<IActionResult> UpdateAddressStatus(UpdateStatusRequest request)
        {
            var response = new BaseResponse();

            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }

            try
            {
                var address = _context.UserAddress.Where(x => x.Uuid == request.Uuid).SingleOrDefault();

                if (address != null)
                {
                    if (address.Status == 1)
                    {
                        address.Status = 0;
                    }
                    else
                    {
                        address.Status = 1;
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
        }*/

    }
    
}
