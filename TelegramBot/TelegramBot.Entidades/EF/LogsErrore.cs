using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class LogsErrore
{
    public int LogId { get; set; }

    public DateTime? Fecha { get; set; }

    public string? TipoError { get; set; }

    public string? Descripcion { get; set; }

    public string? Fuente { get; set; }
}
