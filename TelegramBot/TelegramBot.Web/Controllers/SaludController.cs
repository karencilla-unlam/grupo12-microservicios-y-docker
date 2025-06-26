using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Web.Controllers
{
    public class SaludController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
