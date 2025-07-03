using Microsoft.AspNetCore.Mvc;

namespace TelegramBot.Web.Controllers
{
    public class SaludController : Controller
    {
        private readonly ILogger<SaludController> _logger;

        public SaludController(ILogger<SaludController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Entrando al controlador Salud");
            return View();
        }
    }
}
