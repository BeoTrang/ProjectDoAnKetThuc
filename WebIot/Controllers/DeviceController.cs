using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public async Task<AX01<DHT22, Relay4, Name_AX01>> LayModelAX01(int deviceId)
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

            var AX01 = result.data?.ToObject<AX01<DHT22, Relay4, Name_AX01>>();

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

        [HttpGet]
        [Route("/thong-tin-thiet-bi/{deviceid}")]
        public async Task<ActionResult> SettingThietBi(int deviceid)
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();

            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(_apiSettings.Url + $"/ThietBi/lay-ten-thiet-bi/{deviceid}");
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<Name_AX01>>(responseBody);
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

            if (result.data.type == "AX01")
            {
                return PartialView("AX01_Settings", model);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/du-lieu-thiet-bi-moi-nhat/{type}/{deviceid}")]
        public async Task<ActionResult> LayDuLieuMoiNhat(string type, int deviceid)
        {
            try
            {
                KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
                if (!KiemTraDangNhap.success)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Hết hạn đăng nhập!"
                    });
                }
                if (type == "AX01")
                {
                    var model = await LayModelAX01(deviceid);
                    if (model == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Không có thiết bị nào!"
                        });
                    }
                    return Json(new
                    {
                        success = true,
                        message = "Oke!",
                        data = model
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Không có thiết bị nào!"
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi hệ thống!"
                });
            }
        }




        [HttpPost]
        [Route("/dieu-khien-thiet-bi")]
        public async Task<ActionResult> DieuKhienThietBi([FromBody] DieuKhienThietBi request)
        {
            if (request == null)
            {
                var responseError = new
                {
                    success = false,
                    message = "Lỗi hệ thống"
                };
                string jsonError = JsonConvert.SerializeObject(responseError);
                return Content(jsonError, "application/json");
            }
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success) return NotFound();
            var payload = new
            {
                deviceId = request.deviceId,
                payload = request.payload
            };
            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync(_apiSettings.Url + "/ThietBi/dieu-khien-thiet-bi", content);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<jwtTokens>>(responseBody);

            var responseClient = new
            {
                success = result.success,
                message = result.message
            };

            string json = JsonConvert.SerializeObject(responseClient);
            return Content(json, "application/json");
        }


        [HttpPost]
        [Route("/luu-ten-thiet-bi")]
        public async Task<ActionResult> LuuTenThietBi([FromBody] LuuTenThietBi request)
        {
            try
            {
                KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
                if (!KiemTraDangNhap.success) 
                    return Json(new
                    {
                        success = false,
                        message = "Đã hết hạn đăng nhập!"
                    });
                var accessToken = KiemTraDangNhap.accessToken;
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                if (request == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lỗi hệ thống!"
                    });
                }
                var payload = new
                {
                    deviceid = request.deviceid,
                    master = request.master,
                    nameConfig = request.nameConfig
                };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_apiSettings.Url + "/ThietBi/luu-ten-thiet-bi", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Lỗi hệ thống!" });

                var result = JsonConvert.DeserializeObject<Request<JObject>>(responseBody);

                return Json(new
                {
                    success = result.success,
                    message = result.message
                });

            }
            catch
            {
                return Json(new 
                { 
                    success = false, 
                    message = "Lỗi hệ thống!" 
                });
            }
        }

        [HttpPost]
        [Route("/lay-lich-su-du-lieu-thiet-bi")]
        public async Task<ActionResult> LayLichSuDuLieuThietBi([FromBody] HistorySearch request)
        {
            try
            {
                if (request == null)
                {
                    var responseError = new
                    {
                        success = false,
                        message = "Lỗi hệ thống"
                    };
                    string jsonError = JsonConvert.SerializeObject(responseError);
                    return Content(jsonError, "application/json");
                }
                KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
                if (!KiemTraDangNhap.success)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Đã hết hạn đăng nhập!"
                    });
                }

                var payload = new
                {
                    deviceId = request.deviceId,
                    typePick = request.typePick,
                    pickTime= request.pickTime,
                    startUTC = request.startUTC,
                    endUTC = request.endUTC
                };
                var accessToken = KiemTraDangNhap.accessToken;
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);


                var content = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                var response = await client.PostAsync(_apiSettings.Url + "/Thietbi/lich-su-thiet-bi", content);
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Lỗi hệ thống!" });

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<Request<List<object>>>(responseBody);

                var responseClient = new
                {
                    success = result.success,
                    message = result.message,
                    data = result.data
                };

                string json = JsonConvert.SerializeObject(responseClient);
                return Content(json, "application/json");
            }
            catch
            {
                var responseClient = new
                {
                    success = false,
                    message = "Lỗi hệ thống!"
                };

                string json = JsonConvert.SerializeObject(responseClient);
                return Content(json, "application/json");
            }
        }

        [HttpGet]
        [Route("/device-history/{deviceid}")]
        public async Task<ActionResult> ViewLichSuThietBi(int deviceid)
        {
            KiemTraJWT KiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!KiemTraDangNhap.success)
                return Json(new
                {
                    success = false,
                    message = "Đã hết hạn đăng nhập!"
                });
            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(_apiSettings.Url + $"/ThietBi/thong-tin-thiet-bi/{deviceid}");
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<Device>>(responseBody);
            

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
                var model = result.data;
                return PartialView("_HistoryDevice", model);
            }
        }
    }
}
