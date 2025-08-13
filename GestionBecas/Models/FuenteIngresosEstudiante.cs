using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class FuenteIngresosEstudiante
{
    public string CarnetEstudiante { get; set; } = null!;

    public int IdFuente { get; set; }

    public int Presencia { get; set; }

    public int Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;

    public virtual FuenteIngresos IdFuenteNavigation { get; set; } = null!;
}
