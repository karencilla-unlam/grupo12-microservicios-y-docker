/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot.Logica.Interfaces;

namespace TelegramBot.Logica.Servicios
{
    public class ServicioTelegramBotClient : IServicioTelegramBotClient
    {
        private readonly TelegramBotClient _clienteTelegram;

        public ServicioTelegramBotClient(string token)
        {
            _clienteTelegram = new TelegramBotClient(token);
        }

        public async Task EnviarMensajeTextoAsync(long chatId, string texto)
        {
            await _clienteTelegram.SendTextMessageAsync(chatId, texto);
        }

    }

}*/
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Logica.Interfaces;

public class ServicioTelegramBotClient : IServicioTelegramBotClient
{
    private readonly ITelegramBotClient _clienteTelegram;

    public ServicioTelegramBotClient(IConfiguration configuration)
    {
        var token = configuration["TelegramBot:Token"];
        _clienteTelegram = new TelegramBotClient(token);
    }

    public async Task EnviarMensajeTextoAsync(long chatId, string texto)
    {
        await _clienteTelegram.SendMessage(
            chatId: chatId,
            text: texto,
            parseMode: ParseMode.Markdown
        );
    }
}