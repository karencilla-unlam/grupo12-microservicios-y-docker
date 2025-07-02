using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TelegramBot.Logica.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace TelegramBot.Web.Controllers
{
    public class TelegramController : Controller
    {
        private readonly TelegramBotMicroservicioClient _botMicroservicio;
        private readonly IServicioPreguntas _servicioPreguntas;
        private readonly ILogger<TelegramController> _logger;

        public TelegramController(
            IServicioPreguntas servicioPreguntas,
            TelegramBotMicroservicioClient botMicroservicio,
            ILogger<TelegramController> logger)
        {
            _botMicroservicio = botMicroservicio;
            _servicioPreguntas = servicioPreguntas;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            using var lector = new System.IO.StreamReader(Request.Body);
            var json = await lector.ReadToEndAsync();

            var jToken = JToken.Parse(json);

            var messageDate = jToken["message"]?["date"];
            if (messageDate?.Type == JTokenType.Integer)
            {
                long timestamp = messageDate.Value<long>();
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
                jToken["message"]["date"] = dateTime;
            }
            _logger.LogInformation("JSON recibido: [{Json}]", json);

            var update = jToken.ToObject<Update>();

            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                var chatId = update.Message.Chat.Id;
                var textoRecibido = update.Message.Text;
                _logger.LogInformation(textoRecibido);

                var respuesta = await _servicioPreguntas.ObtenerRespuestaAsync(textoRecibido);

                // Usar el microservicio para enviar el mensaje
                await _botMicroservicio.EnviarMensajeAsync(chatId, respuesta);
            }

            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}