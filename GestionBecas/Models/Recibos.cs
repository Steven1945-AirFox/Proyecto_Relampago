using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class Recibos
{
    public int IdRecibo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<RecibosEstudiante> RecibosEstudiante { get; set; } = new List<RecibosEstudiante>();
}
