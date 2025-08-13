using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class BienesMueblesEstudiante
{
    public string CarnetEstudiante { get; set; } = null!;

    public string? NombrePropietario { get; set; }

    public string NumeroPlaca { get; set; } = null!;

    public int? TipoDeBien { get; set; }

    public int? Año { get; set; }

    public double? MontoMarchamo { get; set; }

    public int? UsoDelBien { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
