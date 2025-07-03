using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TelegramBotMicroservicioClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TelegramBotMicroservicioClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<bool> EnviarMensajeAsync(long chatId, string texto)
    {
        var payload = new
        {
            chatId = chatId,
            texto = texto
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/telegram/enviar-mensaje", content);
        return response.IsSuccessStatusCode;
    }
}