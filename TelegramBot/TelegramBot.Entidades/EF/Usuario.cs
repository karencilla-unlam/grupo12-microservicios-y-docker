using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public long TelegramId { get; set; }

    public string? Nombre { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Consulta> Consulta { get; set; } = new List<Consulta>();
}
