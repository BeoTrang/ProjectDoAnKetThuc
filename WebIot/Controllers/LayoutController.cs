using Microsoft.AspNetCore.Mvc;

namespace WebIot.Controllers
{
    public class LayoutController : Controller
    {
        [Route("/")]
        public IActionResult Layout()
        {
            return View();
        }
    }
}
