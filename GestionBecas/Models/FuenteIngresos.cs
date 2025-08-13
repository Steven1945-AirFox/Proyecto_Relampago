using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class FuenteIngresos
{
    public int IdFuente { get; set; }

    public string? NombreFuente { get; set; }

    public int? Estado { get; set; }

    public virtual ICollection<FuenteIngresosEstudiante> FuenteIngresosEstudiante { get; set; } = new List<FuenteIngresosEstudiante>();

    public virtual ICollection<FuenteIngresosNucleoFamiliarEstudiante> FuenteIngresosNucleoFamiliarEstudiante { get; set; } = new List<FuenteIngresosNucleoFamiliarEstudiante>();
}
