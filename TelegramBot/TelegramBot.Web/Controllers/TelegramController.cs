using Microsoft.AspNetCore.Mvc;
using TelegramBot.Logica.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Logica.Interfaces;

namespace TelegramBot.Web.Controllers
{
    public class TelegramController : Controller
    {
        private readonly ITelegramBotClient _bot;
        private readonly IServicioPreguntas _servicioPreguntas;

        public TelegramController(IServicioPreguntas servicioPreguntas)
        {
            // Iniciás el bot con el token
            _bot = new TelegramBotClient("TOKEN_BOT");
            _servicioPreguntas = servicioPreguntas;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            using var lector = new System.IO.StreamReader(Request.Body);
            var json = await lector.ReadToEndAsync();
            var update = Newtonsoft.Json.JsonConvert.DeserializeObject<Update>(json);

            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var textoRecibido = update.Message.Text;

                // Obtener respuesta (clima o BD)
                var respuesta = await _servicioPreguntas.ObtenerRespuestaAsync(textoRecibido);

                // Enviar respuesta al usuario
                await _bot.SendTextMessageAsync(chatId, respuesta);
            }

            return Ok();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
