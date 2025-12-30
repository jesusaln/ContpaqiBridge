using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;
using ContpaqiBridge.Models;
using System.Collections.Generic;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegracionController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<IntegracionController> _logger;

        public IntegracionController(IContpaqiSdkService sdkService, ILogger<IntegracionController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        [HttpPost("flujo-completo")]
        public IActionResult FlujoCompleto([FromBody] IntegracionRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando flujo completo: Cliente -> Producto -> Factura -> Timbrado");

                if (string.IsNullOrEmpty(request.RutaEmpresa))
                    return BadRequest(new { success = false, message = "RutaEmpresa es requerida" });

                // 1. Crear/Verificar Cliente
                _logger.LogInformation($"Paso 1: Crear/Verificar Cliente {request.Cliente.Codigo}");
                var resCliente = _sdkService.CrearCliente(
                    request.RutaEmpresa,
                    request.Cliente.Codigo,
                    request.Cliente.RazonSocial,
                    request.Cliente.RFC,
                    request.Cliente.Email,
                    request.Cliente.Calle,
                    request.Cliente.Colonia,
                    request.Cliente.CodigoPostal,
                    request.Cliente.Ciudad,
                    request.Cliente.Estado,
                    request.Cliente.Pais,
                    request.Cliente.RegimenFiscal ?? "",
                    request.Cliente.UsoCFDI ?? "",
                    request.Cliente.FormaPago ?? ""
                );


                bool clienteReusado = resCliente.mensaje.Contains("ya existe");
                if (!resCliente.exito && !clienteReusado)
                {
                    return BadRequest(new { success = false, message = $"Error en Paso 1 (Cliente): {resCliente.mensaje}" });
                }

                // 2. Crear/Verificar Producto
                _logger.LogInformation($"Paso 2: Crear/Verificar Producto {request.Producto.Codigo}");
                var resProducto = _sdkService.CrearProducto(
                    request.RutaEmpresa,
                    request.Producto.Codigo,
                    request.Producto.Nombre,
                    request.Producto.Descripcion ?? "",
                    request.Producto.Precio,
                    1, // Tipo Producto
                    request.Producto.UnidadMedida ?? "H87",
                    request.Producto.ClaveSAT ?? ""
                );

                bool productoReusado = resProducto.mensaje.Contains("ya existe");
                if (!resProducto.exito && !productoReusado)
                {
                    return BadRequest(new { success = false, message = $"Error en Paso 2 (Producto): {resProducto.mensaje}" });
                }

                // 3. Crear Factura
                _logger.LogInformation($"Paso 3: Crear Factura con concepto {request.Factura.CodigoConcepto}");
                var productos = new List<(string codigo, double cantidad, double precio)>
                {
                    (request.Producto.Codigo, request.Factura.Cantidad, request.Producto.Precio)
                };

                var resFactura = _sdkService.CrearFactura(
                    request.RutaEmpresa,
                    request.Factura.CodigoConcepto,
                    request.Cliente.Codigo,
                    productos,
                    request.Factura.UsoCFDI ?? request.Cliente.UsoCFDI ?? "G01",
                    request.Factura.FormaPago ?? request.Cliente.FormaPago ?? "99",
                    request.Factura.MetodoPago ?? "PUE"
                );


                if (!resFactura.exito)
                {
                    return BadRequest(new { success = false, message = $"Error en Paso 3 (Factura): {resFactura.mensaje}" });
                }

                // 4. Timbrar (Opcional si se provee passCSD)
                string mensajeTimbrado = "";
                if (!string.IsNullOrEmpty(request.Factura.PassCSD))
                {
                    _logger.LogInformation($"Paso 4: Intentando timbrar factura {resFactura.serie}{resFactura.folio}...");
                    var resTimbrado = _sdkService.TimbrarFactura(
                        request.RutaEmpresa,
                        request.Factura.CodigoConcepto,
                        resFactura.serie,
                        resFactura.folio,
                        request.Factura.PassCSD
                    );

                    if (resTimbrado.exito)
                    {
                        mensajeTimbrado = " | Factura TIMBRADA exitosamente.";
                    }
                    else
                    {
                        mensajeTimbrado = $" | Error al timbrar: {resTimbrado.mensaje}";
                    }
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Proceso completado con éxito." + mensajeTimbrado,
                    Detalles = new {
                        Cliente = clienteReusado ? "REUSADO" : "CREADO",
                        Producto = productoReusado ? "REUSADO" : "CREADO",
                        Factura = "CREADA",
                        Timbrado = string.IsNullOrEmpty(request.Factura.PassCSD) ? "NO SOLICITADO" : (mensajeTimbrado.Contains("TIMBRADA") ? "EXITOSO" : "FALLIDO")
                    },
                    IDs = new {
                        IdCliente = resCliente.idCliente,
                        IdProducto = resProducto.idProducto,
                        IdDocumento = resFactura.idDocumento,
                        Serie = resFactura.serie,
                        Folio = resFactura.folio
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en flujo completo");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
