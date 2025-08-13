using System;
using System.Collections.Generic;

namespace GestionBecas.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido1 { get; set; }

    public string? Apellido2 { get; set; }

    public string? Identificacion { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<EstudianteUsuario> EstudianteUsuario { get; set; } = new List<EstudianteUsuario>();
}
