using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
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
        [Route("/api/lay-danh-sach-thiet-bi")]
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
                var response = await client.GetAsync(_apiSettings.Url + "/thiet-bi/lay-danh-sach-thiet-bi");
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
        [Route("/api/lay-du-lieu-thiet-bi")]
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

            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/kiem-tra-quyen-thiet-bi", content);
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

            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/kiem-tra-quyen-thiet-bi", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<JObject>>(responseBody);

            var AX01 = result.data?.ToObject<AX01<DHT22, Relay4, Name_AX01>>();

            return AX01;
        }

        public async Task<AX02<DHT22, Name_AX02>> LayModelAX02(int deviceId)
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

            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/kiem-tra-quyen-thiet-bi", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<JObject>>(responseBody);

            var AX02 = result.data?.ToObject<AX02<DHT22, Name_AX02>>();

            return AX02;
        }


        [HttpPost]
        [Route("/view-cho-thiet-bi")]
        public async Task<ActionResult> LayViewThietBi([FromBody] LayViewThietBi request)
        {

            switch (request.deviceType)
            {
                case "AX01":
                    var model_AX01 = await LayModelAX01(request.deviceId);
                    return PartialView("AX01", model_AX01);
                case "AX02":
                    var model_AX02 = await LayModelAX02(request.deviceId);
                    return PartialView("AX02", model_AX02);

                default:
                    return BadRequest("Loại thiết bị không hợp lệ");

            } 
        }





        [HttpGet]
        [Route("/thong-tin-thiet-bi/{deviceid}")]
        public async Task<ActionResult> SettingThietBi(int deviceid)
        {
            var kiemTraDangNhap = await _jWT_Helper.KiemTraDangNhap();
            if (!kiemTraDangNhap.success)
                return Unauthorized();

            var accessToken = kiemTraDangNhap.accessToken;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync($"{_apiSettings.Url}/thiet-bi/lay-ten-thiet-bi/{deviceid}");
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Request<JObject>>(responseBody);

            if (!result.success)
            {
                return Json(new
                {
                    success = result.success,
                    message = result.message
                });
            }

            var data = result.data;
            string type = data.Value<string>("type");

            switch (type)
            {
                case "AX01":
                    {
                        var model = data.ToObject<ModelLibrary.Name_AX01>();
                        return PartialView("AX01_Settings", model);
                    }

                case "AX02":
                    {
                        var model = data.ToObject<ModelLibrary.Name_AX02>();
                        return PartialView("AX02_Settings", model);
                    }

                default:
                    return NotFound($"Không có giao diện cho loại thiết bị: {type}");
            }
        }



        [HttpGet]
        [Route("/api/du-lieu-thiet-bi-moi-nhat/{type}/{deviceid}")]
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
                switch (type)
                {
                    case "AX01":
                        var model_AX01 = await LayModelAX01(deviceid);
                        if (model_AX01 == null)
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
                            data = model_AX01
                        });
                    case "AX02":
                        var model_AX02 = await LayModelAX02(deviceid);
                        if (model_AX02 == null)
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
                            data = model_AX02
                        });
                    default:
                        return Json(new
                        {
                            success = false,
                            message = "Không có thiết bị nào!"
                        });
                }
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
        [Route("/api/dieu-khien-thiet-bi")]
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
            var accessToken = KiemTraDangNhap.accessToken;
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            
            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/dieu-khien-thiet-bi", content);
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
        [Route("/api/luu-ten-thiet-bi")]
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
                    nameConfig = string.IsNullOrEmpty(request.nameConfig) ? "" : request.nameConfig
                };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/luu-ten-thiet-bi", content);
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
        [Route("/api/lay-lich-su-du-lieu-thiet-bi")]
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
                var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/lich-su-thiet-bi", content);
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

            var response = await client.GetAsync(_apiSettings.Url + $"/thiet-bi/thong-tin-thiet-bi/{deviceid}");
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });

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

        [HttpGet]
        [Route("/chia-se-thiet-bi/{deviceid}")]
        public async Task<ActionResult> LayMaChiaSeThietBi(int deviceid)
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

            var response = await client.GetAsync(_apiSettings.Url + $"/thiet-bi/lay-ma-chia-se-thiet-bi/{deviceid}");
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Lỗi hệ thống!" });
            Console.WriteLine(responseBody);
            var result = JsonConvert.DeserializeObject<Request<ShareDeviceModel>>(responseBody);


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
                var viewModel = new Models.ShareDevice
                {
                    deivceId = deviceid,
                    share = result.data
                };

                return PartialView("ShareDevice", viewModel);
            }
        }

        [HttpPost]
        [Route("/api/tao-ma-chia-se-thiet-bi")]
        public async Task<ActionResult> TaoMaChiaSeThietBi([FromBody] ShareRequest request)
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
                if (request.deviceid == 0 || (request.quyen != "control" && request.quyen != "view"))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lỗi cú pháp!"
                    });
                }
                var payload = new
                {
                    deviceid = request.deviceid,
                    quyen = request.quyen
                };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/tao-ma-chia-se-thiet-bi", content);
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
        [Route("/api/xoa-ma-chia-se-thiet-bi")]
        public async Task<ActionResult> XoaMaChiaSeThietBi([FromBody] ShareRequest123 request)
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
                if (request.deviceid == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lỗi cú pháp!"
                    });
                }
                var payload = new
                {
                    deviceid = request.deviceid
                };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/xoa-ma-chia-se-thiet-bi", content);
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

        [HttpGet]
        [Route("/trang-setting")]
        public async Task<ActionResult> TrangSetting()
        {
            return  PartialView("DeviceSetting");
        }

        [HttpGet]
        [Route("/view-device-info")]
        public async Task<ActionResult> ViewDeviceInfo()
        {
             return PartialView("DeviceInfo");
        }

        [HttpGet]
        [Route("/api/device-info/{deviceId}")]
        public async Task<ActionResult> DeviceInfo(int deviceId)
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

                var response = await client.GetAsync(_apiSettings.Url + $"/thiet-bi/device-info/{deviceId}");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Lỗi hệ thống!" });
                Console.WriteLine(responseBody);
                var result = JsonConvert.DeserializeObject<Request<DeviceInfo>>(responseBody);


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
                    return new JsonResult(new
                    {
                        success = result.success,
                        message = result.message,
                        data = result.data
                    });
                }
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
        [Route("/api/xoa-thiet-bi-boi-nguoi-dung")]
        public async Task<ActionResult> XoaThietBiBoiNguoiDung([FromBody] ShareRequest123 request)
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
                if (request.deviceid == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Lỗi cú pháp!"
                    });
                }
                var payload = new
                {
                    deviceid = request.deviceid
                };
                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(_apiSettings.Url + "/thiet-bi/xoa-thiet-bi-boi-nguoi-dung", content);
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

    }
}
