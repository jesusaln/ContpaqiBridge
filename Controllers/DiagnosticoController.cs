using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticoController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<DiagnosticoController> _logger;

        public DiagnosticoController(IContpaqiSdkService sdkService, ILogger<DiagnosticoController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        [HttpGet("unidades")]
        public IActionResult ListarUnidades([FromQuery] string rutaEmpresa)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa)) return BadRequest("rutaEmpresa es requerida");

                if (!_sdkService.InicializarSDK())
                {
                    return StatusCode(500, "No se pudo inicializar el SDK");
                }

                if (!_sdkService.AbrirEmpresa(rutaEmpresa))
                {
                    return StatusCode(500, $"No se pudo abrir la empresa: {_sdkService.GetUltimoError()}");
                }

                string unidades = _sdkService.ListarUnidades();
                _sdkService.CerrarEmpresa();

                return Ok(unidades);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
