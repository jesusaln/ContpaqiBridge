using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;

        public StatusController(IContpaqiSdkService sdkService)
        {
            _sdkService = sdkService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            try
            {
                // Simple check: try to init SDK
                bool isConnected = _sdkService.InicializarSDK();
                if (isConnected)
                {
                    return Ok(new { status = "Online", message = "Connected to CONTPAQi SDK" });
                }
                else
                {
                    int errorCode = _sdkService.GetLastInitResult();
                    return StatusCode(500, new { 
                        status = "Error", 
                        message = "Failed to initialize SDK", 
                        sdkErrorCode = errorCode 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Error", message = ex.Message });
            }
        }

        [HttpPost("connect")]
        public IActionResult Connect([FromBody] ConnectRequest request)
        {
            try 
            {
                if (!_sdkService.InicializarSDK())
                {
                     return StatusCode(500, "Could not initialize SDK");
                }

                if (_sdkService.AbrirEmpresa(request.CompanyPath))
                {
                    // Success opening
                    _sdkService.CerrarEmpresa();
                    return Ok(new { message = $"Successfully connected to company at {request.CompanyPath}" });
                }
                else
                {
                    return BadRequest($"Failed to open company. Error: {_sdkService.GetUltimoError()}");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("unidades")]
        public IActionResult GetUnidades([FromQuery] string rutaEmpresa)
        {
            try
            {
                if (!_sdkService.InicializarSDK())
                {
                    return StatusCode(500, "Could not initialize SDK");
                }

                if (!string.IsNullOrEmpty(rutaEmpresa))
                {
                    if (!_sdkService.AbrirEmpresa(rutaEmpresa))
                    {
                        return BadRequest($"Failed to open company. Error: {_sdkService.GetUltimoError()}");
                    }
                }

                string unidades = _sdkService.ListarUnidades();
                
                if (!string.IsNullOrEmpty(rutaEmpresa))
                {
                    _sdkService.CerrarEmpresa();
                }

                return Ok(new { unidades });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("conceptos")]
        public IActionResult GetConceptos([FromQuery] string rutaEmpresa)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest("rutaEmpresa es requerida");

                var conceptos = _sdkService.ListarConceptos(rutaEmpresa);
                return Ok(new { success = true, conceptos = conceptos.Select(c => new { codigo = c.codigo, nombre = c.nombre }) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("documentos")]
        public IActionResult GetDocumentos([FromQuery] string rutaEmpresa)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest("rutaEmpresa es requerida");

                var docs = _sdkService.ListarUltimosDocumentos(rutaEmpresa);
                return Ok(new { success = true, documentos = docs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public class ConnectRequest
        {
            public string CompanyPath { get; set; }
        }
    }
}
