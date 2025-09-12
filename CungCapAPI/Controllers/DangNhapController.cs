using Azure.Core;
using CungCapAPI.Helpers;
using CungCapAPI.Models;
using CungCapAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace CungCapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DangNhapController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext SqlServer;
        private readonly IRedisService _redis;
        public DangNhapController(ApplicationDbContext context, IRedisService redis, IConfiguration configuration)
        {
            SqlServer = context;
            _redis = redis;
            _configuration = configuration;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] Models.TaiKhoanGuiVe request)
        {
            var user = await SqlServer.NguoiDungs.FirstOrDefaultAsync(u => u.TaiKhoan == request.TaiKhoan);
            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Nhập sai tài khoản hoặc mật khẩu!"
                });
            }
            if (!PasswordHelper.KiemTraMatKhau(request.MatKhau, user.MatKhauMaHoa, user.MatKhauMuoi))
            {
                return Json(new
                {
                    success = false,
                    message = "Nhập sai tài khoản hoặc mật khẩu!"
                });
            }

            var ClaimAccessToken = SqlServer.ThongTinNguoiDung
                .FromSqlRaw("EXEC SP_LayThongTinNguoiDungChoAccessToken @NguoiDungId",
                    new SqlParameter("@NguoiDungId", user.NguoiDungId)
                )
                .AsNoTracking()
                .AsEnumerable() 
                .FirstOrDefault();


            var accessToken = JwtHelper.GenerateAccessToken(ClaimAccessToken, HttpContext.RequestServices.GetService<IConfiguration>());
            var refreshToken = JwtHelper.GenerateRefreshToken();

            await _redis.SetAsync($"refresh_token:{refreshToken}", user.NguoiDungId.ToString(), TimeSpan.FromDays(7));

            return Json(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                data = new
                {
                    accessToken = accessToken,
                    refreshToken = refreshToken
                }
            });
        }

        [HttpPost("/cap-lai-access-token")]
        public async Task<IActionResult> Refresh([FromBody] Models.RefreshRequest request)
        {
            var NguoiDungId = await _redis.GetAsync($"refresh_token:{request.RefreshToken}");
            if (NguoiDungId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Refresh token không tồn tại hoặc đã hết hạn!"
                });
            }

            var ClaimAccessToken = SqlServer.ThongTinNguoiDung
                .FromSqlRaw("EXEC SP_LayThongTinNguoiDungChoAccessToken @NguoiDungId",
                    new SqlParameter("@NguoiDungId", NguoiDungId)
                )
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault();

            var newAccessToken = JwtHelper.GenerateAccessToken(
                ClaimAccessToken,
                HttpContext.RequestServices.GetService<IConfiguration>()
            );

            return Json(new
            {
                success = true,
                message = "Cấp lại access token thành công!",
                data = new
                {
                    accessToken = newAccessToken
                }
            });
        }


        [HttpPost("/logout")]
        public async Task<IActionResult> Logout([FromBody] Models.RefreshRequest request)
        {
            var key = $"refresh_token:{request.RefreshToken}";
            var DangXuat = await _redis.GetAsync(key);

            if (string.IsNullOrEmpty(DangXuat))
            {
                return Json(new
                {
                    success = false,
                    message = "Đã hết hạn"
                });
            }

            await _redis.RemoveAsync(key);

            return Json(new
            {
                success = true,
                message = "Đăng xuất thành công!"
            });
        }

        [HttpGet("/lay-thong-tin-nguoi-dung")]
        [Authorize]
        public async Task<IActionResult> LayThongTinNguoiDung()
        {
            var NguoiDungId = User.FindFirst("NguoiDungId")?.Value; 
            var TenNguoiDung = User.FindFirst("TenNguoiDung")?.Value;
            var Email = User.FindFirst("Email")?.Value;
            var VaiTro = User.FindFirst("VaiTro")?.Value;
            return Json(new
            {
                success = true,
                message = "Lấy thông tin người dùng thành công!",
                data = new
                {
                    nguoiDungId = NguoiDungId,
                    tenNguoiDung = TenNguoiDung,
                    email = Email,
                    vaiTro = VaiTro
                }
            });
        }
    }
}
