using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class Respuesta
{
    public int RespuestaId { get; set; }

    public int? ConsultaId { get; set; }

    public string? Contenido { get; set; }

    public string? GeneradaPor { get; set; }

    public int? TiempoGeneracionMs { get; set; }

    public virtual Consulta? Consulta { get; set; }
}
