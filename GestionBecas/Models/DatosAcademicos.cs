using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class DatosAcademicos
{
    public string CarnetEstudiante { get; set; } = null!;

    public string? NombreColegio { get; set; }

    public int? TipoInstitucion { get; set; }

    public int? BecaEnColegio { get; set; }

    public string? NombreInstitucionBecaria { get; set; }

    public double? MontoBeca { get; set; }

    public int? TitulosUniversitariosParauniversitariosAnteriores { get; set; }

    public int? MayorGradoAprobado { get; set; }

    public int? BecaUnivarsitaria { get; set; }

    public double? Monto { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
