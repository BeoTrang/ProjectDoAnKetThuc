using CungCapAPI.Helpers;
using CungCapAPI.Models;
using CungCapAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CungCapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DangNhapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRedisService _redis;
        public DangNhapController(ApplicationDbContext context, IRedisService redis)
        {
            _context = context;
            _redis = redis;
        }

        
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] Models.TaiKhoanGuiVe request)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TaiKhoan == request.TaiKhoan);
            if (user == null)
            {
                return Unauthorized("Nhập sai tài khoản hoặc mật khẩu!");
            }
            if (!PasswordHelper.KiemTraMatKhau(request.MatKhau, user.MatKhauMaHoa, user.MatKhauMuoi))
            {
                return Unauthorized("Nhập sai tài khoản hoặc mật khẩu");
            }

            var ClaimAccessToken = _context.ThongTinNguoiDung
                .FromSqlRaw("EXEC SP_LayThongTinNguoiDungChoAccessToken @NguoiDungId",
                    new SqlParameter("@NguoiDungId", user.NguoiDungId)
                )
                .AsNoTracking()
                .AsEnumerable() 
                .FirstOrDefault();


            var accessToken = JwtHelper.GenerateAccessToken(ClaimAccessToken, HttpContext.RequestServices.GetService<IConfiguration>());
            var refreshToken = JwtHelper.GenerateRefreshToken();

            await _redis.SetAsync($"refresh_token:{refreshToken}", user.NguoiDungId.ToString(), TimeSpan.FromDays(7));

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("/refresh")]
        public async Task<IActionResult> Refresh([FromBody] Models.RefreshRequest request)
        {
            var NguoiDungId = await _redis.GetAsync($"refresh_token:{request.RefreshToken}");
            if (NguoiDungId == null)
            {
                return Unauthorized();
            }

            var ClaimAccessToken = _context.ThongTinNguoiDung
                .FromSqlRaw("EXEC SP_LayThongTinNguoiDungChoAccessToken @NguoiDungId",
                    new SqlParameter("@NguoiDungId", NguoiDungId)
                )
                .AsNoTracking()
                .AsEnumerable()  
                .FirstOrDefault();

            var newAccessToken = JwtHelper.GenerateAccessToken(ClaimAccessToken, HttpContext.RequestServices.GetService<IConfiguration>());
            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout([FromBody] Models.RefreshRequest request)
        {
            var key = $"refresh_token:{request.RefreshToken}";
            var DangXuat = await _redis.GetAsync(key);

            if (string.IsNullOrEmpty(DangXuat))
            {
                return NotFound("Refresh token không tồn tại hoặc đã hết hạn!");
            }

            await _redis.RemoveAsync(key);

            return Ok(new
            {
                Message = "Đăng xuất thành công!"
            });
        }

        [HttpGet("/test-auth")]
        [Authorize]
        public async Task<IActionResult> LayDuLieu()
        {
            var userId = User.FindFirst("NguoiDungId")?.Value; 
            return Ok(new
            {
                Message = $"Oke, UserId = {userId}"
            });
        }
    }
}
