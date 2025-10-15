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
    public class DeviceController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JWT_Helper _jWT_Helper;
        public DeviceController(IHttpClientFactory httpClientFactory, IOptions<Models.ApiSettings> apiSettings, JWT_Helper jWT_Helper)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _jWT_Helper = jWT_Helper;
        }
        [HttpGet]
        [Route("/lay-danh-sach-thiet-bi")]
        public async Task<IActionResult> LayDanhSachThietBi()
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success)
            {
                return Json(new
                {
                    success = false,
                    message = "Hết hạn đăng nhập"
                });
            }
            else
            {
                var accessToken = KiemTraDangNhap.accessToken;
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(_apiSettings.Url + "/ThietBi/lay-danh-sach-thiet-bi");
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Lỗi hệ thống!" });

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Request<List<DanhSachThietBi>>>(responseBody);
                return new JsonResult(new
                {
                    success = result.success,
                    message = result.message,
                    data = result.data
                });                
            }


        }


        [HttpPost]
        [Route("/lay-du-lieu-thiet-bi")]
        public async Task<IActionResult> LayDuLieuThietBi([FromBody] KiemTraQuyenThietBi request)
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
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
        public async Task<AX01<DHT22, Relay4>> LayModelAX01(int deviceId)
        {
            var accessToken = Request.Cookies["accessToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new 
            { 
                deviceId = deviceId 
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(_apiSettings.Url + "/ThietBi/kiem-tra-quyen-thiet-bi", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<JObject>>(responseBody);

             var AX01 = result.data?.ToObject<AX01<DHT22, Relay4>>();

            return AX01;
        }

        [HttpPost]
        [Route("/view-cho-thiet-bi")]
        public async Task<ActionResult> LayViewThietBi([FromBody] LayViewThietBi request)
        {
            if (request.deviceType == "AX01")
            {
                var model = await LayModelAX01(request.deviceId);
                return PartialView("AX01", model);
            }
            return BadRequest("Loại thiết bị không hợp lệ");
        }
    }
}
