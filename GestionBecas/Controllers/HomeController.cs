using GestionBecas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

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

        public IActionResult Login()
        {
            ViewBag.Layout = null;
            return View("~/Views/Home/Login.cshtml");
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






















        #region Funciones para Convocatorias (ADO.NET básico)

        // Vistas de empleados (gestión de becas)
        public IActionResult EmpleadosConvocatorias()
        {
            ViewBag.Layout = null;

            try
            {
                var convocatorias = ObtenerTodasConvocatorias();
                ViewBag.DocumentosRequeridos = ObtenerDocumentosRequeridos();

                return View("~/Views/Home/Empleados/Convocatorias.cshtml", convocatorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar convocatorias");
                ViewBag.Error = "Error al cargar las convocatorias";
                return View("~/Views/Home/Empleados/Convocatorias.cshtml", new List<Convocatoria>());
            }
        }

        [HttpPost]
        public IActionResult CrearConvocatoria(string Nombre, string Descripcion, DateTime FechaInicio,
                                     DateTime FechaCierre, string TipoBeca, List<int> DocumentosRequeridos)
        {
            try
            {
                if (FechaCierre <= FechaInicio)
                {
                    TempData["ErrorMessage"] = "La fecha de cierre debe ser posterior a la fecha de inicio";
                    return RedirectToAction("EmpleadosConvocatorias");
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insertar convocatoria
                    var query = @"INSERT INTO Convocatorias 
                         (Nombre, Descripcion, FechaInicio, FechaCierre, TipoBeca, Estado)
                         VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaCierre, @TipoBeca, 'Activa');
                         SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        command.Parameters.AddWithValue("@Descripcion", Descripcion);
                        command.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                        command.Parameters.AddWithValue("@FechaCierre", FechaCierre);
                        command.Parameters.AddWithValue("@TipoBeca", TipoBeca);

                        var convocatoriaId = (int)command.ExecuteScalar();

                        // Insertar documentos requeridos
                        if (DocumentosRequeridos != null && DocumentosRequeridos.Any())
                        {
                            foreach (var docId in DocumentosRequeridos)
                            {
                                var docQuery = @"INSERT INTO ConvocatoriaDocumentos 
                                        (ConvocatoriaId, DocumentoId) 
                                        VALUES (@ConvocatoriaId, @DocumentoId)";

                                using (var docCommand = new SqlCommand(docQuery, connection))
                                {
                                    docCommand.Parameters.AddWithValue("@ConvocatoriaId", convocatoriaId);
                                    docCommand.Parameters.AddWithValue("@DocumentoId", docId);
                                    docCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                TempData["SuccessMessage"] = "Convocatoria creada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear convocatoria");
                TempData["ErrorMessage"] = "Error al crear la convocatoria. Detalle: " + ex.Message;
            }

            return RedirectToAction("EmpleadosConvocatorias");
        }



        // Método para ver convocatoria
        public IActionResult VerConvocatoria(int id)
        {
            return RedirectToAction("EmpleadosSolicitudes", new { convocatoriaId = id });
        }

        // Método para eliminar convocatoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConvocatoria(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Obtener información de la convocatoria que se va a eliminar
                            var convocatoria = new Convocatoria();
                            using (var getCommand = new SqlCommand(
                                "SELECT FechaInicio, FechaCierre FROM Convocatorias WHERE Id = @Id",
                                connection, transaction))
                            {
                                getCommand.Parameters.AddWithValue("@Id", id);
                                using (var reader = getCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        convocatoria.FechaInicio = reader.GetDateTime(0);
                                        convocatoria.FechaCierre = reader.GetDateTime(1);
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        TempData["ErrorMessage"] = "No se encontró la convocatoria";
                                        return RedirectToAction("EmpleadosConvocatorias");
                                    }
                                }
                            }

                            // 2. Eliminar resoluciones dentro del periodo de la convocatoria
                            using (var deleteResolucionesCommand = new SqlCommand(
                                @"DELETE FROM Resolucion 
                          WHERE FechaResolucion BETWEEN @FechaInicio AND @FechaCierre",
                                connection, transaction))
                            {
                                deleteResolucionesCommand.Parameters.AddWithValue("@FechaInicio", convocatoria.FechaInicio);
                                deleteResolucionesCommand.Parameters.AddWithValue("@FechaCierre", convocatoria.FechaCierre);
                                deleteResolucionesCommand.ExecuteNonQuery();
                            }

                            // 3. Eliminar documentos requeridos asociados
                            using (var deleteDocsCommand = new SqlCommand(
                                "DELETE FROM ConvocatoriaDocumentos WHERE ConvocatoriaId = @Id",
                                connection, transaction))
                            {
                                deleteDocsCommand.Parameters.AddWithValue("@Id", id);
                                deleteDocsCommand.ExecuteNonQuery();
                            }

                            // 4. Finalmente eliminar la convocatoria
                            using (var deleteCommand = new SqlCommand(
                                "DELETE FROM Convocatorias WHERE Id = @Id",
                                connection, transaction))
                            {
                                deleteCommand.Parameters.AddWithValue("@Id", id);
                                int affectedRows = deleteCommand.ExecuteNonQuery();

                                if (affectedRows > 0)
                                {
                                    transaction.Commit();
                                    TempData["SuccessMessage"] = "Convocatoria y resoluciones asociadas eliminadas correctamente";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    TempData["ErrorMessage"] = "No se pudo eliminar la convocatoria";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger.LogError(ex, "Error al eliminar convocatoria");
                            TempData["ErrorMessage"] = "Error al eliminar la convocatoria: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar convocatoria");
                TempData["ErrorMessage"] = "Error al eliminar la convocatoria: " + ex.Message;
            }

            return RedirectToAction("EmpleadosConvocatorias");
        }

        // Método para editar convocatoria
        [HttpPost]
        public IActionResult EditarConvocatoria(int Id, string Nombre, string Descripcion,
                                               DateTime FechaInicio, DateTime FechaCierre,
                                               string TipoBeca, string Estado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"
                UPDATE Convocatorias 
                SET Nombre = @Nombre, 
                    Descripcion = @Descripcion,
                    FechaInicio = @FechaInicio,
                    FechaCierre = @FechaCierre,
                    TipoBeca = @TipoBeca,
                    Estado = @Estado
                WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        command.Parameters.AddWithValue("@Descripcion", Descripcion);
                        command.Parameters.AddWithValue("@FechaInicio", FechaInicio);
                        command.Parameters.AddWithValue("@FechaCierre", FechaCierre);
                        command.Parameters.AddWithValue("@TipoBeca", TipoBeca);
                        command.Parameters.AddWithValue("@Estado", Estado);

                        command.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Convocatoria actualizada correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar convocatoria");
                TempData["ErrorMessage"] = "Error al editar la convocatoria";
            }
            return RedirectToAction("EmpleadosConvocatorias");
        }

        #endregion







        #region Métodos auxiliares (ADO.NET básico)

        private List<Convocatoria> ObtenerTodasConvocatorias()
        {
            var convocatorias = new List<Convocatoria>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Simple query to get just the convocatorias
                    var query = "SELECT Id, Nombre, Descripcion, FechaInicio, FechaCierre, TipoBeca, Estado FROM Convocatorias ORDER BY FechaInicio DESC";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            convocatorias.Add(new Convocatoria
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                FechaInicio = reader.GetDateTime(3),
                                FechaCierre = reader.GetDateTime(4),
                                TipoBeca = reader.GetString(5),
                                Estado = reader.GetString(6),
                                TotalSolicitudes = 0 // We'll implement this later when needed
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener convocatorias");
                // Return empty list if there's an error
                return new List<Convocatoria>();
            }

            return convocatorias;
        }

        private List<DocumentoRequerido> ObtenerDocumentosRequeridos()
        {
            var documentos = new List<DocumentoRequerido>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT IdDocumento, Nombre FROM Documentos";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        documentos.Add(new DocumentoRequerido
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }

            return documentos;
        }

        #endregion









        // Add this test action to your HomeController
        public IActionResult TestConvocatorias()
        {
            try
            {
                var convocatorias = ObtenerTodasConvocatorias();
                return Json(new
                {
                    Success = true,
                    Count = convocatorias.Count,
                    Data = convocatorias
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }


























        //STEVEN SOLICITUDES

        // Método para mostrar solicitudes
        // Método para mostrar solicitudes

        public IActionResult EmpleadosSolicitudes(int convocatoriaId = 0, int estado = 0, string busqueda = "", int pagina = 1, int pageSize = 20)
        {
            ViewBag.ConvocatoriaSeleccionada = convocatoriaId;
            ViewBag.EstadoSeleccionado = estado;
            ViewBag.Busqueda = busqueda;

            List<BecaModelView> lista = new List<BecaModelView>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Main query - simplified since Convocatorias isn't directly related to Resolucion
                string query = @"
            SELECT 
                r.IdResolucion AS IdSolicitud,
                u.Nombre + ' ' + u.Apellido1 + ' ' + ISNULL(u.Apellido2, '') AS NombreEstudiante,
                u.Identificacion AS CedulaEstudiante,
                e.Carrera,
                'Beca Socioeconómica' AS TipoBeca, -- Hardcoded since no convocatoria relation
                r.FechaResolucion AS FechaSolicitud,
                r.RESULTADO AS Resultado,
                CASE 
                    WHEN r.CategoriaBecaSocieconomicaAsignada = 1 THEN 'Beca 0'
                    WHEN r.CategoriaBecaSocieconomicaAsignada = 2 THEN 'Beca 1'
                    WHEN r.CategoriaBecaSocieconomicaAsignada = 3 THEN 'Beca 2'
                    WHEN r.CategoriaBecaSocieconomicaAsignada = 4 THEN 'Beca 3'
                    ELSE '-'
                END AS CategoriaBecaSocioeconomicaAsignada,
                r.FechaResolucion,
                r.Observaciones,
                r.PersonaSupervisa,
                r.PersonaAprueba,
                CASE 
                    WHEN r.RESULTADO IS NULL THEN 1 -- Pendiente
                    WHEN r.RESULTADO = 1 THEN 5      -- Aprobada
                    WHEN r.RESULTADO = 0 THEN 6      -- Rechazada
                    ELSE 1                          -- Default to Pendiente
                END AS Estado
            FROM Resolucion r
            INNER JOIN Estudiante_Usuario e ON r.carnetEstudiante = e.carnet
            INNER JOIN Usuario u ON e.idUsuario = u.idusuario
            WHERE (@Estado = 0 OR 
                  (@Estado = 1 AND r.RESULTADO IS NULL) OR -- Pendiente
                  (@Estado = 5 AND r.RESULTADO = 1) OR     -- Aprobada
                  (@Estado = 6 AND r.RESULTADO = 0))       -- Rechazada
              AND (@Busqueda = '' OR 
                  u.Nombre LIKE '%' + @Busqueda + '%' OR 
                  u.Apellido1 LIKE '%' + @Busqueda + '%' OR 
                  u.Identificacion LIKE '%' + @Busqueda + '%')
            ORDER BY r.IdResolucion DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@Busqueda", busqueda ?? "");
                    cmd.Parameters.AddWithValue("@Offset", (pagina - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new BecaModelView
                            {
                                IdSolicitud = (int)reader["IdSolicitud"],
                                NombreEstudiante = reader["NombreEstudiante"].ToString(),
                                CedulaEstudiante = reader["CedulaEstudiante"].ToString(),
                                Carrera = reader["Carrera"].ToString(),
                                TipoBeca = reader["TipoBeca"].ToString(),
                                FechaSolicitud = (DateTime)reader["FechaSolicitud"],
                                Resultado = reader["Resultado"] as int?,
                                CategoriaBecaSocioeconomicaAsignada = reader["CategoriaBecaSocioeconomicaAsignada"]?.ToString(),
                                FechaResolucion = reader["FechaResolucion"] as DateTime?,
                                Observaciones = reader["Observaciones"]?.ToString(),
                                PersonaSupervisa = reader["PersonaSupervisa"]?.ToString(),
                                PersonaAprueba = reader["PersonaAprueba"]?.ToString(),
                                Estado = (int)reader["Estado"]
                            });
                        }
                    }
                }
            }

            // Load Convocatorias for filter (simplified since no direct relation)
            ViewBag.Convocatorias = new List<dynamic>
    {
        new { Id = 1, Nombre = "Beca Socioeconómica 2025" }
    };

            return View("~/Views/Home/Empleados/Solicitudes.cshtml", lista);
        }











        // Método para mostrar detalle de solicitud
        public IActionResult EmpleadosDetalleSolicitud(int id)
        {
            try
            {
                var detalle = new DetalleSolicitud
                {
                    Documentos = new List<DocumentoSolicitud>(),
                    IngresosFamiliares = new List<IngresoFamiliar>(),
                    GastosMensuales = new List<GastoMensual>()
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 1. Get basic student and application info
                    var basicInfoQuery = @"
                SELECT 
                    r.IdResolucion,
                    u.Nombre + ' ' + u.Apellido1 + ' ' + ISNULL(u.Apellido2, '') AS NombreEstudiante,
                    u.Identificacion AS CedulaEstudiante,
                    e.carnet AS CarnetEstudiante,
                    e.FechaNacimiento,
                    e.Edad,
                    CASE e.Genero WHEN 1 THEN 'Masculino' WHEN 2 THEN 'Femenino' ELSE 'Otro' END AS Genero,
                    e.EstadoCivil,
                    u.Telefono,
                    u.Correo,
                    e.DireccionExacta,
                    e.Provincia,
                    e.Canton,
                    e.Distrito,
                    CASE WHEN EXISTS (SELECT 1 FROM TipoViviendaEstudiante WHERE carnetEstudiante = e.carnet) 
                         THEN (SELECT TOP 1 
                               CASE TipoVivienda 
                                   WHEN 1 THEN 'Alquilada' 
                                   WHEN 2 THEN 'Prestada' 
                                   WHEN 3 THEN 'Propia con hipoteca' 
                                   WHEN 4 THEN 'Propia sin hipoteca' 
                                   ELSE 'Desconocido' 
                               END 
                               FROM TipoViviendaEstudiante WHERE carnetEstudiante = e.carnet)
                         ELSE 'No especificado' END AS TipoVivienda,
                    ISNULL((SELECT TOP 1 MetrosCuadrados FROM TipoViviendaEstudiante WHERE carnetEstudiante = e.carnet), 0) AS MetrosCuadrados,
                    e.Carrera,
                    e.NivelCarrera,
                    ISNULL(da.NombreColegio, '') AS NombreColegio,
                    CASE WHEN da.TipoInstitucion IS NULL THEN '' 
                         WHEN da.TipoInstitucion = 1 THEN 'Pública' 
                         WHEN da.TipoInstitucion = 2 THEN 'Privada' 
                         ELSE 'Desconocido' END AS TipoInstitucion,
                    'Beca Socioeconómica' AS NombreConvocatoria,
                    'Beca Socioeconómica' AS TipoBeca,
                    r.FechaResolucion AS FechaSolicitud,
                    CASE 
                        WHEN r.RESULTADO IS NULL THEN 1 -- Pendiente
                        WHEN r.RESULTADO = 1 THEN 5    -- Aprobada
                        WHEN r.RESULTADO = 0 THEN 6    -- Rechazada
                        ELSE 1                        -- Default
                    END AS Estado
                FROM Resolucion r
                INNER JOIN Estudiante_Usuario e ON r.carnetEstudiante = e.carnet
                INNER JOIN Usuario u ON e.idUsuario = u.idusuario
                LEFT JOIN Datos_academicos da ON e.carnet = da.carnetEstudiante
                WHERE r.IdResolucion = @Id";

                    using (var command = new SqlCommand(basicInfoQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                detalle = new DetalleSolicitud
                                {
                                    IdSolicitud = reader.GetInt32(0),
                                    NombreEstudiante = reader.GetString(1),
                                    CedulaEstudiante = reader.GetString(2),
                                    CarnetEstudiante = reader.GetString(3),
                                    FechaNacimiento = reader.GetDateTime(4),
                                    Edad = reader.GetInt32(5),
                                    Genero = reader.GetString(6),
                                    EstadoCivil = reader.GetString(7),
                                    Telefono = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    Correo = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                    DireccionExacta = reader.GetString(10),
                                    Provincia = reader.GetString(11),
                                    Canton = reader.GetString(12),
                                    Distrito = reader.GetString(13),
                                    TipoVivienda = reader.GetString(14),
                                    MetrosCuadrados = reader.GetInt32(15),
                                    Carrera = reader.GetString(16),
                                    NivelCarrera = reader.GetString(17),
                                    NombreColegio = reader.GetString(18),
                                    TipoInstitucion = reader.GetString(19),
                                    NombreConvocatoria = reader.GetString(20),
                                    TipoBeca = reader.GetString(21),
                                    FechaSolicitud = reader.GetDateTime(22),
                                    Estado = reader.GetInt32(23),
                                    Documentos = new List<DocumentoSolicitud>(),
                                    IngresosFamiliares = new List<IngresoFamiliar>(),
                                    GastosMensuales = new List<GastoMensual>()
                                };
                            }
                            else
                            {
                                _logger.LogWarning($"No se encontró la solicitud con ID {id}");
                                ViewBag.Error = "No se encontró la solicitud";
                                return View("~/Views/Home/Empleados/DetalleSolicitud.cshtml", detalle);
                            }
                        }
                    }

                    // 2. Get documents
                    try
                    {
                        var docQuery = @"
                    SELECT d.Nombre, de.Estado
                    FROM Documentos_Estudiante de
                    JOIN Documentos d ON de.idDocumento = d.IdDocumento
                    WHERE de.carnetEstudiante = @Carnet";

                        using (var command = new SqlCommand(docQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Carnet", detalle.CarnetEstudiante);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    detalle.Documentos.Add(new DocumentoSolicitud
                                    {
                                        NombreDocumento = reader.GetString(0),
                                        Estado = reader.GetInt32(1)
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception docEx)
                    {
                        _logger.LogError(docEx, $"Error al cargar documentos para solicitud {id}");
                        detalle.Documentos = new List<DocumentoSolicitud>();
                    }

                    // 3. Get family income
                    try
                    {
                        var incomeQuery = @"
                    SELECT 
                        NombrePariente + ' ' + Apellido1Pariente + ' ' + ISNULL(Apellido2Pariente, '') AS NombreCompleto,
                        ParentezcoConEstudiante,
                        Ocupacion,
                        IngresoMensualBruto
                    FROM Ingresos_Estudiante
                    WHERE carnetEstudiante = @Carnet";

                        using (var command = new SqlCommand(incomeQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Carnet", detalle.CarnetEstudiante);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    detalle.IngresosFamiliares.Add(new IngresoFamiliar
                                    {
                                        NombreCompleto = reader.GetString(0),
                                        Parentesco = reader.GetString(1),
                                        Ocupacion = reader.GetString(2),
                                        IngresoMensual = Convert.ToDecimal(reader.GetDouble(3))
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception incomeEx)
                    {
                        _logger.LogError(incomeEx, $"Error al cargar ingresos familiares para solicitud {id}");
                        detalle.IngresosFamiliares = new List<IngresoFamiliar>();
                    }

                    // 4. Get monthly expenses
                    try
                    {
                        var expensesQuery = @"
                    SELECT r.Nombre, re.Monto
                    FROM Recibos_Estudiante re
                    JOIN Recibos r ON re.idRecibo = r.IdRecibo
                    WHERE re.carnetEstudiante = @Carnet";

                        using (var command = new SqlCommand(expensesQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Carnet", detalle.CarnetEstudiante);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    detalle.GastosMensuales.Add(new GastoMensual
                                    {
                                        NombreGasto = reader.GetString(0),
                                        Monto = reader.GetDecimal(1)
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception expensesEx)
                    {
                        _logger.LogError(expensesEx, $"Error al cargar gastos mensuales para solicitud {id}");
                        detalle.GastosMensuales = new List<GastoMensual>();
                    }
                }

                ViewBag.Layout = null;
                return View("~/Views/Home/Empleados/DetalleSolicitud.cshtml", detalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalle de solicitud {id}");
                ViewBag.Error = "Error al cargar el detalle de la solicitud";
                return View("~/Views/Home/Empleados/DetalleSolicitud.cshtml", new DetalleSolicitud
                {
                    Documentos = new List<DocumentoSolicitud>(),
                    IngresosFamiliares = new List<IngresoFamiliar>(),
                    GastosMensuales = new List<GastoMensual>()
                });
            }
        }




        public IActionResult EmpleadosEvaluaciones()
        {
            var model = new EvaluacionViewModel();

            using (var conn = new SqlConnection(_connectionString))
            {
                string sql = @"
SELECT 
    r.IdResolucion AS IdSolicitud,
    u.Nombre + ' ' + u.Apellido1 + ' ' + ISNULL(u.Apellido2, '') AS NombreEstudiante,
    e.Carrera,
    da.BecaUnivarsitaria,
    da.TipoInstitucion,
    e.carnet AS CarnetEstudiante,
    ISNULL((SELECT SUM(IngresoMensualBruto) FROM Ingresos_Estudiante WHERE carnetEstudiante = e.carnet), 0) AS IngresosFamiliares,
    ISNULL((SELECT SUM(Monto) FROM Recibos_Estudiante WHERE carnetEstudiante = e.carnet), 0) AS GastosFamiliares,
    ISNULL((SELECT COUNT(*) FROM Ingresos_Estudiante WHERE carnetEstudiante = e.carnet), 1) AS MiembrosFamilia,
    CASE 
        WHEN r.RESULTADO = 1 THEN 'Aprobado'
        WHEN r.RESULTADO = 0 THEN 'Rechazado'
        ELSE 'Solicitud pendiente de evaluación'
    END AS EstadoSolicitud
FROM Resolucion r
INNER JOIN Estudiante_Usuario e ON r.carnetEstudiante = e.carnet
INNER JOIN Usuario u ON e.idUsuario = u.idusuario
LEFT JOIN Datos_academicos da ON e.carnet = da.carnetEstudiante
ORDER BY r.FechaResolucion DESC;
";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Datos básicos
                            var solicitud = new EvaluacionSolicitud
                            {
                                IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                                NombreEstudiante = reader["NombreEstudiante"].ToString(),
                                Carrera = reader["Carrera"].ToString(),
                                CarnetEstudiante = reader["CarnetEstudiante"].ToString(),
                                EstadoSolicitud = reader["EstadoSolicitud"].ToString()
                            };

                            // Ingreso per cápita diario
                            decimal ingresosFamiliares = Convert.ToDecimal(reader["IngresosFamiliares"]);
                            decimal egresosFamiliares = Convert.ToDecimal(reader["GastosFamiliares"]);
                            int miembrosFamilia = Convert.ToInt32(reader["MiembrosFamilia"]);

                            decimal ingresoPerCapita = 0;
                            if (miembrosFamilia > 0)
                                ingresoPerCapita = (ingresosFamiliares - egresosFamiliares) / miembrosFamilia / 30;

                            solicitud.IngresoPerCapita = ingresoPerCapita;

                            // Puntaje socioeconómico (ejemplo simple)
                            if (ingresoPerCapita < 5000) solicitud.PuntajeSocioeconomico = 40;
                            else if (ingresoPerCapita < 10000) solicitud.PuntajeSocioeconomico = 30;
                            else if (ingresoPerCapita < 20000) solicitud.PuntajeSocioeconomico = 20;
                            else solicitud.PuntajeSocioeconomico = 10;

                            // Puntaje académico
                            decimal promedio = 0;
                            if (Convert.ToBoolean(reader["BecaUnivarsitaria"])) promedio = 85;
                            else if (Convert.ToInt32(reader["TipoInstitucion"]) == 1) promedio = 80;
                            else if (Convert.ToInt32(reader["TipoInstitucion"]) == 2) promedio = 75;
                            else promedio = 75;

                            solicitud.Promedio = promedio;

                            if (promedio >= 85) solicitud.PuntajeAcademico = 40;
                            else if (promedio >= 80) solicitud.PuntajeAcademico = 30;
                            else if (promedio >= 75) solicitud.PuntajeAcademico = 20;
                            else solicitud.PuntajeAcademico = 10;

                            // Puntaje total y recomendación
                            solicitud.PuntajeTotal = solicitud.PuntajeSocioeconomico + solicitud.PuntajeAcademico;

                            if (solicitud.PuntajeTotal >= 70) solicitud.Recomendacion = "Beca 0";
                            else if (solicitud.PuntajeTotal >= 60) solicitud.Recomendacion = "Beca 1";
                            else if (solicitud.PuntajeTotal >= 50) solicitud.Recomendacion = "Beca 2";
                            else solicitud.Recomendacion = "Pendiente";

                            model.Solicitudes.Add(solicitud);
                        }
                    }
                }
            }

            // Totales del modelo
            model.TotalSolicitudes = model.Solicitudes.Count;
            model.SolicitudesPendientes = model.Solicitudes.Count(s => s.EstadoSolicitud == "Solicitud pendiente de evaluación");
            model.PresupuestoTotal = 10000000; // ejemplo, puedes reemplazar con tu cálculo real
            model.PresupuestoAsignado = model.Solicitudes.Sum(s => s.PuntajeTotal >= 60 ? 100000 : 0); // ejemplo simple

            return View("~/Views/Home/empleados/evaluaciones.cshtml", model);
        }














        [HttpPost]
        public IActionResult ProcesarEvaluacion(int idSolicitud, string accion, string comentarios)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Obtener datos necesarios para determinar la categoría
                    var datosQuery = @"
                SELECT 
                    ISNULL((SELECT SUM(IngresoMensualBruto) FROM Ingresos_Estudiante ie 
                           JOIN Resolucion r ON ie.carnetEstudiante = r.carnetEstudiante 
                           WHERE r.IdResolucion = @Id), 0) AS Ingresos,
                    ISNULL((SELECT SUM(Monto) FROM Recibos_Estudiante re 
                           JOIN Resolucion r ON re.carnetEstudiante = r.carnetEstudiante 
                           WHERE r.IdResolucion = @Id), 0) AS Gastos,
                    ISNULL((SELECT COUNT(*) FROM Ingresos_Estudiante ie 
                           JOIN Resolucion r ON ie.carnetEstudiante = r.carnetEstudiante 
                           WHERE r.IdResolucion = @Id), 1) AS Miembros,
                    -- Usar Datos_academicos para estimar el promedio
                    CASE 
                        WHEN da.BecaUnivarsitaria = 1 THEN 85.0
                        WHEN da.TipoInstitucion = 1 THEN 80.0
                        ELSE 75.0
                    END AS Promedio
                FROM Resolucion r
                LEFT JOIN Datos_academicos da ON r.carnetEstudiante = da.carnetEstudiante
                WHERE r.IdResolucion = @Id";

                    decimal ingresos = 0, gastos = 0, promedio = 75;
                    int miembros = 1;

                    using (var command = new SqlCommand(datosQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", idSolicitud);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ingresos = reader.GetDecimal(0);
                                gastos = reader.GetDecimal(1);
                                miembros = reader.GetInt32(2);
                                promedio = reader.GetDecimal(3);
                            }
                        }
                    }

                    // Calcular recomendación
                    var ingresoPerCapita = miembros > 0 ? ((ingresos - gastos) / miembros) / 30 : 0;
                    var puntajeSocio = CalcularPuntajeSocioeconomico(ingresoPerCapita);
                    var puntajeAcad = CalcularPuntajeAcademico(promedio);
                    var puntajeTotal = (puntajeSocio + puntajeAcad) / 2;
                    var recomendacion = DeterminarRecomendacion(puntajeTotal);

                    // Actualizar la resolución
                    var updateQuery = @"
                UPDATE Resolucion 
                SET RESULTADO = @Resultado,
                    CategoriaBecaSocieconomicaAsignada = @Categoria,
                    Observaciones = @Observaciones,
                    PersonaSupervisa = @Usuario,
                    FechaResolucion = GETDATE()
                WHERE IdResolucion = @Id";

                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", idSolicitud);
                        command.Parameters.AddWithValue("@Resultado", accion == "aprobar" ? 1 : 0);
                        command.Parameters.AddWithValue("@Categoria", GetCategoriaBeca(recomendacion));
                        command.Parameters.AddWithValue("@Observaciones", comentarios);
                        command.Parameters.AddWithValue("@Usuario", User.Identity.Name);

                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = $"Solicitud {(accion == "aprobar" ? "aprobada" : "rechazada")} correctamente";
                return RedirectToAction("EmpleadosEvaluaciones");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar evaluación {idSolicitud}");
                TempData["ErrorMessage"] = "Error al procesar la evaluación: " + ex.Message;
                return RedirectToAction("EmpleadosEvaluaciones");
            }
        }










        // --- Helpers que ya tenías ---
        private int CalcularPuntajeSocioeconomico(decimal ingresoPerCapita)
        {
            if (ingresoPerCapita < 500) return 100;
            if (ingresoPerCapita < 1000) return 80;
            if (ingresoPerCapita < 1500) return 60;
            if (ingresoPerCapita < 2000) return 40;
            return 20;
        }

        private int CalcularPuntajeAcademico(decimal promedio)
        {
            if (promedio >= 90) return 100;
            if (promedio >= 80) return 85;
            if (promedio >= 70) return 70;
            if (promedio >= 60) return 55;
            return 40;
        }

        private string DeterminarRecomendacion(decimal puntajeTotal)
        {
            if (puntajeTotal >= 90) return "Beca 0";
            if (puntajeTotal >= 75) return "Beca 1";
            if (puntajeTotal >= 60) return "Beca 2";
            if (puntajeTotal >= 45) return "Beca 3";
            return "No Recomendada";
        }

        private int GetCategoriaBeca(string recomendacion)
        {
            switch (recomendacion)
            {
                case "Beca 0": return 1;
                case "Beca 1": return 2;
                case "Beca 2": return 3;
                case "Beca 3": return 4;
                default: return 0;
            }
        }





























    }

        //FIN STEVEN SOLICITUDES


        #endregion
}