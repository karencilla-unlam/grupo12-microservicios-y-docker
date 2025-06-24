using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class Consulta
{
    public int ConsultaId { get; set; }

    public int? UsuarioId { get; set; }

    public string? Pregunta { get; set; }

    public DateTime? FechaHora { get; set; }

    public string? FuenteRespuesta { get; set; }

    public string? Respuesta { get; set; }

    public virtual ICollection<Respuesta> RespuestaNavigation { get; set; } = new List<Respuesta>();

    public virtual Usuario? Usuario { get; set; }
}
