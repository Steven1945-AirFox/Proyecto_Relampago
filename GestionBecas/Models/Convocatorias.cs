using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class Convocatorias
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaCierre { get; set; }

    public string TipoBeca { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<Documentos> Documento { get; set; } = new List<Documentos>();
}
