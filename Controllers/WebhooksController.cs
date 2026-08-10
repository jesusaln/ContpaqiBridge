using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly IContpaqiSdkService _sdkService;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(IContpaqiSdkService sdkService, ILogger<WebhooksController> logger)
        {
            _sdkService = sdkService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un webhook que será llamado cuando ocurra un evento.
        /// Eventos disponibles: timbrado.exitoso, timbrado.fallido, cancelacion.exitosa,
        /// cancelacion.fallida, documento.creado, factura.pagada
        /// Usa "*" para suscribirte a todos los eventos.
        /// </summary>
        [HttpPost]
        public IActionResult Registrar([FromBody] WebhookRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Evento) || string.IsNullOrEmpty(request.Url))
                    return BadRequest(new { success = false, message = "evento y url requeridos" });

                if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
                    return BadRequest(new { success = false, message = "url inválida" });

                ((ContpaqiSdkService)_sdkService).RegistrarWebhook(request.Evento, request.Url);
                return Ok(new { success = true, message = $"Webhook registrado: {request.Evento} -> {request.Url}" });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }

        /// <summary>
        /// Lista todos los webhooks registrados.
        /// </summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var webhooks = ((ContpaqiSdkService)_sdkService).ListarWebhooks();
            return Ok(new
            {
                success = true,
                count = webhooks.Count,
                webhooks = webhooks.Select(w => new { evento = w.evento, url = w.url })
            });
        }

        /// <summary>
        /// Emite un webhook manualmente (testing).
        /// </summary>
        [HttpPost("emit")]
        public IActionResult Emit([FromBody] WebhookEmitRequest request)
        {
            try
            {
                ((ContpaqiSdkService)_sdkService).EmitirWebhook(request.Evento, request.Payload);
                return Ok(new { success = true, message = $"Webhook '{request.Evento}' emitido a suscriptores." });
            }
            catch (Exception ex) { return StatusCode(500, new { success = false, message = ex.Message }); }
        }
    }

    public class WebhookRequest
    {
        public string Evento { get; set; } = "";
        public string Url { get; set; } = "";
    }

    public class WebhookEmitRequest
    {
        public string Evento { get; set; } = "";
        public object Payload { get; set; } = new { };
    }
}