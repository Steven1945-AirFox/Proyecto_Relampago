using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class Documentos
{
    public int IdDocumento { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<DocumentosEstudiante> DocumentosEstudiante { get; set; } = new List<DocumentosEstudiante>();
}
