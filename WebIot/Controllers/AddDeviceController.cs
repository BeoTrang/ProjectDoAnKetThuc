using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebIot.Helper;
using WebIot.Models;

namespace WebIot.Controllers
{
    public class AddDeviceController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JWT_Helper _jWT_Helper;
        public AddDeviceController(IHttpClientFactory httpClientFactory, IOptions<Models.ApiSettings> apiSettings, JWT_Helper jWT_Helper)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _jWT_Helper = jWT_Helper;
        }

        [HttpGet("/view-trang-them-thiet-bi")]
        public IActionResult TrangAddDevice()
        {
            return PartialView("TrangAddDevice");
        }

        [HttpGet("/view-them-thiet-bi-theo-kieu-thiet-bi-moi")]
        public async Task<ActionResult> ViewNewDevice()
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(_apiSettings.Url + $"/thiet-bi/lay-ma-them-thiet-bi-cua-nguoi-dung");
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<ThemThietBi>>(responseBody);

            var model = result.data;

            if (!result.success)
            {
                var responseClient = new
                {
                    success = result.success,
                    message = result.message
                };

                string json = JsonConvert.SerializeObject(responseClient);
                return Content(json, "application/json");
            }
            else
            {
                return PartialView("ViewNewDevice", model);
            }
        }

        [HttpGet("/view-them-thiet-bi-theo-kieu-chia-se")]
        public async Task<ActionResult> ViewShareDevice()
        {
            return PartialView("ViewShareDevice");
        }

        [HttpPost("/api/tao-ma-them-thiet-bi")]
        public async Task<ActionResult> TaoMaThemThietBi([FromBody] KieuThietBi request)
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new { deviceType = request.deviceType };
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/tao-ma-them-thiet-bi", content);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Request<KieuThietBi>>(responseBody);

            var responseClient = new
            {
                success = result.success,
                message = result.message
            };

            string json = JsonConvert.SerializeObject(responseClient);
            return Content(json, "application/json");
        }

        [HttpPost("/api/huy-ma-them-thiet-bi")]
        public async Task<ActionResult> HuyMaThemThietBi()
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);


            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/huy-ma-them-thiet-bi", content: null);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Request<KieuThietBi>>(responseBody);

            var responseClient = new
            {
                success = result.success,
                message = result.message
            };

            string json = JsonConvert.SerializeObject(responseClient);
            return Content(json, "application/json");
        }

        [HttpPost("/api/them-thiet-bi-chia-se")]
        public async Task<ActionResult> ThemThietBiChiSe([FromBody] ShareDeviceRequest request)
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new { maThietBi = request.maThietBi };
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/them-thiet-bi-chia-se", content);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Request<KieuThietBi>>(responseBody);

            var responseClient = new
            {
                success = result.success,
                message = result.message
            };

            string json = JsonConvert.SerializeObject(responseClient);
            return Content(json, "application/json");
        }
    }
}
