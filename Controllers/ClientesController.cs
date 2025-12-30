using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;
using ContpaqiBridge.Models;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IContpaqiSdkService sdkService, ILogger<ClientesController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un cliente en CONTPAQi
        /// </summary>
        [HttpPost]
        public IActionResult CrearCliente([FromBody] ClienteRequest request)
        {
            try
            {
                _logger.LogInformation($"Creando cliente: {request.Codigo} - {request.RazonSocial}");

                if (string.IsNullOrEmpty(request.RutaEmpresa))
                    return BadRequest(new { success = false, message = "RutaEmpresa es requerida" });

                if (string.IsNullOrEmpty(request.Codigo))
                    return BadRequest(new { success = false, message = "Codigo es requerido" });

                if (string.IsNullOrEmpty(request.RazonSocial))
                    return BadRequest(new { success = false, message = "RazonSocial es requerida" });

                var service = (ContpaqiSdkService)_sdkService;
                var resultado = service.CrearCliente(
                    request.RutaEmpresa,
                    request.Codigo,
                    request.RazonSocial,
                    request.RFC ?? "",
                    request.Email ?? "",
                    request.Calle ?? "",
                    request.Colonia ?? "",
                    request.CodigoPostal ?? "",
                    request.Ciudad ?? "",
                    request.Estado ?? "",
                    request.Pais ?? "México"
                );

                if (resultado.exito)
                {
                    return Ok(new
                    {
                        success = true,
                        message = resultado.mensaje,
                        idCliente = resultado.idCliente
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = resultado.mensaje
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
