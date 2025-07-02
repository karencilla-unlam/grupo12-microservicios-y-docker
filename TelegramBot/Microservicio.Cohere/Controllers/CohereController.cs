using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TelegramBot.Logica;

namespace Microservicio.Cohere.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CohereController : ControllerBase
{
    private readonly ICohereLogica _cohereLogica;
    private readonly ILogger<CohereController> _logger;

    public CohereController(ICohereLogica cohereLogica, ILogger<CohereController> logger)
    {
        _cohereLogica = cohereLogica;
        _logger = logger;
    }

    [HttpPost("generar-respuesta")]
    public async Task<IActionResult> GenerarRespuesta([FromBody] string prompt)
    {
        try
        {
            var respuesta = await _cohereLogica.GenerarRespuestaAsync(prompt);
            return Ok(respuesta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GenerarRespuesta con prompt: {Prompt}", prompt);
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    [HttpPost("comparar-similitud")]
    public async Task<IActionResult> CompararSimilitud([FromBody] CompararSimilitudRequest request)
    {
        try
        {
            var indice = await _cohereLogica.CompararSimilitudAsync(
                request.PreguntaUsuario,
                request.PreguntasAlmacenadas,
                request.Umbral
            );
            return Ok(indice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CompararSimilitud con request: {@Request}", request);
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    public class CompararSimilitudRequest
    {
        public string PreguntaUsuario { get; set; }
        public List<string> PreguntasAlmacenadas { get; set; }
        public double Umbral { get; set; } = 0.8;
    }
}