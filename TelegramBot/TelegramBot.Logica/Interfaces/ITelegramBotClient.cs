using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Logica.Interfaces
{
    public interface IServicioTelegramBot
    {
        Task EnviarMensajeTextoAsync(long chatId, string texto);
    }
}
