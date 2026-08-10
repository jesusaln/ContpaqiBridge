using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;

        public ReportesController(IContpaqiSdkService sdkService)
        {
            _sdkService = sdkService;
        }

        /// <summary>
        /// Ventas agrupadas por día en un rango de fechas.
        /// GET /api/Reportes/ventas?rutaEmpresa=...&desde=2024-01-01&hasta=2024-01-31
        /// </summary>
        [HttpGet("ventas")]
        public IActionResult Ventas([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });

                var datos = ((ContpaqiSdkService)_sdkService).ReporteVentasPorPeriodo(rutaEmpresa, desde, hasta);
                double total = datos.Sum(d => Convert.ToDouble(d.GetValueOrDefault("total") ?? 0));

                return Ok(new
                {
                    success = true,
                    desde,
                    hasta,
                    totalGeneral = total,
                    count = datos.Count,
                    ventas = datos
                });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Top N clientes que más compraron en un periodo.
        /// GET /api/Reportes/top-clientes?rutaEmpresa=...&desde=2024-01-01&hasta=2024-01-31&top=10
        /// </summary>
        [HttpGet("top-clientes")]
        public IActionResult TopClientes([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] DateTime hasta, [FromQuery] int top = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });

                var datos = ((ContpaqiSdkService)_sdkService).ReporteTopClientes(rutaEmpresa, desde, hasta, top);
                return Ok(new { success = true, desde, hasta, top, count = datos.Count, clientes = datos });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Top N productos más vendidos en un periodo.
        /// </summary>
        [HttpGet("top-productos")]
        public IActionResult TopProductos([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] DateTime hasta, [FromQuery] int top = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });

                var datos = ((ContpaqiSdkService)_sdkService).ReporteTopProductos(rutaEmpresa, desde, hasta, top);
                return Ok(new { success = true, desde, hasta, top, count = datos.Count, productos = datos });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }
    }
}