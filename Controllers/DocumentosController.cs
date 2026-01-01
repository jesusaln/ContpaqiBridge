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

        /// <summary>
        /// Timbra una factura en CONTPAQi
        /// </summary>
        [HttpPost("timbrar")]
        public IActionResult TimbrarFactura([FromBody] TimbrarRequest request)
        {
            try
            {
                _logger.LogInformation($"Timbrando factura {request.Serie}{request.Folio} para concepto {request.CodigoConcepto}");

                if (string.IsNullOrEmpty(request.RutaEmpresa))
                    return BadRequest(new { success = false, message = "RutaEmpresa es requerida" });

                if (string.IsNullOrEmpty(request.CodigoConcepto))
                    return BadRequest(new { success = false, message = "CodigoConcepto es requerido" });

                if (request.Folio <= 0)
                    return BadRequest(new { success = false, message = "Folio es requerido" });

                var resultado = _sdkService.TimbrarFactura(
                    request.RutaEmpresa,
                    request.CodigoConcepto,
                    request.Serie ?? "",
                    request.Folio,
                    request.PassCSD ?? ""
                );

                if (resultado.exito)
                {
                    return Ok(new { success = true, message = resultado.mensaje });
                }
                else
                {
                    return BadRequest(new { success = false, message = resultado.mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al timbrar factura");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el XML de una factura timbrada
        /// </summary>
        [HttpGet("xml")]
        public IActionResult ObtenerXml([FromQuery] string rutaEmpresa, [FromQuery] string codigoConcepto, [FromQuery] string serie, [FromQuery] double folio)
        {
            try
            {
                _logger.LogInformation($"Solicitud de XML: Concepto={codigoConcepto}, Serie={serie}, Folio={folio}");
                var resultado = _sdkService.ObtenerXml(rutaEmpresa, codigoConcepto, serie ?? "", folio);
                
                if (resultado.exito)
                {
                    return Ok(new { success = true, xml = resultado.xml });
                }
                else
                {
                    return BadRequest(new { success = false, message = resultado.mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en endpoint de XML");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
