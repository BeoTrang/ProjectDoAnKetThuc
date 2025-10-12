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

        [HttpPost]
        [Route("/lay-du-lieu-thiet-bi")]
        public async Task<IActionResult> LayDuLieuThietBi([FromBody] KiemTraQuyenThietBi request)
        {
            bool KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap) return NotFound();

            var accessToken = Request.Cookies["accessToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Gọi API check quyền
            var payload = new { deviceId = request.deviceId };
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_apiSettings.Url + "/ThietBi/kiem-tra-quyen-thiet-bi", content);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

            var responseBody = await response.Content.ReadAsStringAsync();

            // Deserialize thành JObject dynamic
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Request<JObject>>(responseBody);
            
            var responseClient = new
            {
                success = result.success,
                message = result.message,
                data = result.data
            };

            string json = JsonConvert.SerializeObject(responseClient);
            return Content(json, "application/json");
        }

    }
}
