using Microsoft.Build.Execution;

namespace CungCapAPI.Application.DTOs
{
    public class LoginRequest
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
    public class LoginResult<T>
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public T data { get; set; }
    }
    public class jwtTokens
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
    public class ThongTinNguoiDung
    {
        public int NguoiDungId { get; set; }
        public string TenNguoiDung { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string VaiTro { get; set; }
    }
}
