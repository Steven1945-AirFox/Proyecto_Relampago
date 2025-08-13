using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class AposentosVivienda
{
    public int IdAposento { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<AposentosViviendaEstudiante> AposentosViviendaEstudiante { get; set; } = new List<AposentosViviendaEstudiante>();
}
