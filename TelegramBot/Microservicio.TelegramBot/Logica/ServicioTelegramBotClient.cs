
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