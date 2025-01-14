
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.Response;
using FashionAPI.Models.Request;
using FashionAPI.Configuaration;
using FashionAPI.Enums;

namespace FashionAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Upload Controller")]
    public class UploadController : BaseController
    {
        private readonly DBContext _context;
        private readonly ILogger<UploadController> _logger;
        public UploadController(DBContext context, ILogger<UploadController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPut("upload-image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [SwaggerResponse(statusCode: 200, type: typeof(BaseResponseMessage<string>), description: "UploadImage Response")]
        public async Task<IActionResult> UploadImage([FromForm] UploadFileRequest request)
        {
            var validToken = validateToken(_context);
            if (validToken is null)
            {
                return Unauthorized();
            }
                
            var response = new BaseResponseMessage<string>();

            try
            {

                if (request.FileData != null && request.FileData.Length > 0)
                {
                    if (!Directory.Exists(GlobalSettings.FOLDER_EXPORT))
                    {
                        Directory.CreateDirectory(GlobalSettings.FOLDER_EXPORT);
                    }

                    var folderSavePath = "";

                    if (request.Type == 1)
                    {
                        folderSavePath = $"{GlobalSettings.FOLDER_EXPORT}/{GlobalSettings.SUB_FOLDER_AVATAR}";
                    }
                    else if (request.Type == 2)
                    {
                        folderSavePath = $"{GlobalSettings.FOLDER_EXPORT}/{GlobalSettings.SUB_FOLDER_PRODUCT}";
                    }

                    if (!Directory.Exists(folderSavePath))
                    {
                        Directory.CreateDirectory(folderSavePath);
                    }

                    var ext = Path.GetExtension(request.FileData.FileName);

                    if (GlobalSettings.IMAGES_UPLOAD_EXTENSIONS.Contains(ext))
                    {
                        var newUuid = Guid.NewGuid().ToString();

                        var filename = $"{newUuid}{ext}";

                        var filePath = $"{folderSavePath}/{filename}";

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await request.FileData.CopyToAsync(stream);
                        }

                        response.Data = filePath;
                    }
                    else
                    {
                        response.error.SetErrorCode(ErrorCode.INVALID_PARAM);
                    }
                }
                else
                {
                    response.error.SetErrorCode(ErrorCode.INVALID_PARAM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                response.error.SetErrorCode(ErrorCode.SYSTEM_ERROR);
            }

            return Ok(response);
        }

    }
    
}
