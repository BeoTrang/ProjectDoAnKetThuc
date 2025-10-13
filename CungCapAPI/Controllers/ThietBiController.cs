using CungCapAPI.Application.Interfaces;
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
        public ThietBiController(IThietBiService thietBiService)
        {
            _thietBiService = thietBiService;
        }

        [Authorize]
        [HttpPost("kiem-tra-quyen-thiet-bi")]
        public async Task<ActionResult> KiemTraQuyenThietBi([FromBody]KiemTraQuyenThietBi request)
        {
            int NguoiDungId = int.Parse(User.FindFirst("NguoiDungId").Value);
            bool KetQua = await _thietBiService.KiemTraQuyenThietBi(NguoiDungId, request.deviceId);
            if (KetQua == true)
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
    }
}
