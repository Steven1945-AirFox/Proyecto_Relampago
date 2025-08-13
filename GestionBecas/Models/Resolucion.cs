using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class Resolucion
{
    public int IdResolucion { get; set; }

    public string CarnetEstudiante { get; set; } = null!;

    public int? LineaPobrezaCuc { get; set; }

    public int? Sinirube { get; set; }

    public int? Resultado { get; set; }

    public int? CategoriaBecaSocieconomicaAsignada { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaResolucion { get; set; }

    public string PersonaSupervisa { get; set; } = null!;

    public string PersonaAprueba { get; set; } = null!;

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
