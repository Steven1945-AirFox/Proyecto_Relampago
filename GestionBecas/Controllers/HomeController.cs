using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using GestionBecas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GestionBecas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("ProyectoRelampagoDB");
        }

        // Vistas públicas
        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult Becas() => View("~/Views/pages/becas.cshtml");
        public IActionResult Contacto() => View("~/Views/pages/contacto.cshtml");
        public IActionResult Documentos() => View("~/Views/pages/documentos.cshtml");
        public IActionResult Formulario() => View("~/Views/pages/formulario.cshtml");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });













        #region Steven
        //
        //
        //Steven
        //
        //


        // Vistas de empleados (gestión de becas)
        public IActionResult EmpleadosConvocatorias()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Convocatorias.cshtml");
        }

        public IActionResult EmpleadosSolicitudes()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Solicitudes.cshtml");
        }

        public IActionResult EmpleadosDetalleSolicitud(int id)
        {
            ViewBag.Layout = null;
            ViewBag.SolicitudId = id;
            return View("~/Views/Home/Empleados/DetalleSolicitud.cshtml");
        }

        public IActionResult EmpleadosEvaluaciones()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Evaluaciones.cshtml");
        }

        public IActionResult EmpleadosBeneficiarios()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Beneficiarios.cshtml");
        }

        public IActionResult EmpleadosSeguimiento()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Seguimiento.cshtml");
        }










        
        public IActionResult EmpleadosDashboard()
        {
            var model = new GestorBecas_ViewModel();

            try
            {
                // Obtener el período vigente
                model.GestorBecas_PeriodoVigente = ObtenerPeriodoVigente();

                // Obtener estadísticas de solicitudes
                var estadisticas = ObtenerEstadisticasSolicitudes();

                // Asignar datos a ViewBag para la vista
                ViewBag.TotalSolicitudes = estadisticas.TotalSolicitudes;
                ViewBag.EnRevision = estadisticas.EnRevision;
                ViewBag.Aprobadas = estadisticas.Aprobadas;
                ViewBag.Rechazadas = estadisticas.Rechazadas;
                ViewBag.SolicitudesEstaSemana = estadisticas.SolicitudesEstaSemana;

                // Calcular porcentajes
                ViewBag.PorcentajeNuevas = estadisticas.TotalSolicitudes > 0 ?
                    (estadisticas.SolicitudesEstaSemana * 100 / estadisticas.TotalSolicitudes) : 0;
                ViewBag.PorcentajeRevision = estadisticas.TotalSolicitudes > 0 ?
                    (estadisticas.EnRevision * 100 / estadisticas.TotalSolicitudes) : 0;
                ViewBag.PorcentajeAprobadas = estadisticas.TotalSolicitudes > 0 ?
                    (estadisticas.Aprobadas * 100 / estadisticas.TotalSolicitudes) : 0;
                ViewBag.PorcentajeRechazadas = estadisticas.TotalSolicitudes > 0 ?
                    (estadisticas.Rechazadas * 100 / estadisticas.TotalSolicitudes) : 0;

                // Obtener alertas
                ViewBag.DocumentosFaltantes = ObtenerCantidadDocumentosFaltantes();
                ViewBag.PlazosCriticos = ObtenerSolicitudesPlazosCriticos();
                ViewBag.EvaluacionesCompletadas = ObtenerEvaluacionesCompletadasHoy();
                ViewBag.SolicitudesPrioritarias = ObtenerSolicitudesPrioritarias();

                // Obtener próximas actividades
                ViewBag.ProximasActividades = ObtenerProximasActividades();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard de empleados");
                ViewBag.Error = "Ocurrió un error al cargar los datos del dashboard";
            }

            ViewBag.Layout = null;
            return View("~/Views/Home/Empleados/Dashboard.cshtml", model);
        }

        // Acción para probar la conexión
        public IActionResult TestConexion()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                }
                return Content("Conexión exitosa a la base de datos");
            }
            catch (Exception ex)
            {
                return Content($"Error en la conexión: {ex.Message}");
            }
        }

       

        private GestorBecas_PeriodoRecepcion ObtenerPeriodoVigente()
        {
            var periodo = new GestorBecas_PeriodoRecepcion();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT TOP 1 Id, FechaInicio, FechaFinalizacion, Estado, Descripcion 
                                       FROM PeriodosRecepcion 
                                       WHERE FechaFinalizacion >= @Hoy 
                                       ORDER BY FechaInicio";
                command.Parameters.AddWithValue("@Hoy", DateTime.Today);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        periodo.GestorBecas_Id = reader.GetInt32(0);
                        periodo.GestorBecas_FechaInicio = reader.GetDateTime(1);
                        periodo.GestorBecas_FechaFinalizacion = reader.GetDateTime(2);
                        periodo.GestorBecas_Estado = reader.GetInt32(3);
                        periodo.GestorBecas_Descripcion = reader.GetString(4);
                    }
                }
            }

            return periodo;
        }

        private (int TotalSolicitudes, int EnRevision, int Aprobadas, int Rechazadas, int SolicitudesEstaSemana) ObtenerEstadisticasSolicitudes()
        {
            int total = 0, enRevision = 0, aprobadas = 0, rechazadas = 0, estaSemana = 0;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT 
                        COUNT(*) AS Total,
                        SUM(CASE WHEN Estado = 1 THEN 1 ELSE 0 END) AS EnRevision,
                        SUM(CASE WHEN Estado = 2 THEN 1 ELSE 0 END) AS Aprobadas,
                        SUM(CASE WHEN Estado = 3 THEN 1 ELSE 0 END) AS Rechazadas,
                        SUM(CASE WHEN FechaCreacion >= DATEADD(day, -7, GETDATE()) THEN 1 ELSE 0 END) AS EstaSemana
                      FROM SolicitudesBecas";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        total = reader.GetInt32(0);
                        enRevision = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        aprobadas = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        rechazadas = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        estaSemana = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                    }
                }
            }

            return (total, enRevision, aprobadas, rechazadas, estaSemana);
        }

        private int ObtenerCantidadDocumentosFaltantes()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT COUNT(DISTINCT s.Id)
                                      FROM SolicitudesBecas s
                                      JOIN DocumentosSolicitud d ON s.Id = d.SolicitudId
                                      WHERE s.Estado = 1 AND d.Estado = 2";

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int ObtenerSolicitudesPlazosCriticos()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT COUNT(*)
                                      FROM SolicitudesBecas s
                                      JOIN PeriodosRecepcion p ON s.PeriodoId = p.Id
                                      WHERE s.Estado = 1 AND DATEDIFF(day, GETDATE(), p.FechaFinalizacion) <= 3";

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int ObtenerEvaluacionesCompletadasHoy()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT COUNT(*)
                                      FROM Evaluaciones
                                      WHERE CONVERT(date, FechaEvaluacion) = CONVERT(date, GETDATE())";

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int ObtenerSolicitudesPrioritarias()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT COUNT(*)
                                      FROM SolicitudesBecas
                                      WHERE Prioridad = 1 AND Estado = 1";

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private List<ActividadDashboard> ObtenerProximasActividades()
        {
            var actividades = new List<ActividadDashboard>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = @"SELECT Nombre, Fecha
                                      FROM ActividadesBecas
                                      WHERE Fecha >= GETDATE() AND Fecha <= DATEADD(day, 30, GETDATE())
                                      ORDER BY Fecha";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actividades.Add(new ActividadDashboard
                        {
                            Nombre = reader.GetString(0),
                            Fecha = reader.GetDateTime(1)
                        });
                    }
                }
            }

            return actividades;
        }

        // Clase auxiliar para las actividades
        public class ActividadDashboard
        {
            public string Nombre { get; set; }
            public DateTime Fecha { get; set; }
        }

        #endregion
    }
}