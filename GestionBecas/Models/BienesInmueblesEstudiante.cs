using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class BienesInmueblesEstudiante
{
    public string CarnetEstudiante { get; set; } = null!;

    public string? NombrePropietario { get; set; }

    public int? TipoDeBien { get; set; }

    public double? ExtensionM2 { get; set; }

    public int? UsoDelBien { get; set; }

    public double? IngresoMensualGenerado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
