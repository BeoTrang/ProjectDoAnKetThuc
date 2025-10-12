using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class ResultBool
    {
        public int KetQua { get; set; }
    }
    public class NguoiDungId
    {
        public int KetQua { get; set; }
    }

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

    public class Request<T>
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
    public class DangKyTaiKhoan
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string account_login { get; set; }
        public string password_login { get; set; }
    }

    public class Request1
    {
        public bool success { get; set; }
        public string message { get; set; } = string.Empty;
        public Object data { get; set; }
    }
}
