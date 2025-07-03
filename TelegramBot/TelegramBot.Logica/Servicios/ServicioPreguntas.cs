using Microsoft.EntityFrameworkCore;
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

        public ServicioPreguntas(TelegramBotContext contexto, IServicioClima servicioClima, CohereLogica cohereLogica)
        {
            _contexto = contexto;
            _servicioClima = servicioClima;
            _cohereLogica = cohereLogica;
        }

        /* public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
         {
             if (EsPreguntaSobreClima(textoPregunta))
             {
                 // Para pruebas, inicialmente estamos usando "San Justo" como ubicación
                 return await _servicioClima.ObtenerClimaActualAsync("San Justo");
             }

             var pregunta = await _contexto.Preguntas
                 .FirstOrDefaultAsync(p => textoPregunta.Contains(p.Texto));

             return pregunta?.Respuesta ?? "Lo siento, no encontré una respuesta para tu pregunta.";
         }*/

        //cambia nombre de variable preguntas a consultas. ver como tiene cada uno su base
        public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
        {
            if (EsPreguntaSobreClima(textoPregunta))
            {
                // Para pruebas, inicialmente estamos usando "San Justo" como ubicación
                return await _servicioClima.ObtenerClimaActualAsync("San Justo");
            }

            var consulta = await _contexto.Consultas
                .FirstOrDefaultAsync(c => textoPregunta.Contains(c.Pregunta));

            return consulta?.Respuesta ?? await _cohereLogica.GenerarRespuestaAsync(textoPregunta);
            
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
