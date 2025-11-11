using Microsoft.AspNetCore.Mvc;
using CungCapAPI.Application.Interfaces;
using ModelLibrary;
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
        [HttpPost("/tai-khoan/dang-nhap")]
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
        [HttpPost("/tai-khoan/dang-xuat")]
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
        [HttpGet("/tai-khoan/thong-tin-nguoi-dung")]
        public async Task<ActionResult> LayThongTinNguoiDung()
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            var result = await _taiKhoanService.LayThongTinNguoiDung(NguoiDungId);
            return new JsonResult(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                data = result.data
            });
        }

        [HttpPost("/tai-khoan/cap-lai-access-token")]
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
        [HttpPost("/tai-khoan/dang-ky-tai-khoan")]
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
                    bool DangKy = await _taiKhoanService.DangKyTaiKhoan(request);
                    if (DangKy == false)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Đăng ký không thành công!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Đăng ký thành công!"
                        });
                    }      
                }
            } 
        }
        [Authorize]
        [HttpGet("/tai-khoan/ho-so-tai-khoan")]
        public async Task<ActionResult> LayHoSoTaiKhoan()
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            var result = await _taiKhoanService.LayHoSoTaiKhoan(NguoiDungId);
            return new JsonResult(new
            {
                success = result.success,
                message = result.message,
                data = result.data
            });
        }

        [Authorize]
        [HttpPost("/tai-khoan/doi-mat-khau")]
        public async Task<ActionResult> KiemTraVaDoiMatKhau([FromBody] DoiMatKhau request)
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            var result = await _taiKhoanService.KiemTraVaDoiMatKhau(NguoiDungId, request.matKhauCu, request.matKhauMoi);
            if (result == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Mật khẩu cũ không đúng!"
                });
            }
            else if (result == 1)
            {
                return new JsonResult(new
                {
                    success = true,
                    message = "Đổi mật khẩu thành công!"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Đổi mật không thành công"
                });
            }
        }
        [Authorize]
        [HttpPost("/tai-khoan/doi-thong-tin-nguoi-dung")]
        public async Task<IActionResult> DoiThongTinNguoiDung([FromBody] CaiDatThongTinTaiKhoan request)
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            var result = await _taiKhoanService.DoiThongTinNguoiDung(NguoiDungId, request);
            if (result == false)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Đã có tài khoản khác sử dụng thông tin bạn vừa điền!"
                });
            }
            else
            {
                return new JsonResult(new
                { 
                    success = true,
                    message = "Chỉnh sửa thành công!"
                });
            }
        }

        [Authorize]
        [HttpPost("/tai-khoan/doi-thong-tin-telegram")]
        public async Task<IActionResult> DoiThongTinTelegram([FromBody] CaiDatTelegram request)
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            var result = await _taiKhoanService.DoiThongTinTelegram(NguoiDungId, request);
            if (result == false)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Đã có tài khoản khác sử dụng thông tin bạn vừa điền!"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    success = true,
                    message = "Chỉnh sửa thành công!"
                });
            }
        }

    }
}
