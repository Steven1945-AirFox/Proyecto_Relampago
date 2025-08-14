namespace GestionBecas.Models
{
    public class FormularioBeca
    {
        public string AnoAnterior { get; set; }
        public string AreaConstruccion { get; set; }
        public string Banos { get; set; }
        public string BecaColegio { get; set; }
        public string BecaTitulo { get; set; }
        public string Canton { get; set; }
        public string CantonEstudios { get; set; }
        public string Carrera { get; set; }
        public string Categoria { get; set; }
        public string Cedula { get; set; }
        public string CedulaDeclaracion { get; set; }
        public string Cocina { get; set; }
        public string ColegioConcluyo { get; set; }
        public string Comedor { get; set; }
        public string Condicion { get; set; }
        public string CuatrimestreActual { get; set; }
        public string DireccionExacta { get; set; }
        public string DireccionExactaEstudios { get; set; }
        public string Distrito { get; set; }
        public string DistritoEstudios { get; set; }
        public string Dormitorios { get; set; }
        public string Edad { get; set; }
        public string EgresoAlquiler { get; set; }
        public string EgresoEducacion { get; set; }
        public string EgresoHipoteca { get; set; }
        public string EgresoPensionAlimentaria { get; set; }
        public string EgresoSaludPrivada { get; set; }
        public string Email { get; set; }
        public string EstadoCivil { get; set; }

        // Campos de familiares
        public List<string> FamiliarApellido1 { get; set; }
        public List<string> FamiliarApellido2 { get; set; }
        public List<string> FamiliarBeca { get; set; }
        public List<string> FamiliarCedula { get; set; }
        public List<string> FamiliarEdad { get; set; }
        public List<string> FamiliarEstadoCivil { get; set; }
        public List<string> FamiliarEstudia { get; set; }
        public List<string> FamiliarIngreso { get; set; }
        public List<string> FamiliarInstitucion { get; set; }
        public List<string> FamiliarNombre { get; set; }
        public List<string> FamiliarOcupacion { get; set; }
        public List<string> FamiliarParentesco { get; set; }
        public List<string> FamiliarSalud { get; set; }

        public DateTime FechaCompromiso { get; set; }
        public DateTime FechaDeclaracion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string FirmaCanvas { get; set; }
        public string FirmaCompromisoCanvas { get; set; }
        public string FuenteIngresos { get; set; }
        public string Genero { get; set; }
        public string GradoTitulo { get; set; }
        public string InstitucionBeca { get; set; }
        public string LineaPobrezaCuc { get; set; }
        public string MontoAlquiler { get; set; }
        public string MontoBecaColegio { get; set; }
        public string MontoBecaTitulo { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public string OtrosAposentos { get; set; }
        public string OtrosIngresosDetalle { get; set; }
        public string PrimerApellido { get; set; }
        public string Provincia { get; set; }
        public string ProvinciaEstudios { get; set; }
        public string Resultado { get; set; }
        public string RevisadoPor { get; set; }
        public string Sala { get; set; }
        public string SegundoApellido { get; set; }
        public string Sinirube { get; set; }
        public string SupervisadoPor { get; set; }
        public string TelefonoCelular { get; set; }
        public string TelefonoResidencia { get; set; }
        public string TenenciaVivienda { get; set; }
        public string TipoBecaAnterior { get; set; }
        public string TipoInstitucion { get; set; }
        public string TitulosPrevios { get; set; }
        public string UltimoCuatrimestre { get; set; }
    }

}
