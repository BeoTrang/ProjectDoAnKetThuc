using Microsoft.AspNetCore.Mvc;

namespace WebIot.Controllers
{
    public class DangNhapController : Controller
    {
        public IActionResult DangNhap()
        {
            return View();
        }
    }
}
