using Azure.Core;
using CungCapAPI.Application.Interfaces;
using CungCapAPI.Application.Services;
using CungCapAPI.MQTT;
using CungCapAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CungCapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThietBiController : Controller
    {
        private readonly IThietBiService _thietBiService;
        private readonly MqttService _mqttService;
        private readonly InfluxService _influxService;
        public ThietBiController(IThietBiService thietBiService, MqttService mqttService, InfluxService influxService)
        {
            _thietBiService = thietBiService;
            _mqttService = mqttService;
            _influxService = influxService;
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
        [Authorize]
        [HttpGet("lay-ten-thiet-bi/{deviceid}")]
        public async Task<ActionResult> LayTenThietBi(int deviceid)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceid);

                if (Quyen == "full" || QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(deviceid);
                    if (info.type == "AX01")
                    {
                        Name_AX01 name = await _thietBiService.LayTenThietBi(deviceid);


                        return new JsonResult(new
                        {
                            success = true,
                            data = name,
                            message = "Ok",
                        });
                    }
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Lỗi hệ thống!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền cài đặt thiết bị!",
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
        [Authorize]
        [HttpPost("luu-ten-thiet-bi")]
        public async Task<ActionResult> LuuTenThietBi([FromBody] LuuTenThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceid);
                if (Quyen == "full" || QuyenAdmin == "Admin")
                {
                    var KetQua = await _thietBiService.LuuTenThietBi(request);
                    if (KetQua)
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Chỉnh sửa thành công!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Lỗi hệ thống!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền chỉnh sửa!"
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
        [HttpPost]
        [Route("lich-su-thiet-bi")]
        public async Task<ActionResult> LichSuDuLieuThietBi([FromBody] HistorySearch model)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, model.deviceId);
                if (Quyen == "full" || Quyen == "view" || QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(model.deviceId);
                    model.type = info.type;
                    var data = await _influxService.LichSuDuLieuThietBi(model);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Đăng xuất thành công!",
                        data = data
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền chỉnh sửa!"
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
        [HttpGet]
        [Route("thong-tin-thiet-bi/{deviceid}")]
        public async Task<ActionResult> ThongTinThietBi(int deviceid)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceid);
                if (Quyen == "full" || Quyen =="view" || QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(deviceid);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Đăng xuất thành công!",
                        data = info
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền chỉnh sửa!"
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
    }
}
