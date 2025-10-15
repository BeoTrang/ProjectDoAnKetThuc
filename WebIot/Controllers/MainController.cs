using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelLibrary;
using WebIot.Helper;
using WebIot.Models;

namespace WebIot.Controllers
{
    public class MainController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JWT_Helper _jWT_Helper;
        public MainController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, JWT_Helper jWT_Helper)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _jWT_Helper = jWT_Helper;
        }

        [Route("/")]
        public async Task<IActionResult> Main()
        {
            KiemTraJWT KetQua = await _jWT_Helper.KiemTraDangNhap();
            if (KetQua.success)
            {
                return Redirect("/trang-chu");
            }
            else
            {
                return Redirect("/dang-nhap");
            }
        }
    }
}
