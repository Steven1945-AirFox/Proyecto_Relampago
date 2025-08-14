namespace GestionBecas.Models
{
    public class EvaluacionViewModel
    {
        public List<EvaluacionSolicitud> Solicitudes { get; set; } = new List<EvaluacionSolicitud>();
        public decimal PresupuestoTotal { get; set; }
        public decimal PresupuestoAsignado { get; set; }
        public int SolicitudesPendientes { get; set; }
        public int TotalSolicitudes { get; set; }

        // Add these computed properties
        public decimal PresupuestoAsignadoPorcentaje =>
            PresupuestoTotal != 0 ? (PresupuestoAsignado / PresupuestoTotal * 100) : 0;

        public double SolicitudesPendientesPorcentaje =>
            TotalSolicitudes != 0 ? (SolicitudesPendientes / (double)TotalSolicitudes * 100) : 0;







    }

    public class EvaluacionSolicitud
    {
        public int IdSolicitud { get; set; }
        public string NombreEstudiante { get; set; }
        public string Carrera { get; set; }
        public decimal Promedio { get; set; }
        public decimal IngresoPerCapita { get; set; }
        public int PuntajeSocioeconomico { get; set; }
        public int PuntajeAcademico { get; set; }
        public decimal PuntajeTotal { get; set; }
        public string Recomendacion { get; set; }
        public string CarnetEstudiante { get; set; }

        // ESTA ES LA NUEVA
        public string EstadoSolicitud { get; set; }


    }




   






}
