using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class PeriodoRecepcion
{
    public int Idperiodo { get; set; }

    public DateTime? FechaHoraInicio { get; set; }

    public DateTime? FechaHoraFin { get; set; }

    public int? Estado { get; set; }

    public string? Nombre { get; set; }
}
