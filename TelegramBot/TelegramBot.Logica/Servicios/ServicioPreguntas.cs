using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Data.EF;
using TelegramBot.Logica.Interfaces;
using TelegramBot.Logica.Servicios;

namespace TelegramBot.Logica.Servicios
{
    public class ServicioPreguntas : IServicioPreguntas
    {
        private readonly TelegramBotContext _contexto;
        private readonly IServicioClima _servicioClima;
        private readonly CohereLogica _cohereLogica;
        private readonly ILogger<ServicioPreguntas> _logger;

        public ServicioPreguntas(TelegramBotContext contexto, IServicioClima servicioClima, CohereLogica cohereLogica, ILogger<ServicioPreguntas> logger)
        {
            _contexto = contexto;
            _servicioClima = servicioClima;
            _cohereLogica = cohereLogica;
            _logger = logger;
        }

        public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
        {
            _logger.LogInformation("Pregunta recibida: {Pregunta}", textoPregunta);

            if (EsPreguntaSobreClima(textoPregunta))
            {
                var respuestaClima = await _servicioClima.ObtenerClimaActualAsync("San Justo");
                _logger.LogInformation("Respuesta (Clima): {Respuesta}", respuestaClima);
                return respuestaClima;
            }

            var consulta = await _contexto.Consultas
                .FirstOrDefaultAsync(c => textoPregunta.Contains(c.Pregunta));

            if (consulta != null)
            {
                _logger.LogInformation("Respuesta (DB): {Respuesta}", consulta.Respuesta);
                return consulta.Respuesta;
            }

            var respuestaCohere = await _cohereLogica.GenerarRespuestaAsync(textoPregunta);
            _logger.LogInformation("Respuesta (Cohere): {Respuesta}", respuestaCohere);
            return respuestaCohere;
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
