namespace FashionAPI.Models.Response
{
    public class BaseResponseMessageItem<DTO> : BaseResponse
    {
        public List<DTO>? Data { get; set; } = new List<DTO>();
    }
}
