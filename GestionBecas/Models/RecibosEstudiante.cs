using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class RecibosEstudiante
{
    public int IdRegistro { get; set; }

    public string CarnetEstudiante { get; set; } = null!;

    public int IdRecibo { get; set; }

    public decimal Monto { get; set; }

    public int? Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;

    public virtual Recibos IdReciboNavigation { get; set; } = null!;
}
