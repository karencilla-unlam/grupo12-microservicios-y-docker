using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Logica;

public interface ICohereLogica
{
    Task<string> GenerarRespuestaAsync(string prompt);
    Task<int> CompararSimilitudAsync(string preguntaUsuario, List<string> preguntasAlmacenadas, double umbral = 0.8);
}

public class CohereLogica : ICohereLogica
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CohereLogica> _logger;
    private readonly string _apiKey = "apy key";

    public CohereLogica(HttpClient httpClient, ILogger<CohereLogica> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GenerarRespuestaAsync(string prompt)
    {
        string contexto = "Eres un asistente virtual de la Universidad Nacional de La Matanza (UNLaM). Responde siempre en español y proporciona información precisa y relevante sobre la universidad.";
        string promptFinal = $"{contexto}\nPregunta: {prompt}\nRespuesta:";

        var requestBody = new
        {
            model = "command",
            prompt = promptFinal,
            max_tokens = 500
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/generate", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseBody);
        return result.generations[0].text;
    }

    public async Task<int> CompararSimilitudAsync(string preguntaUsuario, List<string> preguntasAlmacenadas, double umbral = 0.70)
    {
        var requestBody = new
        {
            model = "embed-english-v3.0",
            input_type = "search_document",
            texts = new[] { preguntaUsuario }.Concat(preguntasAlmacenadas).ToArray()
        };
        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        _logger.LogInformation("Payload enviado a Cohere: {Payload}", JsonConvert.SerializeObject(requestBody));

        var response = await _httpClient.PostAsync("https://api.cohere.ai/v1/embed", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error Cohere: {StatusCode}", response.StatusCode);
            _logger.LogError("Respuesta Cohere: {ResponseBody}", responseBody);
            throw new Exception($"Cohere API error: {response.StatusCode} - {responseBody}");
        }

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