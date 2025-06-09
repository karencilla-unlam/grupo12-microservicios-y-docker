using System.Threading.Tasks;
using System.Web.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;

public class TelegramController : Controller
{
    [HttpPost]
    public async Task<ActionResult> Webhook()
    {
        using (var lector = new System.IO.StreamReader(Request.InputStream))
        {
            var json = await lector.ReadToEndAsync();
            var actualizacion = JsonConvert.DeserializeObject<Update>(json);

            if (actualizacion.Type == UpdateType.Message && actualizacion.Message.Text != null)
            {
                var bot = new TelegramBotClient("TOKEN_BOT");

                var idChat = actualizacion.Message.Chat.Id;
                var textoMensaje = actualizacion.Message.Text;

                await bot.SendTextMessageAsync(
                    chatId: idChat,
                    text: $"Recibí tu mensaje: {textoMensaje}"
                );
            }
        }

        return new HttpStatusCodeResult(200);
    }
}
