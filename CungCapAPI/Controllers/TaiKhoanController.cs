using Microsoft.AspNetCore.Mvc;
using CungCapAPI.Application.Interfaces;
using CungCapAPI.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace CungCapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaiKhoanController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ITaiKhoanService _taiKhoanService;
        public TaiKhoanController(IConfiguration configuration, ITaiKhoanService taiKhoanService)
        {
            _configuration = configuration;
            _taiKhoanService = taiKhoanService;
        }
        [HttpPost("dang-nhap")]
        public async Task<ActionResult> KiemTraMatKhau([FromBody] TaiKhoanGuiVe request)
        {
            int NguoiDungId = await _taiKhoanService.KiemTraMatKhau(request.TaiKhoan, request.MatKhau);
            if (NguoiDungId == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Sai tài khoản hoặc mật khẩu!"
                });
            }
            else
            {
                var result = await _taiKhoanService.CapJTW(NguoiDungId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Đăng nhập thành công!",
                    data = result.data
                });
            }
        }
        [HttpPost("dang-xuat")]
        public async Task<ActionResult> DangXuat([FromBody] RefreshRequest request)
        {
            if (request.RefreshToken.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Đăng xuất không thành công!"
                });
            }
            else
            {
                bool result = await _taiKhoanService.XoaRefreshToken(request.RefreshToken);
                if (!result)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Đăng xuất không thành công!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Đăng xuất thành công!"
                    });
                }
            }
        }
        [Authorize]
        [HttpGet("thong-tin-nguoi-dung")]
        public async Task<ActionResult> LayThongTinNguoiDung()
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId")!.Value);
            var result = await _taiKhoanService.LayThongTinNguoiDung(NguoiDungId);
            return new JsonResult(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                data = result.data
            });
        }
        [HttpPost("cap-lai-access-token")]
        public async Task<ActionResult> CapLaiAccessToken([FromBody] RefreshRequest request)
        {
            string RefreshToken = request.RefreshToken;
            if (string.IsNullOrEmpty(RefreshToken))
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Refresh Token bị null rồi!"
                });
            }
            var result = await _taiKhoanService.CapLaiAccessToken(RefreshToken);
            if (result.success == false)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = result.message
                });
            }
            else
            {
                return new JsonResult(new
                {
                    success = true,
                    message = result.message,
                    data = result.data
                });
            }
        }
        [HttpPost("dang-ky-tai-khoan")]
        public async Task<ActionResult> DangKyTaiKhoan([FromBody] DangKyTaiKhoan request)
        {
            if (request.name.IsNullOrEmpty() || request.email.IsNullOrEmpty() || request.phone_number.IsNullOrEmpty() || request.account_login.IsNullOrEmpty() || request.password_login.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Chưa điền đầy đủ thông tin!"
                });
            }
            else
            {
                bool result = await _taiKhoanService.KiemTraTaiKhoanTonTai(request.email, request.phone_number, request.account_login);
                if (result == false)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Tài khoản đã tồn tại!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Oke rồi đấy!"
                    });
                }
            }    
        }
    }
}
