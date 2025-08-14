using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionBecas.Models
{
    public class GestorBecas_ViewModel
    {
        // Datos del periodo de recepción
        public GestorBecas_PeriodoRecepcion GestorBecas_PeriodoVigente { get; set; }
        public bool GestorBecas_PeriodoHabilitado => GestorBecas_PeriodoVigente != null &&
                                      DateTime.Now >= GestorBecas_PeriodoVigente.GestorBecas_FechaInicio &&
                                      DateTime.Now <= GestorBecas_PeriodoVigente.GestorBecas_FechaFinalizacion;

        // Datos del usuario/estudiante
        public GestorBecas_Usuario GestorBecas_DatosUsuario { get; set; }
        public GestorBecas_Estudiante GestorBecas_InformacionEstudiante { get; set; }

        // Datos académicos
        public GestorBecas_DatosAcademicos GestorBecas_DatosAcademicos { get; set; }

        // Fuentes de ingreso
        public List<GestorBecas_FuenteIngreso> GestorBecas_FuentesIngresoFamiliar { get; set; }
        public List<GestorBecas_FuenteIngreso> GestorBecas_FuentesIngresoPersonales { get; set; }

        // Bienes
        public List<GestorBecas_BienInmueble> GestorBecas_BienesInmuebles { get; set; }
        public List<GestorBecas_BienMueble> GestorBecas_BienesMuebles { get; set; }

        // Vivienda
        public GestorBecas_CaracteristicasVivienda GestorBecas_CaracteristicasVivienda { get; set; }
        public List<GestorBecas_AposentoVivienda> GestorBecas_AposentosVivienda { get; set; }

        // Ingresos familiares
        public List<GestorBecas_IngresoFamiliar> GestorBecas_IngresosFamiliares { get; set; }

        // Egresos familiares
        public List<GestorBecas_ReciboEstudiante> GestorBecas_EgresosFamiliares { get; set; }

        // Documentos
        public List<GestorBecas_DocumentoEstudiante> GestorBecas_Documentos { get; set; }

        // Proceso de revisión (para empleados)
        public GestorBecas_DecisionBeca GestorBecas_DecisionBeca { get; set; }
        public string GestorBecas_Observaciones { get; set; }
        public int GestorBecas_EstadoSolicitud { get; set; } // 0=Pendiente, 1=Revisado, 2=Aprobado, 3=Rechazado

        // Métodos auxiliares
        public decimal GestorBecas_CalcularIngresoPerCapita()
        {
            decimal ingresosTotales = GestorBecas_IngresosFamiliares?.Sum(i => (decimal)i.GestorBecas_IngresoMensualBruto) ?? 0;
            decimal egresosTotales = GestorBecas_EgresosFamiliares?.Sum(e => (decimal)e.GestorBecas_Monto) ?? 0;
            int miembrosFamilia = GestorBecas_IngresosFamiliares?.Count ?? 1;

            return ((ingresosTotales - egresosTotales) / miembrosFamilia) / 30;
        }
    }

    // Clases auxiliares para el ViewModel
    public class GestorBecas_PeriodoRecepcion
    {
        public int GestorBecas_Id { get; set; }
        public DateTime GestorBecas_FechaInicio { get; set; }
        public DateTime GestorBecas_FechaFinalizacion { get; set; }
        public int GestorBecas_Estado { get; set; }
        public string GestorBecas_Descripcion { get; set; }
    }

    public class GestorBecas_Usuario
    {
        public int GestorBecas_Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string GestorBecas_PrimerNombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es requerido")]
        public string GestorBecas_PrimerApellido { get; set; }
        public string GestorBecas_SegundoApellido { get; set; }

        [Required(ErrorMessage = "La identificación es requerida")]
        public string GestorBecas_NumeroIdentificacion { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string GestorBecas_CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string GestorBecas_TelefonoContacto { get; set; }
    }

    public class GestorBecas_Estudiante
    {
        [Required(ErrorMessage = "El carnet es requerido")]
        public string GestorBecas_Carnet { get; set; }
        public int GestorBecas_IdUsuario { get; set; }

        [Required(ErrorMessage = "La carrera es requerida")]
        public string GestorBecas_Carrera { get; set; }
        public string GestorBecas_Nacionalidad { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime? GestorBecas_FechaNacimiento { get; set; }
        public int GestorBecas_Edad => GestorBecas_FechaNacimiento.HasValue ? DateTime.Now.Year - GestorBecas_FechaNacimiento.Value.Year : 0;

        [Required(ErrorMessage = "El estado civil es requerido")]
        public string GestorBecas_EstadoCivil { get; set; }

        [Required(ErrorMessage = "El nivel de carrera es requerido")]
        public string GestorBecas_NivelCarrera { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        public string GestorBecas_Provincia { get; set; }

        [Required(ErrorMessage = "El cantón es requerido")]
        public string GestorBecas_Canton { get; set; }
        public string GestorBecas_Distrito { get; set; }

        [Required(ErrorMessage = "La dirección exacta es requerida")]
        public string GestorBecas_DireccionExacta { get; set; }

        [Required(ErrorMessage = "La condición del solicitante es requerida")]
        public int GestorBecas_CondicionSolicitante { get; set; } // 0=Primer ingreso, 1=Regular primera vez, 2=Regular con beca anterior

        [Required(ErrorMessage = "El género es requerido")]
        public int GestorBecas_Genero { get; set; } // 0=Femenino, 1=Masculino, 2=No binario

        // Dirección mientras estudia (puede ser diferente)
        public string GestorBecas_ProvinciaEstudio { get; set; }
        public string GestorBecas_CantonEstudio { get; set; }
        public string GestorBecas_DistritoEstudio { get; set; }
        public string GestorBecas_DireccionExactaEstudio { get; set; }

        // Información de beca anterior (si aplica)
        public string GestorBecas_TipoBecaAnterior { get; set; }
        public int? GestorBecas_UltimoCuatrimestreBeca { get; set; }
        public int? GestorBecas_AnioBeca { get; set; }
    }

    public class GestorBecas_DatosAcademicos
    {
        public string GestorBecas_CarnetEstudiante { get; set; }

        [Required(ErrorMessage = "El nombre del colegio es requerido")]
        public string GestorBecas_NombreColegio { get; set; }

        [Required(ErrorMessage = "El tipo de institución es requerido")]
        public int GestorBecas_TipoInstitucion { get; set; } // 1=Pública, 2=Privada

        [Required(ErrorMessage = "Debe indicar si tuvo beca en el colegio")]
        public int GestorBecas_BecaEnColegio { get; set; } // 0=No, 1=Si
        public string GestorBecas_NombreInstitucionBecaria { get; set; }
        public decimal? GestorBecas_MontoBeca { get; set; }

        [Required(ErrorMessage = "Debe indicar si posee títulos anteriores")]
        public int GestorBecas_TitulosUniversitarios { get; set; } // 0=No, 1=Si
        public string GestorBecas_MayorGradoAprobado { get; set; }

        [Required(ErrorMessage = "Debe indicar si tuvo beca universitaria")]
        public int GestorBecas_BecaUniversitaria { get; set; } // 0=No, 1=Si
        public decimal? GestorBecas_Monto { get; set; }
    }

    public class GestorBecas_FuenteIngreso
    {
        public int GestorBecas_IdFuente { get; set; }
        public string GestorBecas_NombreFuente { get; set; }
        public bool GestorBecas_Seleccionado { get; set; }
        public string GestorBecas_Detalles { get; set; } // Para "Otros" especificar
    }

    public class GestorBecas_BienInmueble
    {
        public string GestorBecas_NombrePropietario { get; set; }

        [Required(ErrorMessage = "El tipo de bien es requerido")]
        public int GestorBecas_TipoDeBien { get; set; } // 1=Terreno, 2=Casa, 3=Lote, 4=Apartamento, 5=Otro
        public decimal? GestorBecas_ExtensionM2 { get; set; }

        [Required(ErrorMessage = "El uso del bien es requerido")]
        public int GestorBecas_UsoDelBien { get; set; } // 1=Personal, 2=Laboral
        public decimal? GestorBecas_IngresoMensualGenerado { get; set; }
        public string GestorBecas_DescripcionOtro { get; set; } // Para cuando TipoDeBien es 5=Otro
    }

    public class GestorBecas_BienMueble
    {
        public string GestorBecas_NombrePropietario { get; set; }
        public string GestorBecas_NumeroPlaca { get; set; }

        [Required(ErrorMessage = "El tipo de bien es requerido")]
        public int GestorBecas_TipoDeBien { get; set; } // 1=Auto, 2=Motocicleta, 3=Cuadraciclo, 4=Camion, 5=Otro
        public int? GestorBecas_Anio { get; set; }
        public decimal? GestorBecas_MontoMarchamo { get; set; }

        [Required(ErrorMessage = "El uso del bien es requerido")]
        public int GestorBecas_UsoDelBien { get; set; } // 1=Personal, 2=Laboral
        public string GestorBecas_Marca { get; set; }
        public string GestorBecas_DescripcionOtro { get; set; } // Para cuando TipoDeBien es 5=Otro
    }

    public class GestorBecas_CaracteristicasVivienda
    {
        public string GestorBecas_CarnetEstudiante { get; set; }

        [Required(ErrorMessage = "El tipo de vivienda es requerido")]
        public int GestorBecas_TipoVivienda { get; set; } // 1=Alquilada, 2=Prestada, 3=Propia con Hipoteca, 4=Propia sin hipoteca

        [Required(ErrorMessage = "El medio de adquisición es requerido")]
        public int GestorBecas_MedioObtencion { get; set; } // 1=Fondos propios, 2=Préstamo, 3=Herencia, 4=Bono de vivienda, 5=Donación, 6=Otro
        public int? GestorBecas_AreaMetrosCuadrados { get; set; }
        public int GestorBecas_Estado { get; set; }
        public string GestorBecas_DetalleMedioObtencion { get; set; } // Para cuando MedioObtencion es 6=Otro
    }

    public class GestorBecas_AposentoVivienda
    {
        public int GestorBecas_IdAposento { get; set; }
        public string GestorBecas_Nombre { get; set; }
        public bool GestorBecas_Presente { get; set; }
        public int GestorBecas_Cantidad { get; set; }
        public string GestorBecas_Detalle { get; set; } // Para "Otros" especificar
    }

    public class GestorBecas_IngresoFamiliar
    {
        public string GestorBecas_CedulaPariente { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string GestorBecas_NombrePariente { get; set; }

        [Required(ErrorMessage = "El primer apellido es requerido")]
        public string GestorBecas_Apellido1Pariente { get; set; }
        public string GestorBecas_Apellido2Pariente { get; set; }

        [Required(ErrorMessage = "La edad es requerida")]
        public int? GestorBecas_EdadPariente { get; set; }

        [Required(ErrorMessage = "El parentesco es requerido")]
        public string GestorBecas_ParentezcoConEstudiante { get; set; }

        [Required(ErrorMessage = "El estado civil es requerido")]
        public int GestorBecas_EstadoCivil { get; set; } // 1=Casado, 2=Soltero, 3=Unión libre, 4=Divorciado, 5=Viudo

        [Required(ErrorMessage = "Debe indicar si estudia")]
        public int GestorBecas_Estudia { get; set; } // 0=No, 1=Si

        [Required(ErrorMessage = "Debe indicar si tiene beca")]
        public int GestorBecas_Beca { get; set; } // 0=No, 1=Si
        public string GestorBecas_CondicionSalud { get; set; }

        [Required(ErrorMessage = "La ocupación es requerida")]
        public string GestorBecas_Ocupacion { get; set; }
        public string GestorBecas_InstitucionDondeLabora { get; set; }

        [Required(ErrorMessage = "El ingreso mensual es requerido")]
        public decimal GestorBecas_IngresoMensualBruto { get; set; }
        public int GestorBecas_Estado { get; set; }
    }

    public class GestorBecas_ReciboEstudiante
    {
        public int GestorBecas_IdRecibo { get; set; }
        public string GestorBecas_Nombre { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        public decimal GestorBecas_Monto { get; set; }
        public int GestorBecas_Estado { get; set; }
    }

    public class GestorBecas_DocumentoEstudiante
    {
        public int GestorBecas_IdDocumento { get; set; }
        public string GestorBecas_Nombre { get; set; }
        public IFormFile GestorBecas_Archivo { get; set; }
        public int GestorBecas_Estado { get; set; } // 1=Presente, 2=Omiso, 3=Incorrecto, 4=No Aplica
        public string GestorBecas_RutaArchivo { get; set; }
        public bool GestorBecas_Obligatorio { get; set; }
    }

    public class GestorBecas_DecisionBeca
    {
        public int GestorBecas_Id { get; set; }
        public string GestorBecas_CarnetEstudiante { get; set; }
        public int GestorBecas_NivelPobreza { get; set; } // 1=PobrezaExtrema, 2=Pobreza, 3=Fuera
        public int GestorBecas_ClasificacionSINIRUBE { get; set; } // 1=PobrezaExtrema, 2=PobrezaBásica, etc.
        public int GestorBecas_EstadoSolicitud { get; set; } // 0=Rechazado, 1=Aprobado
        public int GestorBecas_TipoBecaAsignada { get; set; } // 1=Beca0, 2=Beca1, 3=Beca2, 4=Beca3
        public string GestorBecas_Comentarios { get; set; }
        public DateTime GestorBecas_FechaDecision { get; set; }
        public string GestorBecas_Revisor { get; set; }
        public string GestorBecas_Aprobador { get; set; }
    }




    public class Convocatoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }
        public string TipoBeca { get; set; }
        public string Estado { get; set; }

        // Make this optional (nullable)
        public int? TotalSolicitudes { get; set; }
    }

    public class DocumentoRequerido
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Obligatorio { get; set; }
    }
















    // SolicitudBeca.cs
    public class SolicitudBeca
    {
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaEstudiante { get; set; }
        public string CarnetEstudiante { get; set; }
        public string Carrera { get; set; }
        public string TipoBeca { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int Estado { get; set; } // 1=En revisión, 2=Doc. completa, etc.
        public int? Resultado { get; set; }
        public int? CategoriaBecaSocioeconomicaAsignada { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string PersonaSupervisa { get; set; }
        public string PersonaAprueba { get; set; }






        // Resolution properties
        public bool TieneResolucion { get; set; }
        public int? ResultadoResolucion { get; set; } // 0=Rechazado, 1=Aprobado
        public int? CategoriaBeca { get; set; } // 1=Beca0, 2=Beca1, etc.
        public DateTime? gestorFechaResolucion { get; set; }
        public string ObservacionesResolucion { get; set; }


       
        public DateTime? gestionFechaResolucion { get; set; } // 👈 coincide con tu CSHTML
    







    }



    // DetalleSolicitud.cs
    public class DetalleSolicitud
    {
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaEstudiante { get; set; }
        public string CarnetEstudiante { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string Genero { get; set; }
        public string EstadoCivil { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string DireccionExacta { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string TipoVivienda { get; set; }
        public int MetrosCuadrados { get; set; }
        public string Carrera { get; set; }
        public string NivelCarrera { get; set; }
        public string NombreColegio { get; set; }
        public string TipoInstitucion { get; set; }
        public string NombreConvocatoria { get; set; }
        public string TipoBeca { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int Estado { get; set; }

        public List<DocumentoSolicitud> Documentos { get; set; }
        public List<IngresoFamiliar> IngresosFamiliares { get; set; }
        public List<GastoMensual> GastosMensuales { get; set; }
    }

    public class DocumentoSolicitud
    {
        public string NombreDocumento { get; set; }
        public int Estado { get; set; } // 1=Presente, 2=Faltante, 3=Incorrecto
    }

    public class IngresoFamiliar
    {
        public string NombreCompleto { get; set; }
        public string Parentesco { get; set; }
        public string Ocupacion { get; set; }
        public decimal IngresoMensual { get; set; }
    }

    public class GastoMensual
    {
        public string NombreGasto { get; set; }
        public decimal Monto { get; set; }
    }



    public class ResolucionBecaViewModel
    {
        public int IdResolucion { get; set; }
        public string CarnetEstudiante { get; set; }
        public int LineaPobrezaCUC { get; set; }
        public int SINIRUBE { get; set; }
        public int RESULTADO { get; set; }
        public int CategoriaBecaSocieconomicaAsignada { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaResolucion { get; set; }
        public string PersonaSupervisa { get; set; }
        public string PersonaAprueba { get; set; }
    }










    public class SolicitudConResolucionViewModel
    {
        // Datos de la solicitud
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaEstudiante { get; set; }
        public string Carrera { get; set; }
        public string TipoBeca { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int Estado { get; set; }

        
    }



    public class SolicitudViewModel
    {
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; }
        public string CedulaEstudiante { get; set; }
        public string Carrera { get; set; }
        public string TipoBeca { get; set; }
        public DateTime FechaSolicitud { get; set; }

        // Estado: 1=En revisión, 2=Doc. completa, 3=Falta doc., 4=En evaluación, 5=Aprobada, 6=Rechazada
        public int Estado { get; set; }

        // Resultado: 0=Rechazado, 1=Aprobado, null si aún no hay
        public int? Resultado { get; set; }

        public string CategoriaBecaSocioeconomicaAsignada { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string Observaciones { get; set; }
        public string PersonaSupervisa { get; set; }
        public string PersonaAprueba { get; set; }
    }

    public class ConvocatoriaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }






    public class BecaModelView
    {
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; } // Concatenación de Nombre + Apellidos
        public string CedulaEstudiante { get; set; } // Identificacion de Usuario
        public string Carrera { get; set; }
        public string TipoBeca { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int Estado { get; set; }
        public int? Resultado { get; set; }
        public string CategoriaBecaSocioeconomicaAsignada { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string Observaciones { get; set; }
        public string PersonaSupervisa { get; set; }
        public string PersonaAprueba { get; set; }


    }




}