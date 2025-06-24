using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class TemasUniversidad
{
    public int TemaId { get; set; }

    public string? Titulo { get; set; }

    public string? Categoria { get; set; }

    public string? Contenido { get; set; }
}
