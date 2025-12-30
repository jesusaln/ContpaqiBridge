using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;
using ContpaqiBridge.Models;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<DocumentosController> _logger;

        public DocumentosController(IContpaqiSdkService sdkService, ILogger<DocumentosController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una factura en CONTPAQi
        /// </summary>
        [HttpPost("factura")]
        public IActionResult CrearFactura([FromBody] FacturaRequest request)
        {
            try
            {
                _logger.LogInformation($"Creando factura para cliente {request.CodigoCliente} con concepto {request.CodigoConcepto}");

                if (string.IsNullOrEmpty(request.RutaEmpresa))
                {
                    return BadRequest("RutaEmpresa es requerida");
                }

                if (string.IsNullOrEmpty(request.CodigoConcepto))
                {
                    return BadRequest("CodigoConcepto es requerido");
                }

                if (string.IsNullOrEmpty(request.CodigoCliente))
                {
                    return BadRequest("CodigoCliente es requerido");
                }

                // Productos opcionales para testing (probar cabecera primero)
                // if (request.Productos == null || request.Productos.Count == 0)
                // {
                //     return BadRequest("Debe incluir al menos un producto");
                // }

                // Convertir productos al formato esperado por el servicio (puede estar vacío)
                var productos = (request.Productos ?? new List<ProductoFactura>())
                    .Select(p => (p.Codigo, p.Cantidad, p.Precio))
                    .ToList();

                // Llamar al servicio
                var resultado = ((ContpaqiSdkService)_sdkService).CrearFactura(
                    request.RutaEmpresa,
                    request.CodigoConcepto,
                    request.CodigoCliente,
                    productos
                );

                if (resultado.exito)
                {
                    return Ok(new
                    {
                        success = true,
                        message = resultado.mensaje,
                        idDocumento = resultado.idDocumento
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
                _logger.LogError(ex, "Error al crear factura");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
