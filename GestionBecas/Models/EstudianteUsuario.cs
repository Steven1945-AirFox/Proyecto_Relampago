using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class EstudianteUsuario
{
    public string Carnet { get; set; } = null!;

    public int? IdUsuario { get; set; }

    public string? Carrera { get; set; }

    public string? Nacionalidad { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public int? Edad { get; set; }

    public string? EstadoCivil { get; set; }

    public string? NivelCarrera { get; set; }

    public string? Provincia { get; set; }

    public string? Canton { get; set; }

    public string? Distrito { get; set; }

    public string? DireccionExacta { get; set; }

    public int? CondicionSolicitante { get; set; }

    public int? Genero { get; set; }

    public virtual ICollection<AposentosViviendaEstudiante> AposentosViviendaEstudiante { get; set; } = new List<AposentosViviendaEstudiante>();

    public virtual BienesInmueblesEstudiante? BienesInmueblesEstudiante { get; set; }

    public virtual ICollection<BienesMueblesEstudiante> BienesMueblesEstudiante { get; set; } = new List<BienesMueblesEstudiante>();

    public virtual DatosAcademicos? DatosAcademicos { get; set; }

    public virtual ICollection<DocumentosEstudiante> DocumentosEstudiante { get; set; } = new List<DocumentosEstudiante>();

    public virtual ICollection<FuenteIngresosEstudiante> FuenteIngresosEstudiante { get; set; } = new List<FuenteIngresosEstudiante>();

    public virtual ICollection<FuenteIngresosNucleoFamiliarEstudiante> FuenteIngresosNucleoFamiliarEstudiante { get; set; } = new List<FuenteIngresosNucleoFamiliarEstudiante>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<IngresosEstudiante> IngresosEstudiante { get; set; } = new List<IngresosEstudiante>();

    public virtual ICollection<RecibosEstudiante> RecibosEstudiante { get; set; } = new List<RecibosEstudiante>();

    public virtual ICollection<Resolucion> Resolucion { get; set; } = new List<Resolucion>();

    public virtual TipoViviendaEstudiante? TipoViviendaEstudiante { get; set; }
}
