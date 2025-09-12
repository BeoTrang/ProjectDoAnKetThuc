using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebIot.Models;

namespace WebIot.Controllers
{
    public class MainController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public MainController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        [Route("/")]
        public IActionResult Main()
        {
            var accessToken = Request.Cookies["accessToken"];
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            else if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("CapLaiRefreshToken", "TaiKhoan");
            }
            else
            {
                return Redirect("/trang-chu");
            }
        }
    }
}
