using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Logica.Interfaces
{
    public interface IServicioClima
    {
        Task<string> ObtenerClimaActualAsync(string ciudad);
    }
}
