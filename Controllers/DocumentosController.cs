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

        /// <summary>
        /// Cancela un documento CFDI 4.0 ante el SAT
        /// </summary>
        [HttpPost("cancelar")]
        public IActionResult CancelarDocumento([FromBody] CancelarDocumentoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.CodigoConcepto))
                    return BadRequest(new { success = false, message = "CodigoConcepto es requerido" });

                _logger.LogInformation($"Solicitud de cancelación: Serie={request.Serie}, Folio={request.Folio}, Motivo={request.MotivoCancelacion}");
                
                var resultado = _sdkService.CancelarDocumento(
                    request.RutaEmpresa,
                    request.CodigoConcepto,
                    request.Serie ?? "",
                    request.Folio,
                    request.MotivoCancelacion ?? "02",
                    request.PassCSD ?? "",
                    request.UuidSustitucion
                );

                if (resultado.exito)
                {
                    return Ok(new { 
                        success = true, 
                        message = resultado.mensaje,
                        acuse = resultado.acuse 
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = resultado.mensaje });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar documento");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cancela un documento administrativamente (solo en CONTPAQi, no afecta SAT)
        /// </summary>
        [HttpPost("cancelar-admin")]
        public IActionResult CancelarDocumentoAdmin([FromBody] CancelarAdminRequest request)
        {
            try
            {
                _logger.LogInformation($"Solicitud de cancelación administrativa: Serie={request.Serie}, Folio={request.Folio}");
                
                var resultado = _sdkService.CancelarDocumentoAdministrativamente(
                    request.RutaEmpresa,
                    request.CodigoConcepto,
                    request.Serie ?? "",
                    request.Folio
                );

                return Ok(new { success = resultado.exito, message = resultado.mensaje });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar documento administrativamente");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lista los últimos documentos de la empresa para diagnóstico
        /// </summary>
        [HttpGet("ultimos")]
        public IActionResult ListarUltimosDocumentos([FromQuery] string rutaEmpresa, [FromQuery] int cantidad = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa es requerida" });

                var resultado = ((ContpaqiSdkService)_sdkService).ListarUltimosDocumentos(rutaEmpresa, cantidad);
                
                return Ok(new { success = true, documentos = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar documentos");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CancelarDocumentoRequest
    {
        public string RutaEmpresa { get; set; } = "";
        public string CodigoConcepto { get; set; } = "";
        public string Serie { get; set; } = "";
        public double Folio { get; set; }
        public string MotivoCancelacion { get; set; } = "02";
        public string PassCSD { get; set; } = "";
        public string? UuidSustitucion { get; set; }
    }

    public class CancelarAdminRequest
    {
        public string RutaEmpresa { get; set; } = "";
        public string CodigoConcepto { get; set; } = "";
        public string Serie { get; set; } = "";
        public double Folio { get; set; }
    }
}
