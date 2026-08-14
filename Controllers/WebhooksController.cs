using Microsoft.AspNetCore.Mvc;
using ContpaqiBridge.Services;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : ControllerBase
    {
        private readonly WebhookService _webhooks;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(WebhookService webhooks, ILogger<WebhooksController> logger)
        {
            _webhooks = webhooks;
            _logger = logger;
        }

        /// <summary>
        /// Registra un webhook. Devuelve el secret (solo se muestra UNA VEZ).
        /// Guárdalo de forma segura: lo necesitarás para verificar la firma HMAC en tu Laravel.
        /// </summary>
        [HttpPost]
        public IActionResult Registrar([FromBody] WebhookRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(req.Evento) || string.IsNullOrEmpty(req.Url))
                    return BadRequest(new { success = false, message = "evento y url requeridos" });

                if (!Uri.TryCreate(req.Url, UriKind.Absolute, out _))
                    return BadRequest(new { success = false, message = "url inválida" });

                string secret = _webhooks.Registrar(req.Evento, req.Url);
                return Ok(new
                {
                    success = true,
                    message = $"Webhook registrado: {req.Evento} -> {req.Url}",
                    evento = req.Evento,
                    url = req.Url,
                    secret, // ⚠️ Mostrar solo en el registro
                    headersImportantes = new[] {
                        "X-Contpaqi-Signature: sha256={HMAC_SHA256_DEL_PAYLOAD}",
                        "X-Contpaqi-Event: nombre_del_evento",
                        "X-Contpaqi-Delivery: id_unico_del_intento",
                        "X-Contpaqi-Attempt: numero_de_intento"
                    },
                    verificacion = new {
                        algoritmo = "HMAC-SHA256",
                        header = "X-Contpaqi-Signature",
                        formato = "sha256={hex}",
                        payloadASignar = "cuerpo crudo del request (raw body)"
                    },
                    retry = new {
                        politica = "backoff exponencial",
                        intentos = new[] { "0s (inmediato)", "60s", "5min", "15min" },
                        maxIntentos = 4
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lista webhooks registrados (NO muestra el secret por seguridad).
        /// </summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var webhooks = _webhooks.Listar();
            return Ok(new
            {
                success = true,
                count = webhooks.Count,
                webhooks = webhooks.Select(w => new {
                    evento = w.evento,
                    url = w.url,
                    id = w.id,
                    createdAt = w.createdAt
                    // secret NO se devuelve
                })
            });
        }

        /// <summary>
        /// Emite un webhook manualmente (testing).
        /// </summary>
        [HttpPost("emit")]
        public IActionResult Emit([FromBody] WebhookEmitRequest req)
        {
            try
            {
                _webhooks.Emitir(req.Evento, req.Payload);
                return Ok(new { success = true, message = $"Webhook '{req.Evento}' encolado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un webhook por evento + URL.
        /// </summary>
        [HttpDelete]
        public IActionResult Eliminar([FromQuery] string evento, [FromQuery] string url)
        {
            try
            {
                if (string.IsNullOrEmpty(evento) || string.IsNullOrEmpty(url))
                    return BadRequest(new { success = false, message = "evento y url requeridos" });
                _webhooks.Eliminar(evento, url);
                return Ok(new { success = true, message = $"Webhook eliminado: {evento} -> {url}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
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