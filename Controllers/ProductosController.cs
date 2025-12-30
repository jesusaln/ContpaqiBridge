using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;
using ContpaqiBridge.Models;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IContpaqiSdkService sdkService, ILogger<ProductosController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un producto en CONTPAQi
        /// </summary>
        [HttpPost]
        public IActionResult CrearProducto([FromBody] ProductoRequest request)
        {
            try
            {
                _logger.LogInformation($"Creando producto: {request.Codigo} - {request.Nombre}");

                if (string.IsNullOrEmpty(request.RutaEmpresa))
                    return BadRequest(new { success = false, message = "RutaEmpresa es requerida" });

                if (string.IsNullOrEmpty(request.Codigo))
                    return BadRequest(new { success = false, message = "Codigo es requerido" });

                if (string.IsNullOrEmpty(request.Nombre))
                    return BadRequest(new { success = false, message = "Nombre es requerido" });

                var service = (ContpaqiSdkService)_sdkService;
                var resultado = service.CrearProducto(
                    request.RutaEmpresa,
                    request.Codigo,
                    request.Nombre,
                    request.Descripcion ?? "",
                    request.Precio,
                    request.TipoProducto > 0 ? request.TipoProducto : 1,
                    request.UnidadMedida ?? "PZA",
                    request.ClaveSAT ?? ""
                );

                if (resultado.exito)
                {
                    return Ok(new
                    {
                        success = true,
                        message = resultado.mensaje,
                        idProducto = resultado.idProducto
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
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
