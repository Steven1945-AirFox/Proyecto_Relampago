using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class TipoViviendaEstudiante
{
    public string CarnetEstudiante { get; set; } = null!;

    public int TipoVivienda { get; set; }

    public int MedioAdquisicion { get; set; }

    public int? MetrosCuadrados { get; set; }

    public int? Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
