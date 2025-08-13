using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class IngresosEstudiante
{
    public int IdIngreso { get; set; }

    public string CarnetEstudiante { get; set; } = null!;

    public string? CedulaPariente { get; set; }

    public string? NombrePariente { get; set; }

    public string? Apellido1Pariente { get; set; }

    public string? Apellido2Pariente { get; set; }

    public int? EdadPariente { get; set; }

    public string? ParentezcoConEstudiante { get; set; }

    public int? EstadoCivil { get; set; }

    public int? Estudia { get; set; }

    public int? Beca { get; set; }

    public string? CondicionSalud { get; set; }

    public string? Ocupacion { get; set; }

    public string? InstitucionDondeLaboraPensionBeca { get; set; }

    public double? IngresoMensualBruto { get; set; }

    public int? Estado { get; set; }

    public virtual EstudianteUsuario CarnetEstudianteNavigation { get; set; } = null!;
}
