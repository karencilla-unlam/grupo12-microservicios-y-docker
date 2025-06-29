using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace TelegramBot.Logica;

public interface ICohereLogica
{
    Task<string> GenerarRespuestaAsync(string prompt);
    Task<int> CompararSimilitudAsync(string preguntaUsuario, List<string> preguntasAlmacenadas, double umbral = 0.8);

}

public class CohereLogica : ICohereLogica
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "api cohere key";

        public CohereLogica(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GenerarRespuestaAsync(string prompt)
        {
            var requestBody = new
            {
                model = "command",
                prompt = prompt,
                max_tokens = 500
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/generate", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);
            return result.generations[0].text;
        }

    public async Task<int> CompararSimilitudAsync(string preguntaUsuario, List<string> preguntasAlmacenadas, double umbral = 0.8)
    {
        var requestBody = new
        {
            model = "embed-english-v3.0",
            texts = new[] { preguntaUsuario }.Concat(preguntasAlmacenadas).ToArray()
        };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/embed", content);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseBody);

        var embeddingUsuario = result.embeddings[0].ToObject<float[]>();

        double maxSimilitud = double.MinValue;
        int indiceMejor = -1;
        for (int i = 1; i < preguntasAlmacenadas.Count + 1; i++)
        {
            var embeddingAlmacenada = result.embeddings[i].ToObject<float[]>();
            double similitud = CalcularSimilitudCoseno(embeddingUsuario, embeddingAlmacenada);
            if (similitud > maxSimilitud)
            {
                maxSimilitud = similitud;
                indiceMejor = i - 1;
            }
        }
        // Solo devuelve el índice si la similitud supera el umbral
        return (maxSimilitud >= umbral) ? indiceMejor : -1;
    }

    // Método auxiliar para calcular la similitud del coseno
    public static double CalcularSimilitudCoseno(float[] vectorA, float[] vectorB)
    {
        double dot = 0.0;
        double magA = 0.0;
        double magB = 0.0;
        for (int i = 0; i < vectorA.Length; i++)
        {
            dot += vectorA[i] * vectorB[i];
            magA += Math.Pow(vectorA[i], 2);
            magB += Math.Pow(vectorB[i], 2);
        }
        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }

}

