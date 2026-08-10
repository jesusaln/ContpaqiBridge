using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmpresasController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Lista las empresas CONTPAQi disponibles en la carpeta configurada.
        /// Útil para que el cliente sepa qué `rutaEmpresa` enviar en los demás endpoints.
        /// </summary>
        [HttpGet]
        public IActionResult ListarEmpresas()
        {
            try
            {
                string empresasPath = _config["Contpaqi:EmpresasPath"] ?? @"C:\Compac\Empresas";
                if (!Directory.Exists(empresasPath))
                {
                    return Ok(new
                    {
                        success = true,
                        empresasPath,
                        count = 0,
                        empresas = new string[0],
                        message = $"La carpeta {empresasPath} no existe. Configure Contpaqi:EmpresasPath en appsettings.json."
                    });
                }

                var carpetas = Directory.GetDirectories(empresasPath);
                var empresas = new System.Collections.Generic.List<object>();

                foreach (var carpeta in carpetas)
                {
                    var nombre = Path.GetFileName(carpeta);
                    // Heurística: una empresa es una carpeta que contiene admDocumentos o MetaDatos.inf
                    bool esEmpresaValida =
                        System.IO.File.Exists(Path.Combine(carpeta, "MetaDatos.inf")) ||
                        Directory.Exists(Path.Combine(carpeta, "CSD"));

                    empresas.Add(new
                    {
                        nombre,
                        rutaEmpresa = carpeta,
                        valida = esEmpresaValida,
                        tieneCSD = Directory.Exists(Path.Combine(carpeta, "CSD"))
                    });
                }

                return Ok(new
                {
                    success = true,
                    empresasPath,
                    count = empresas.Count,
                    empresas
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}