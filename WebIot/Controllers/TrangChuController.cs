using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
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
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(_apiSettings.Url + "/TaiKhoan/thong-tin-nguoi-dung");
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<Request<ThongTinNguoiDung>>(responseBody);
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
        [Route("/dashboard")]
        public async Task<IActionResult> _Dashboard()
        {
            bool KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap)
            {
                return Redirect("/dang-nhap");
            }
            else
            {
                var accessToken = Request.Cookies["accessToken"];

                return View();
            }
        }
        [Route("/lay-url-api")]
        public async Task<IActionResult> LayUrlSignalR()
        {
            return Json(new
            {
                success = true,
                data = _apiSettings.Url
            });
        }

        

    }
}
