using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class AposentosViviendaEstudiante
{
    public string CarnetEstudiante { get; set; } = null!;

    public int IdAposento { get; set; }

    public int CantidadPresentEnVivienda { get; set; }

    public int? Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;

    public virtual AposentosVivienda IdAposentoNavigation { get; set; } = null!;
}
