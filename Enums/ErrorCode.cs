using System.ComponentModel;

namespace FashionAPI.Enums
{
    public enum ErrorCode
    {
        [Description("Failed")]
        FAILED = -1,
        [Description("Success")]
        SUCCESS = 0,
        [Description("Token Invalid")]
        TOKEN_INVALID = 2,
        [Description("System error")]
        SYSTEM_ERROR = 3,
        [Description("Database failed")]
        DB_FAILED = 4,
        [Description("Thư mục chứa ảnh chưa được cấu hình")]
        FOLDER_IMAGE_NOT_FOUND = 5,
        [Description("Định dạng tập tin không được hỗ trợ")]
        DOES_NOT_SUPPORT_FILE_FORMAT = 6,
        [Description("Not found")]
        NOT_FOUND = 7,
        [Description("Invalid parameters")]
        INVALID_PARAM = 8,
        [Description("Exists")]
        EXISTS = 9,
        [Description("Key cert invalid")]
        INVALID_CERT = 10,
        [Description("Bad request")]
        BAD_REQUEST = 400,
        [Description("Unauthorization")]
        UNAUTHOR = 401,

        [Description("User locked")]
        USER_LOCKED = 20,
        [Description("Otp invalid")]
        OTP_INVALID = 21,
        [Description("Otp expired")]
        OTP_EXPIRED = 22,
        [Description("Tài khoản đã bị khóa. Vui lòng liên hệ Admin để biết thêm chi tiết")]
        ACCOUNT_LOCKED = 23,

        [Description("Mật khẩu không chính xác. Vui lòng thử lại")]
        INVALID_PASS = 24,

        [Description("Tài khoản không tồn tại. Vui lòng kiểm tra lại")]
        ACCOUNT_NOTFOUND = 25,

        [Description("Email hoặc Mật khẩu sai ! Vui lòng thử lại")]
        WRONG_LOGIN = 26,

        [Description("Tài khoản này đã bị khóa!")]
        LOCKED_ACCOUNT = 27,

        [Description("Tài khoản này không có quyền đăng nhập Admin!")]
        NO_PERMISSION = 28,

        [Description("Tài khoản đã có")]
        DUPLICATE_EMAIL = 29,

        [Description("Không tìm thấy người dùng!")]
        USER_NOTFOUND = 30,

        [Description("Sai mật khẩu cũ!")]
        WRONG_OLD_PASS = 31,

        [Description("Size đã có!")]
        DUPLICATE_SIZE = 32,

        [Description("Màu đã có!")]
        DUPLICATE_COLOR = 33,

        [Description("Tài khoản này không có quyền đăng nhập Admin!")]
        NO_PERMISSION_ACTION = 34,

        [Description("Không thể khóa size do còn sản phẩm tồn kho!")]
        CANT_LOCKED_SIZE = 35,

        [Description("Không tìm thấy địa chỉ!")]
        ADDRESS_NOTFOUND = 36,
    }
}
