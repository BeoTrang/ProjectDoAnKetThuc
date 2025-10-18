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
        [Authorize]
        [HttpGet("lay-danh-sach-thiet-bi")]
        public async Task<ActionResult> LayDanhSachThietBi()
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
        [Authorize]
        [HttpPost("dieu-khien-thiet-bi")]
        public async Task<ActionResult> DieuKhienThietBi([FromBody] DieuKhienThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceId);
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
