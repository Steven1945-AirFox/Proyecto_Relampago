using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class DocumentosEstudiante
{
    public int IdRegistroDocumento { get; set; }

    public string CarnetEstudiante { get; set; } = null!;

    public int IdDocumento { get; set; }

    public int? Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;

    public virtual Documentos IdDocumentoNavigation { get; set; } = null!;
}
