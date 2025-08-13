using System.Diagnostics;
using GestionBecas.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionBecas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Becas()
        {
            return View("~/Views/pages/becas.cshtml");
        }

        public IActionResult Contacto()
        {
            return View("~/Views/pages/contacto.cshtml");
        }
        public IActionResult Documentos()
        {
            return View("~/Views/pages/documentos.cshtml");
        }
        public IActionResult Fomulario()
        {
            return View("~/Views/pages/formulario.cshtml");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
