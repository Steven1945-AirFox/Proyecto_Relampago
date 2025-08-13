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

        // Inyectamos IConfiguration para obtener la cadena de conexión
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("ProyectoRelampagoDB");
        }






        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult Becas() => View("~/Views/pages/becas.cshtml");
        public IActionResult Contacto() => View("~/Views/pages/contacto.cshtml");
        public IActionResult Documentos() => View("~/Views/pages/documentos.cshtml");
        public IActionResult Fomulario() => View("~/Views/pages/formulario.cshtml");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });









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
    }
}
