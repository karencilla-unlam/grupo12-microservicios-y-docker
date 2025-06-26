using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Logica.Interfaces
{
    public interface IServicioPreguntas
    {
        /// <summary>
        /// Obtiene una respuesta basada en el texto de una pregunta.
        /// Puede consultar una base de datos o un servicio externo (como clima).
        /// </summary>
        /// <param name="textoPregunta">Texto ingresado por el usuario.</param>
        /// <returns>Respuesta adecuada a la pregunta.</returns>
        Task<string> ObtenerRespuestaAsync(string textoPregunta);
    }
}
