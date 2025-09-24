using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using WebIot.Helper;
using WebIot.Models;

namespace WebIot.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JWT_Helper _jWT_Helper;
        public TaiKhoanController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, JWT_Helper jWT_Helper)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _jWT_Helper = jWT_Helper;
        }

        [Route("/dang-nhap")]
        public async Task<IActionResult> DangNhap()
        {
            bool KetQua = await _jWT_Helper.KiemTraDangNhap();
            if (KetQua)
            {
                return Redirect("/trang-chu");
            }
            else
            {
                return View();
            }
        }

        [Route("/dang-ky")]
        public ActionResult DangKy()
        {
            return View();
        }

        [Route("/dang-ky-tai-khoan")]
        public async Task<IActionResult> DangKyTaiKhoan([FromBody] TaiKhoanDangKy request)
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                name = request.name,
                email = request.email,
                phone_number = request.phone_number,
                account_login = request.account_login,
                password_login = request.password_login
            };
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiSettings.Url + "/TaiKhoan/dang-ky-tai-khoan", content);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin." });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<JWT>>(responseBody);

            if (result.success == true)
            {
                return Json(new
                {
                    success = true,
                    message = result.message
                });
            }
            else
            {
                return Json(new 
                { 
                    success = false, 
                    message = result.message 
                });
            }
        }

        [Route("/kiem-tra-dang-nhap")]
        public async Task<IActionResult> KiemTraDangNhap([FromBody] TaiKhoanGuiVe request)
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                taiKhoan = request.TaiKhoan,
                matKhau = request.MatKhau
            };
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiSettings.Url + "/TaiKhoan/dang-nhap", content);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin." });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<JWT>>(responseBody);
            
            if (result.success == true)
            {
                Response.Cookies.Append("accessToken", result.data.accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });

                Response.Cookies.Append("refreshToken", result.data.refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Json(new
                {
                    success = true,
                    message = "Đăng nhập thành công!"
                });
            }
            else
            {
                return Json(new { success = false, message = result.message });
            }  
        }
        
        [Route("/duy-tri-dang-nhap")]
        public async Task<IActionResult> CaplaiRefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Json(new { success = false, message = "Đã hết hạn, yêu cầu đăng nhập lại!" });
            }
            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                refreshToken = refreshToken
            };
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiSettings.Url + "/TaiKhoan/cap-lai-access-token", content);
            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Lỗi!" });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<JWT>>(responseBody);
            if (result.success == false || result.success == null)
            {
                return Json(new { success = false, message = result.message });
            }
            else
            {
                Response.Cookies.Append("accessToken", result.data.accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });
                return Json(new
                {
                    success = true,
                    message = "Đã cấp lại Access Token!"
                });
            }
        }
    }
}
