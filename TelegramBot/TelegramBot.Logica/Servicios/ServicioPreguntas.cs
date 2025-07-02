using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.Data.EF;
using TelegramBot.Logica.Interfaces;

namespace TelegramBot.Logica.Servicios
{
    public class ServicioPreguntas : IServicioPreguntas
    {
        private readonly TelegramBotContext _contexto;
        private readonly IServicioClima _servicioClima;
        private readonly CohereMicroservicioClient _cohereClient;
        private readonly ILogger<ServicioPreguntas> _logger;

        public ServicioPreguntas(
            TelegramBotContext contexto,
            IServicioClima servicioClima,
            CohereMicroservicioClient cohereClient,
            ILogger<ServicioPreguntas> logger)
        {
            _contexto = contexto;
            _servicioClima = servicioClima;
            _cohereClient = cohereClient;
            _logger = logger;
        }

        public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
        {
            _logger.LogInformation("Pregunta recibida: {Pregunta}", textoPregunta);

            var respuestaClima = await ManejarPreguntaClimaAsync(textoPregunta);
            if (respuestaClima != null)
            {
                _logger.LogInformation("Respuesta clima encontrada: {Respuesta}", respuestaClima);
                return respuestaClima;
            }

            var respuestaBase = await BuscarRespuestaEnBaseAsync(textoPregunta);
            if (respuestaBase != null)
            {
                _logger.LogInformation("Respuesta base encontrada: {Respuesta}", respuestaBase);
                return respuestaBase;
            }

            var respuestaIA = await GenerarRespuestaIAAsync(textoPregunta);
            _logger.LogInformation("Respuesta IA generada: {Respuesta}", respuestaIA);
            return respuestaIA;
        }

        private async Task<string?> ManejarPreguntaClimaAsync(string textoPregunta)
        {
            if (EsPreguntaSobreClima(textoPregunta))
            {
                return await _servicioClima.ObtenerClimaActualAsync("San Justo");
            }
            return null;
        }

        private async Task<string?> BuscarRespuestaEnBaseAsync(string textoPregunta)
        {
            var respuestaExacta = await BuscarRespuestaExactaAsync(textoPregunta);
            if (respuestaExacta != null)
                return respuestaExacta;

            var respuestaSimilar = await BuscarRespuestaSimilarAsync(textoPregunta);
            if (respuestaSimilar != null)
                return respuestaSimilar;

            return null;
        }

        private async Task<string?> BuscarRespuestaExactaAsync(string textoPregunta)
        {
            var consulta = await _contexto.Consultas
                .FirstOrDefaultAsync(c => textoPregunta.Contains(c.Pregunta));

            if (consulta != null && !string.IsNullOrEmpty(consulta.Respuesta))
                return consulta.Respuesta;

            return null;
        }

        private async Task<string?> BuscarRespuestaSimilarAsync(string textoPregunta)
        {
            var preguntasAlmacenadas = await _contexto.Consultas
                .Where(c => !string.IsNullOrEmpty(c.Pregunta) && !string.IsNullOrEmpty(c.Respuesta))
                .Select(c => c.Pregunta)
                .ToListAsync();

            if (preguntasAlmacenadas.Any())
            {
                int idxSimilar = await _cohereClient.CompararSimilitudAsync(textoPregunta, preguntasAlmacenadas);
                _logger.LogInformation("Índice similar encontrado: {Indice}", idxSimilar);

                if (idxSimilar >= 0 && idxSimilar < preguntasAlmacenadas.Count)
                {
                    var consultaSimilar = await _contexto.Consultas
                        .FirstOrDefaultAsync(c => c.Pregunta == preguntasAlmacenadas[idxSimilar]);
                    if (consultaSimilar != null && !string.IsNullOrEmpty(consultaSimilar.Respuesta))
                    {
                        _logger.LogInformation("Respuesta similar encontrada: {Respuesta}", consultaSimilar.Respuesta);
                        return consultaSimilar.Respuesta;
                    }
                }
            }
            _logger.LogInformation("No se encontró respuesta similar.");
            return null;
        }

        private async Task<string> GenerarRespuestaIAAsync(string textoPregunta)
        {
            return await _cohereClient.GenerarRespuestaAsync(textoPregunta);
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