using System;
using System.Collections.Generic;

namespace TelegramBot.Data.EF;

public partial class Clima
{
    public int ClimaId { get; set; }

    public string? Ciudad { get; set; }

    public DateTime? FechaConsulta { get; set; }

    public decimal? TemperaturaActual { get; set; }

    public string? Descripcion { get; set; }

    public int? Humedad { get; set; }

    public string? DatosCompletos { get; set; }
}
