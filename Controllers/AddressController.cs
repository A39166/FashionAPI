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


    }
    
}
