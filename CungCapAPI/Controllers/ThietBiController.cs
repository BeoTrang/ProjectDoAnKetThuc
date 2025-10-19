using CungCapAPI.Application.Interfaces;
using CungCapAPI.Application.Services;
using CungCapAPI.MQTT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CungCapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThietBiController : Controller
    {
        private readonly IThietBiService _thietBiService;
        private readonly MqttService _mqttService;
        public ThietBiController(IThietBiService thietBiService, MqttService mqttService)
        {
            _thietBiService = thietBiService;
            _mqttService = mqttService;
        }

        [Authorize]
        [HttpPost("kiem-tra-quyen-thiet-bi")]
        public async Task<ActionResult> KiemTraQuyenThietBi([FromBody]KiemTraQuyenThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceId);
                if (Quyen == "full" || Quyen == "view" || QuyenAdmin == "Admin")
                {
                    JObject data = await _thietBiService.LayDuLieuThietBi(request.deviceId);
                    var response = new
                    {
                        success = true,
                        message = "Ok",
                        data = data
                    };

                    string json = JsonConvert.SerializeObject(response);

                    return Content(json, "application/json");
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Ko"
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Lỗi hệ thống!"
                });
            }
        }
        [Authorize]
        [HttpGet("lay-danh-sach-thiet-bi")]
        public async Task<ActionResult> LayDanhSachThietBi()
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                List<DanhSachThietBi> result = await _thietBiService.LayDanhSachThietBi(NguoiDungId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Ok",
                    data = result
                });
            }
            catch
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Lỗi hệ thống!"
                });
            }
        }
        [Authorize]
        [HttpPost("dieu-khien-thiet-bi")]
        public async Task<ActionResult> DieuKhienThietBi([FromBody] DieuKhienThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceId);

                bool status = await _thietBiService.TrangThaiThietBi(request.deviceId);

                if (status == true)
                {
                    if (Quyen == "full" || QuyenAdmin == "Admin")
                    {
                        var info = await _thietBiService.LayThongTinThietBi(request.deviceId);
                        string topic = "esp/" + info.type + "/" + request.deviceId + "/control";
                        await _mqttService.PublishAsync(topic, request.payload);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Ok",
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Bạn không có quyền điều khiển thiết bị!",
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Không điều khiển được thiết bị offline!",
                    });
                }
                
            }
            catch
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Lỗi hệ thống!",
                });
            }
        }
    }
}
