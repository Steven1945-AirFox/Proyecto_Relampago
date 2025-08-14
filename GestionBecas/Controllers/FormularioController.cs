using GestionBecas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionBecas.Controllers
{
    public class FormularioController : Controller
    {
        private readonly ProyectoRelampagoContext _context;

        public FormularioController(ProyectoRelampagoContext context)
        { 
             _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GuardarFormulario([FromBody] FormularioBeca modelo)
        {
            if (modelo == null)
                return BadRequest("Formulario vacío");

            try
            {
                // 1️⃣ Guardar imágenes físicas
                GuardarImagenBase64(modelo.FuenteIngresos, modelo.Cedula, "fuente-ingreso");
                GuardarImagenBase64(modelo.FirmaCompromisoCanvas, modelo.Cedula, "compromiso");

                // 2️⃣ Insertar o actualizar EstudianteUsuario
                var estudiante = await _context.EstudianteUsuario
                    .FirstOrDefaultAsync(e => e.Carnet == modelo.Cedula);

                if (estudiante == null)
                {
                    estudiante = new EstudianteUsuario
                    {
                        Carnet = modelo.Cedula,
                        Carrera = modelo.Carrera,
                        Edad = int.TryParse(modelo.Edad, out var edad) ? edad : null,
                        EstadoCivil = modelo.EstadoCivil,
                        Provincia = modelo.Provincia,
                        Canton = modelo.Canton,
                        Distrito = modelo.Distrito,
                        DireccionExacta = modelo.DireccionExacta,
                        Genero = int.TryParse(modelo.Genero, out var genero) ? genero : null
                    };
                    _context.EstudianteUsuario.Add(estudiante);
                }
                else
                {
                    estudiante.Carrera = modelo.Carrera;
                    estudiante.Edad = int.TryParse(modelo.Edad, out var edad) ? edad : estudiante.Edad;
                    estudiante.EstadoCivil = modelo.EstadoCivil;
                    estudiante.Provincia = modelo.Provincia;
                    estudiante.Canton = modelo.Canton;
                    estudiante.Distrito = modelo.Distrito;
                    estudiante.DireccionExacta = modelo.DireccionExacta;
                    estudiante.Genero = int.TryParse(modelo.Genero, out var genero) ? genero : estudiante.Genero;
                }

                // 3️⃣ Datos académicos
                var datosAcad = await _context.DatosAcademicos
                    .FirstOrDefaultAsync(d => d.CarnetEstudiante == modelo.Cedula);

                if (datosAcad == null)
                {
                    datosAcad = new DatosAcademicos
                    {
                        CarnetEstudiante = modelo.Cedula,
                        NombreColegio = modelo.ColegioConcluyo,
                        TipoInstitucion = int.TryParse(modelo.TipoInstitucion, out var tipoInst) ? tipoInst : null,
                        BecaEnColegio = int.TryParse(modelo.BecaColegio, out var becaColegio) ? becaColegio : null,
                        NombreInstitucionBecaria = modelo.InstitucionBeca,
                        MontoBeca = double.TryParse(modelo.MontoBecaColegio, out var monto) ? monto : null
                    };
                    _context.DatosAcademicos.Add(datosAcad);
                }
                else
                {
                    datosAcad.NombreColegio = modelo.ColegioConcluyo;
                    datosAcad.TipoInstitucion = int.TryParse(modelo.TipoInstitucion, out var tipoInst) ? tipoInst : datosAcad.TipoInstitucion;
                    datosAcad.BecaEnColegio = int.TryParse(modelo.BecaColegio, out var becaColegio) ? becaColegio : datosAcad.BecaEnColegio;
                    datosAcad.NombreInstitucionBecaria = modelo.InstitucionBeca;
                    datosAcad.MontoBeca = double.TryParse(modelo.MontoBecaColegio, out var monto) ? monto : datosAcad.MontoBeca;
                }

                // 4️⃣ Registrar documentos en la tabla
                var documentoFuente = new DocumentosEstudiante
                {
                    CarnetEstudiante = modelo.Cedula,
                    IdDocumento = 1, // ID real de "Fuente Ingresos" en tabla Documentos
                    Estado = 1
                };
                var documentoCompromiso = new DocumentosEstudiante
                {
                    CarnetEstudiante = modelo.Cedula,
                    IdDocumento = 2, // ID real de "Compromiso" en tabla Documentos
                    Estado = 1
                };

                _context.DocumentosEstudiante.AddRange(documentoFuente, documentoCompromiso);

                // 5️⃣ Relación con FuenteIngresosEstudiante
                var fuenteEst = new FuenteIngresosEstudiante
                {
                    CarnetEstudiante = modelo.Cedula,
                    IdFuente = 1 // ID de la fuente que corresponda
                };
                _context.FuenteIngresosEstudiante.Add(fuenteEst);

                // Guardar cambios
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Formulario guardado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private void GuardarImagenBase64(string base64, string cedula, string tipo)
        {
            if (string.IsNullOrEmpty(base64))
                return;

            var base64Data = base64.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            var carpetaDocumentos = Path.Combine(Directory.GetCurrentDirectory(), "Documentos");
            if (!Directory.Exists(carpetaDocumentos))
                Directory.CreateDirectory(carpetaDocumentos);

            var nombreArchivo = $"{cedula}-{tipo}.png";
            var rutaArchivo = Path.Combine(carpetaDocumentos, nombreArchivo);

            System.IO.File.WriteAllBytes(rutaArchivo, imageBytes);
        }
    }
}

