using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TelegramBot.Logica.Servicios
{
    public class CohereMicroservicioClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CohereMicroservicioClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            var content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/cohere/generar-respuesta", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public class CompararSimilitudRequest
        {
            public string PreguntaUsuario { get; set; }
            public List<string> PreguntasAlmacenadas { get; set; }
            public double Umbral { get; set; } = 0.8;
        }

        public async Task<int> CompararSimilitudAsync(string preguntaUsuario, List<string> preguntasAlmacenadas, double umbral = 0.8)
        {
            var request = new CompararSimilitudRequest
            {
                PreguntaUsuario = preguntaUsuario,
                PreguntasAlmacenadas = preguntasAlmacenadas,
                Umbral = umbral
            };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/cohere/comparar-similitud", content);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return int.Parse(resultString);
        }
    }
}

