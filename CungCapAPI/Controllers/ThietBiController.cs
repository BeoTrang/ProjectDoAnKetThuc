using Azure;
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

        [HttpPost("/thiet-bi/dang-ky-thiet-bi")]
        public async Task<ActionResult> DangKyThietBi([FromBody] DangKyThietBi request)
        {
            try
            {
                bool check = await _thietBiService.CheckGhepNoiThietBiVoiTaiKhoan(request);
                if (!check)
                {
                    var response = new
                    {
                        success = false,
                        message = "Đăng ký không thành công!"
                    };
                    string json = JsonConvert.SerializeObject(response);
                    return Content(json, "application/json");
                }
                else
                {
                    int deviceId = await _thietBiService.DangKyThietBiMoi(request);
                    if (deviceId == 0)
                    {
                        var response = new
                        {
                            success = false,
                            message = "Lỗi hệ thống 1!"
                        };
                        string json = JsonConvert.SerializeObject(response);
                        return Content(json, "application/json");
                    }
                    else
                    {
                        var response = new
                        {
                            success = true,
                            message = "Đã đăng ký thành công!",
                            deviceId = deviceId.ToString()
                        };
                        string json = JsonConvert.SerializeObject(response);
                        return Content(json, "application/json");
                    }
                }
            }
            catch
            {
                var response = new
                {
                    success = false,
                    message = "Lỗi hệ thống 2!"
                };
                string json = JsonConvert.SerializeObject(response);
                return Content(json, "application/json");
            }
        }

        [Authorize]
        [HttpGet("/thiet-bi/lay-ma-them-thiet-bi-cua-nguoi-dung")]
        public async Task<ActionResult> LayMaThemThietBiCuaNguoiDung()
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string Ma = await _thietBiService.LayMaDangKyThietBi(NguoiDungId);
                var data = new ThemThietBi
                {
                    userId = NguoiDungId,
                    maThemThietBi = Ma
                };
                var response = new
                {
                    success = true,
                    message = "Oke!",
                    data = data
                };
                string json = JsonConvert.SerializeObject(response);
                return Content(json, "application/json");
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
        [HttpPost("/thiet-bi/tao-ma-them-thiet-bi")]
        public async Task<ActionResult> TaoMaThemThietBi([FromBody] KieuThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                bool KetQua = await _thietBiService.TaoMaThemThietBi(NguoiDungId, request.deviceType);
                if (!KetQua)
                {
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
                        success = true,
                        message = "Tạo mã thành công!"
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
        [HttpPost("/thiet-bi/huy-ma-them-thiet-bi")]
        public async Task<ActionResult> HuyMaThemThietBi()
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                bool KetQua = await _thietBiService.HuyMaThemThietBi(NguoiDungId);
                if (!KetQua)
                {
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
                        success = true,
                        message = "Hủy mã thành công!"
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
        [HttpPost("/thiet-bi/kiem-tra-quyen-thiet-bi")]
        public async Task<ActionResult> KiemTraQuyenThietBi([FromBody]KiemTraQuyenThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceId);
                if (Quyen == "full" || Quyen == "view"  || Quyen == "control" || QuyenAdmin == "Admin")
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
        [HttpGet("/thiet-bi/lay-danh-sach-thiet-bi")]
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
        [HttpPost("/thiet-bi/dieu-khien-thiet-bi")]
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
                    if (Quyen == "full" || Quyen == "control" || QuyenAdmin == "Admin")
                    {
                        var info = await _thietBiService.LayThongTinThietBi(request.deviceId);
                        string topic = "esp/" + info.type + "/" + request.deviceId + "/control";

                        var json = new
                        {
                            userId = NguoiDungId,
                            control = request.control,
                            state = request.state
                        };

                        string payload = JsonConvert.SerializeObject(json);

                        await _mqttService.PublishAsync(topic, payload);
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
        [HttpGet("/thiet-bi/lay-ten-thiet-bi/{deviceid}")]
        public async Task<ActionResult> LayTenThietBi(int deviceid)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceid);

                if (Quyen == "full" || Quyen == "control" || Quyen == "view" ||QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(deviceid);
                    switch (info.type)
                    {
                        case "AX01":
                            Name_AX01 name_AX01 = await _thietBiService.LayTenThietBi_AX01(deviceid);
                            return new JsonResult(new
                            {
                                success = true,
                                data = name_AX01,
                                message = "Ok",
                            });
                        case "AX02":
                            Name_AX02 name_AX02 = await _thietBiService.LayTenThietBi_AX02(deviceid);
                            return new JsonResult(new
                            {
                                success = true,
                                data = name_AX02,
                                message = "Ok",
                            });
                    }
                    if (info.type == "AX01")
                    {
                        
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
        [HttpPost("/thiet-bi/luu-ten-thiet-bi")]
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
        [Route("/thiet-bi/lich-su-thiet-bi")]
        public async Task<ActionResult> LichSuDuLieuThietBi([FromBody] HistorySearch model)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, model.deviceId);
                if (Quyen == "full" || Quyen == "view" || Quyen == "control" || QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(model.deviceId);
                    model.type = info.type;
                    var data = await _influxService.LichSuDuLieuThietBi(model);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Oke!",
                        data = data
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền!"
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
        [Route("/thiet-bi/thong-tin-thiet-bi/{deviceid}")]
        public async Task<ActionResult> ThongTinThietBi(int deviceid)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceid);
                if (Quyen == "full" || Quyen == "control" || Quyen == "view" || QuyenAdmin == "Admin")
                {
                    var info = await _thietBiService.LayThongTinThietBi(deviceid);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Thành công!",
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

        [Authorize]
        [HttpGet]
        [Route("/thiet-bi/lay-ma-chia-se-thiet-bi/{deviceid}")]
        public async Task<ActionResult> MaChiaSeThietBi(int deviceid)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceid);
                if (Quyen == "full" || QuyenAdmin == "Admin")
                {
                    JObject info = await _thietBiService.MaChiaSeThietBi(deviceid);

                    var response = new
                    {
                        success = true,
                        message = "Ok",
                        data = info
                    };

                    string json = JsonConvert.SerializeObject(response);

                    return Content(json, "application/json");
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền!"
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
        [Route("/thiet-bi/tao-ma-chia-se-thiet-bi")]
        public async Task<ActionResult> TaoMaChiaSeThietBi([FromBody] ShareRequest request)
        {
            try
            {
                if (request.deviceid == 0 || (request.quyen != "control" && request.quyen != "view"))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Sai cú pháp!"
                    });
                }
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceid);
                if (Quyen == "full" || QuyenAdmin == "Admin")
                {
                    var KetQua = await _thietBiService.TaoMaChiaSeThietBi(request.deviceid, request.quyen);
                    if (KetQua)
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Tạo mã thành công!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Tạo mã không thành công!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền!"
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
        [Route("/thiet-bi/xoa-ma-chia-se-thiet-bi")]
        public async Task<ActionResult> XoaMaChiaSeThietBi([FromBody] ShareRequest123 request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string QuyenAdmin = User.FindFirst("VaiTro").Value;
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceid);
                if (Quyen == "full" || QuyenAdmin == "Admin")
                {
                    var KetQua = await _thietBiService.XoaMaChiaSeThietBi(request.deviceid);
                    if (KetQua)
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Hủy chia sẻ thành công!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Hủy chia sẻ không thành công!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền!"
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
        [HttpPost("/thiet-bi/them-thiet-bi-chia-se")]
        public async Task<ActionResult> ThemThietBiChiaSe([FromBody] ShareDeviceRequest request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                bool KetQua = await _thietBiService.ThemThietBiChiaSe(NguoiDungId, request);
                if (KetQua)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Thêm thiết bị thành công!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Thêm thiết bị không thành công!"
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
        [HttpPost("thiet-bi/huy-theo-doi-thiet-bi")]
        public async Task<ActionResult> HuyTheoDoiThietBi([FromBody] KiemTraQuyenThietBi request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                bool KetQua = await _thietBiService.HuyTheoDoiThietBi(NguoiDungId, request.deviceId);
                if (KetQua)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Hủy theo dõi thành công!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Hủy theo dõi không thành công!"
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
        [HttpGet("/thiet-bi/device-info/{deviceId}")]
        public async Task<ActionResult> DeviceInfo(int deviceId)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, deviceId);
                if (Quyen == "full" || Quyen == "control" || Quyen == "view")
                {
                    var KetQua = await _thietBiService.LayDeviceInfo(deviceId);
                    if (KetQua == null)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Bạn không có quyền!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Oke!",
                            data = KetQua
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Bạn không có quyền!"
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
        [HttpPost("/thiet-bi/xoa-thiet-bi-boi-nguoi-dung")]
        public async Task<ActionResult> XoaThietBiBoiNguoiDung([FromBody] ShareRequest123 request)
        {
            try
            {
                int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
                string Quyen = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceid);
                if (Quyen == "full")
                {
                    bool KetQua = await _thietBiService.XoaThietBi(request.deviceid);
                    if (!KetQua)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Không biết nữa!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Xóa thiết bị thành công!"
                        });
                    }
                }
                else if (Quyen == "view" || Quyen == "control")
                {
                    var KetQua = await _thietBiService.HuyTheoDoiThietBi(NguoiDungId, request.deviceid);
                    if (!KetQua)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Không biết nữa!"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Hủy theo dõi thành công!"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Không biết nữa!"
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
