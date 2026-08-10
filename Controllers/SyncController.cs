using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;
using ContpaqiBridge.Models;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(IContpaqiSdkService sdkService, ILogger<SyncController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        // ====================================================================
        // ============ LECTURA (CONTPAQi → Laravel) ==========================
        // ====================================================================

        /// <summary>
        /// Obtiene TODOS los clientes de CONTPAQi (snapshot completo).
        /// Útil para la sincronización inicial o reparación.
        /// </summary>
        [HttpGet("clientes")]
        public IActionResult PullClientes([FromQuery] string rutaEmpresa, [FromQuery] int limite = 500)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
                var clientes = ((ContpaqiSdkService)_sdkService).ListarClientesTodos(rutaEmpresa, limite);
                return Ok(new { success = true, count = clientes.Count, clientes });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Obtiene clientes modificados desde una fecha (sincronización incremental).
        /// </summary>
        [HttpGet("clientes/modificados")]
        public IActionResult PullClientesModificados([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] int limite = 500)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
                var clientes = ((ContpaqiSdkService)_sdkService).ListarClientesModificados(rutaEmpresa, desde, limite);
                return Ok(new { success = true, count = clientes.Count, desde, clientes });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Obtiene TODOS los productos de CONTPAQi.
        /// </summary>
        [HttpGet("productos")]
        public IActionResult PullProductos([FromQuery] string rutaEmpresa, [FromQuery] int limite = 500)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
                var productos = ((ContpaqiSdkService)_sdkService).ListarProductosTodos(rutaEmpresa, limite);
                return Ok(new { success = true, count = productos.Count, productos });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Obtiene productos modificados desde una fecha.
        /// </summary>
        [HttpGet("productos/modificados")]
        public IActionResult PullProductosModificados([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] int limite = 500)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
                var productos = ((ContpaqiSdkService)_sdkService).ListarProductosModificados(rutaEmpresa, desde, limite);
                return Ok(new { success = true, count = productos.Count, desde, productos });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Obtiene documentos (facturas, notas) modificados desde una fecha.
        /// Cada documento trae el UUID, folio, total, cliente, etc.
        /// Es el endpoint clave para sincronizar ventas desde CONTPAQi hacia Laravel.
        /// </summary>
        [HttpGet("documentos/modificados")]
        public IActionResult PullDocumentos([FromQuery] string rutaEmpresa, [FromQuery] DateTime desde, [FromQuery] int limite = 500)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
                var docs = ((ContpaqiSdkService)_sdkService).ListarDocumentosModificados(rutaEmpresa, desde, limite);
                return Ok(new { success = true, count = docs.Count, desde, documentos = docs });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        // ====================================================================
        // ============ ESCRITURA (Laravel → CONTPAQi) ========================
        // ====================================================================

        /// <summary>
        /// Crea/actualiza un lote de clientes en CONTPAQi.
        /// Recibe un array y procesa uno por uno, devolviendo el resultado de cada uno.
        /// Diseñado para sincronización masiva desde Laravel.
        /// </summary>
        [HttpPost("clientes/batch")]
        public IActionResult PushClientesBatch([FromBody] SyncBatchRequest<ClienteRequest> request)
        {
            if (string.IsNullOrEmpty(request.RutaEmpresa))
                return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { success = false, message = "items vacío" });

            var resultados = new List<object>();
            int ok = 0, error = 0;
            var service = (ContpaqiSdkService)_sdkService;

            foreach (var cli in request.Items)
            {
                try
                {
                    cli.RutaEmpresa = request.RutaEmpresa;
                    var r = service.CrearCliente(
                        cli.RutaEmpresa, cli.Codigo, cli.RazonSocial, cli.RFC ?? "",
                        cli.Email ?? "", cli.Calle ?? "", cli.Colonia ?? "",
                        cli.CodigoPostal ?? "", cli.Ciudad ?? "", cli.Estado ?? "",
                        cli.Pais ?? "México",
                        cli.RegimenFiscal ?? "", cli.UsoCFDI ?? "", cli.FormaPago ?? ""
                    );
                    resultados.Add(new { codigo = cli.Codigo, exito = r.exito, mensaje = r.mensaje, idCliente = r.idCliente });
                    if (r.exito) ok++; else error++;
                }
                catch (Exception ex)
                {
                    resultados.Add(new { codigo = cli.Codigo, exito = false, mensaje = ex.Message });
                    error++;
                }
            }

            return Ok(new { success = true, total = request.Items.Count, ok, error, resultados });
        }

        /// <summary>
        /// Crea/actualiza un lote de productos en CONTPAQi.
        /// </summary>
        [HttpPost("productos/batch")]
        public IActionResult PushProductosBatch([FromBody] SyncBatchRequest<ProductoRequest> request)
        {
            if (string.IsNullOrEmpty(request.RutaEmpresa))
                return BadRequest(new { success = false, message = "rutaEmpresa requerida" });
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { success = false, message = "items vacío" });

            var resultados = new List<object>();
            int ok = 0, error = 0;
            var service = (ContpaqiSdkService)_sdkService;

            foreach (var prod in request.Items)
            {
                try
                {
                    prod.RutaEmpresa = request.RutaEmpresa;
                    var r = service.CrearProducto(
                        prod.RutaEmpresa, prod.Codigo, prod.Nombre,
                        prod.Descripcion ?? "", prod.Precio,
                        prod.TipoProducto > 0 ? prod.TipoProducto : 1,
                        prod.UnidadMedida ?? "PZA", prod.ClaveSAT ?? ""
                    );
                    resultados.Add(new { codigo = prod.Codigo, exito = r.exito, mensaje = r.mensaje, idProducto = r.idProducto });
                    if (r.exito) ok++; else error++;
                }
                catch (Exception ex)
                {
                    resultados.Add(new { codigo = prod.Codigo, exito = false, mensaje = ex.Message });
                    error++;
                }
            }

            return Ok(new { success = true, total = request.Items.Count, ok, error, resultados });
        }

        /// <summary>
        /// Devuelve el estado general del sistema para diagnóstico.
        /// </summary>
        [HttpGet("status")]
        public IActionResult Status([FromQuery] string rutaEmpresa)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaEmpresa))
                    return BadRequest(new { success = false, message = "rutaEmpresa requerida" });

                if (!_sdkService.InicializarSDK())
                    return StatusCode(500, new { success = false, message = "SDK no inicializado" });

                bool empresaOk = _sdkService.AbrirEmpresa(rutaEmpresa);
                _sdkService.CerrarEmpresa();

                int clientes = ((ContpaqiSdkService)_sdkService).ListarClientesTodos(rutaEmpresa, 9999).Count;
                int productos = ((ContpaqiSdkService)_sdkService).ListarProductosTodos(rutaEmpresa, 9999).Count;
                int documentos = ((ContpaqiSdkService)_sdkService).ListarUltimosDocumentos(rutaEmpresa, 9999).Count;

                return Ok(new
                {
                    success = true,
                    sdkInicializado = true,
                    empresaAbierta = empresaOk,
                    rutaEmpresa,
                    conteos = new { clientes, productos, documentos },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }
    }

    /// <summary>
    /// Request genérico para batch sync.
    /// </summary>
    public class SyncBatchRequest<T>
    {
        public string RutaEmpresa { get; set; } = "";
        public List<T> Items { get; set; } = new();
    }
}