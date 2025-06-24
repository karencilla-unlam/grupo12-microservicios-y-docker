using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Logica.Interfaces;
using TelegramBot.Logica.Servicios;

namespace TelegramBot.Logica.Servicios
{
    public class ServicioPreguntas : IServicioPreguntas
    {
        private readonly ContextoBaseDatosUnlam _contexto;
        private readonly IServicioClima _servicioClima;

        public ServicioPreguntas(ContextoBaseDatosUnlam contexto, IServicioClima servicioClima)
        {
            _contexto = contexto;
            _servicioClima = servicioClima;
        }

        public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
        {
            if (EsPreguntaSobreClima(textoPregunta))
            {
                // Para pruebas, inicialmente estamos usando "San Justo" como ubicación
                return await _servicioClima.ObtenerClimaActualAsync("San Justo");
            }

            var pregunta = await _contexto.Preguntas
                .FirstOrDefaultAsync(p => textoPregunta.Contains(p.Texto));

            return pregunta?.Respuesta ?? "Lo siento, no encontré una respuesta para tu pregunta.";
        }

        private bool EsPreguntaSobreClima(string texto)
        {
            var textoNormalizado = texto.ToLower();
            return textoNormalizado.Contains("clima") ||
                   textoNormalizado.Contains("tiempo") ||
                   textoNormalizado.Contains("temperatura");
        }
    }
}
