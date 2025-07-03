using Microsoft.AspNetCore.Mvc;
using TelegramBot.Logica.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TelegramController : ControllerBase
{
    private readonly IServicioTelegramBotClient _servicioTelegram;

    public TelegramController(IServicioTelegramBotClient servicioTelegram)
    {
        _servicioTelegram = servicioTelegram;
    }

    [HttpPost("enviar-mensaje")]
    public async Task<IActionResult> EnviarMensaje([FromBody] EnviarMensajeRequest request)
    {
        await _servicioTelegram.EnviarMensajeTextoAsync(request.ChatId, request.Texto);
        return Ok(new { exito = true });
    }
}

public class EnviarMensajeRequest
{
    public long ChatId { get; set; }
    public string Texto { get; set; }
}