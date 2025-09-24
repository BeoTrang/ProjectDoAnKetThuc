using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebIot.Helper;
using WebIot.Models;

namespace WebIot.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JWT_Helper _jWT_Helper;

        public TrangChuController(IHttpClientFactory httpClientFactory, IOptions<Models.ApiSettings> apiSettings, JWT_Helper jWT_Helper)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _jWT_Helper = jWT_Helper;
        }

        [Route("/trang-chu")]
        public async Task<IActionResult> TrangChu()
        {
            bool KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap)
            {
                return Redirect("/dang-nhap");
            }
            else
            {
                var accessToken = Request.Cookies["accessToken"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    var refreshToken = Request.Cookies["refreshToken"];
                    var capLaiAccessToken = _httpClientFactory.CreateClient();
                    var payload = new
                    {
                        refreshToken = refreshToken
                    };
                    var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new System.Net.Http.StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");


                    var response1 = await capLaiAccessToken.PostAsync(_apiSettings.Url + "/TaiKhoan/cap-lai-access-token", content);
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(_apiSettings.Url + "/TaiKhoan/thong-tin-nguoi-dung");
                if (!response.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("accessToken");
                    Response.Cookies.Delete("refreshToken");
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<PhanHoiApi<ThongTinNguoiDung>>(responseBody);
                if (result.success == true)
                {
                    var ThongTinNguoiDung = result.data;
                }
                var viewModel = new TrangChu
                {
                    thongTinNguoiDung = result.data
                };
                return View(viewModel);
            }
        }
    }
}
