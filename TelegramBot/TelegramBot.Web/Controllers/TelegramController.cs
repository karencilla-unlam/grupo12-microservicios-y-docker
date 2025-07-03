using Microsoft.AspNetCore.Mvc;
using TelegramBot.Logica.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;

namespace TelegramBot.Web.Controllers
{
    public class TelegramController : Controller
    {
        private readonly IServicioTelegramBotClient _bot;
        private readonly IServicioPreguntas _servicioPreguntas;
        private readonly ILogger<TelegramController> _logger;

        public TelegramController(IServicioPreguntas servicioPreguntas, IConfiguration configuration, ILogger<TelegramController> logger)
        {
            _bot = new ServicioTelegramBotClient(configuration);
            _servicioPreguntas = servicioPreguntas;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            using var lector = new System.IO.StreamReader(Request.Body);
            var json = await lector.ReadToEndAsync();

            // ✅ Interceptar y transformar "date"
            var jToken = JToken.Parse(json);

            var messageDate = jToken["message"]?["date"];
            if (messageDate?.Type == JTokenType.Integer)
            {
                long timestamp = messageDate.Value<long>();
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
                jToken["message"]["date"] = dateTime;
            }
            _logger.LogInformation("JSON recibido: [{Json}]", json);
            // ✅ Ahora sí deserializar sin error
            var update = jToken.ToObject<Update>();

            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var textoRecibido = update.Message.Text;
                _logger.LogInformation(textoRecibido);


                // Obtener respuesta (clima o BD)
                var respuesta = await _servicioPreguntas.ObtenerRespuestaAsync(textoRecibido);

                // Enviar respuesta al usuario
                await _bot.EnviarMensajeTextoAsync(chatId, respuesta);
            }

            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}