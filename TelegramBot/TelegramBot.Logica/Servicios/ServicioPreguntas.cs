using Microsoft.EntityFrameworkCore;
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

        public ServicioPreguntas(TelegramBotContext contexto, IServicioClima servicioClima)
        {
            _contexto = contexto;
            _servicioClima = servicioClima;
        }

        public async Task<string> ObtenerRespuestaAsync(string textoPregunta)
        {
            if (EsPreguntaSobreClima(textoPregunta))
            {
                // Para pruebas, usamos "San Justo", donde está la UNLaM
                return await _servicioClima.ObtenerClimaActualAsync("San Justo");
            }

            // Normalizamos la pregunta
            var preguntaLower = textoPregunta.ToLower();

            // Buscamos un tema de la universidad que coincida parcialmente
            var tema = await _contexto.TemasUniversidads
                .FirstOrDefaultAsync(t =>
                    EF.Functions.Like(t.Titulo.ToLower(), $"%{preguntaLower}%") ||
                    EF.Functions.Like(t.Categoria.ToLower(), $"%{preguntaLower}%"));

            if (tema != null)
            {
                return $"Tema: {tema.Titulo}\nCategoría: {tema.Categoria}";
            }

            return "Lo siento, no encontré información sobre eso. ¿Podés reformular la pregunta?";
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
