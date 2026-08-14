using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContpaqiBridge.Services
{
    /// <summary>
    /// Sistema de webhooks con:
    /// - Firma HMAC-SHA256 (header X-Contpaqi-Signature) para autenticidad
    /// - Retry automático con backoff exponencial (1min, 5min, 15min)
    /// - Persistencia de webhooks pendientes en disco (sobrevive reinicios)
    /// - Sync log JSON con timestamp, evento, resultado
    /// </summary>
    public class WebhookService
    {
        private readonly ILogger<WebhookService> _logger;
        private readonly string _dataDir;
        private readonly string _webhooksFile;
        private readonly string _pendingFile;
        private readonly string _logFile;
        private readonly object _lock = new object();

        // Webhooks persistidos en disco: { id, evento, url, secret, createdAt, active }
        private List<WebhookRegistration> _registrations = new();

        // Cola de webhooks pendientes: { id, evento, payloadJson, attempts, nextRetryAt, originalTimestamp }
        private List<PendingWebhook> _pending = new();

        public WebhookService(IConfiguration config, ILogger<WebhookService> logger)
        {
            _logger = logger;
            _dataDir = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(_dataDir);
            _webhooksFile = Path.Combine(_dataDir, "webhooks.json");
            _pendingFile = Path.Combine(_dataDir, "webhooks-pending.json");
            _logFile = Path.Combine(_dataDir, "sync-log.json");

            Load();
        }

        // ====================================================================
        // PERSISTENCIA
        // ====================================================================

        private void Load()
        {
            try
            {
                if (File.Exists(_webhooksFile))
                {
                    var json = File.ReadAllText(_webhooksFile);
                    _registrations = JsonSerializer.Deserialize<List<WebhookRegistration>>(json) ?? new();
                }
                if (File.Exists(_pendingFile))
                {
                    var json = File.ReadAllText(_pendingFile);
                    _pending = JsonSerializer.Deserialize<List<PendingWebhook>>(json) ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error cargando webhooks pendientes. Empezando limpio.");
            }
        }

        private void SaveRegistrations()
        {
            try
            {
                var json = JsonSerializer.Serialize(_registrations, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_webhooksFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando webhooks.");
            }
        }

        private void SavePending()
        {
            try
            {
                var json = JsonSerializer.Serialize(_pending, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_pendingFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando cola de webhooks.");
            }
        }

        // ====================================================================
        // REGISTRO DE WEBHOOKS
        // ====================================================================

        public string Registrar(string evento, string url)
        {
            lock (_lock)
            {
                // Generar secret único por webhook
                string secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

                // Si ya existe uno igual, reutilizar secret
                var existente = _registrations.Find(w =>
                    string.Equals(w.Evento, evento, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(w.Url, url, StringComparison.OrdinalIgnoreCase));
                if (existente != null)
                {
                    existente.Active = true;
                    secret = existente.Secret;
                }
                else
                {
                    _registrations.Add(new WebhookRegistration
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Evento = evento,
                        Url = url,
                        Secret = secret,
                        CreatedAt = DateTime.UtcNow,
                        Active = true
                    });
                }
                SaveRegistrations();
                LogEvent("webhook.registered", evento, $"Webhook registrado: {url}");
                return secret;
            }
        }

        public void Eliminar(string evento, string url)
        {
            lock (_lock)
            {
                int antes = _registrations.Count;
                _registrations.RemoveAll(w =>
                    string.Equals(w.Evento, evento, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(w.Url, url, StringComparison.OrdinalIgnoreCase));
                if (_registrations.Count < antes)
                {
                    SaveRegistrations();
                    LogEvent("webhook.deleted", evento, $"Webhook eliminado: {url}");
                }
            }
        }

        public List<(string evento, string url, string id, DateTime createdAt)> Listar()
        {
            lock (_lock)
            {
                return _registrations.ConvertAll(w => (w.Evento, w.Url, w.Id, w.CreatedAt));
            }
        }

        // ====================================================================
        // EMISIÓN DE WEBHOOKS
        // ====================================================================

        /// <summary>
        /// Emite un webhook a todos los suscriptores del evento.
        /// Si el envío falla, se encola con backoff exponencial.
        /// </summary>
        public void Emitir(string evento, object payload)
        {
            List<WebhookRegistration> destinos;
            lock (_lock)
            {
                destinos = _registrations.FindAll(w =>
                    w.Active &&
                    (string.Equals(w.Evento, evento, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(w.Evento, "*", StringComparison.OrdinalIgnoreCase)));
            }

            if (destinos.Count == 0)
            {
                LogEvent("webhook.no_subscribers", evento, "No hay suscriptores para este evento");
                return;
            }

            string payloadJson = JsonSerializer.Serialize(new
            {
                evento,
                timestamp = DateTime.UtcNow.ToString("o"),
                payload
            });

            foreach (var destino in destinos)
            {
                var pendiente = new PendingWebhook
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Evento = evento,
                    Url = destino.Url,
                    Secret = destino.Secret,
                    PayloadJson = payloadJson,
                    Attempts = 0,
                    NextRetryAt = DateTime.UtcNow,
                    OriginalTimestamp = DateTime.UtcNow,
                    LastError = null
                };

                _ = Task.Run(() => EnviarConRetry(pendiente));
            }
        }

        private async Task EnviarConRetry(PendingWebhook pw)
        {
            // Backoff exponencial: 0s (primer intento), 60s, 300s (5min), 900s (15min)
            int[] delaysSegundos = new[] { 0, 60, 300, 900 };
            int maxIntentos = delaysSegundos.Length;

            while (pw.Attempts < maxIntentos)
            {
                pw.Attempts++;
                pw.NextRetryAt = DateTime.UtcNow.AddSeconds(delaysSegundos[pw.Attempts - 1]);

                if (pw.Attempts > 1)
                {
                    int delay = delaysSegundos[pw.Attempts - 2];
                    _logger.LogInformation($"Webhook {pw.Id} reintento #{pw.Attempts} en {delay}s (evento={pw.Evento} url={pw.Url})");
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }

                try
                {
                    // Calcular firma HMAC-SHA256
                    string firma = ComputeHmac(pw.PayloadJson, pw.Secret);

                    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    var content = new StringContent(pw.PayloadJson, Encoding.UTF8, "application/json");
                    content.Headers.Add("X-Contpaqi-Signature", $"sha256={firma}");
                    content.Headers.Add("X-Contpaqi-Event", pw.Evento);
                    content.Headers.Add("X-Contpaqi-Delivery", pw.Id);
                    content.Headers.Add("X-Contpaqi-Attempt", pw.Attempts.ToString());

                    var resp = await http.PostAsync(pw.Url, content);

                    if (resp.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Webhook OK {pw.Id} (intento {pw.Attempts}) evento={pw.Evento} url={pw.Url} status={resp.StatusCode}");
                        LogEvent("webhook.delivered", pw.Evento, $"OK intento {pw.Attempts}: {pw.Url}");
                        QuitarDeCola(pw);
                        return;
                    }

                    pw.LastError = $"HTTP {resp.StatusCode}";
                    LogEvent("webhook.failed", pw.Evento, $"Fallo intento {pw.Attempts}: HTTP {resp.StatusCode}");
                }
                catch (Exception ex)
                {
                    pw.LastError = ex.Message;
                    _logger.LogWarning($"Webhook {pw.Id} intento {pw.Attempts} error: {ex.Message}");
                    LogEvent("webhook.failed", pw.Evento, $"Fallo intento {pw.Attempts}: {ex.Message}");
                }
            }

            // Agotó todos los intentos
            _logger.LogError($"Webhook {pw.Id} AGOTÓ los {maxIntentos} intentos. Evento={pw.Evento} url={pw.Url}");
            LogEvent("webhook.exhausted", pw.Evento, $"Agotó {maxIntentos} intentos: {pw.Url}. último: {pw.LastError}");
            QuitarDeCola(pw);
        }

        private void QuitarDeCola(PendingWebhook pw)
        {
            lock (_lock)
            {
                _pending.RemoveAll(p => p.Id == pw.Id);
                SavePending();
            }
        }

        // ====================================================================
        // SYNC LOG
        // ====================================================================

        public void LogEvent(string tipo, string contexto, string mensaje, object? detalle = null)
        {
            try
            {
                var entry = new
                {
                    timestamp = DateTime.UtcNow.ToString("o"),
                    tipo,
                    contexto,
                    mensaje,
                    detalle
                };

                lock (_lock)
                {
                    List<object> entries;
                    if (File.Exists(_logFile))
                    {
                        var existing = File.ReadAllText(_logFile);
                        try { entries = JsonSerializer.Deserialize<List<object>>(existing) ?? new(); }
                        catch { entries = new(); }
                    }
                    else
                    {
                        entries = new();
                    }

                    entries.Add(entry);

                    // Mantener máximo 5000 entradas para evitar crecer infinito
                    if (entries.Count > 5000)
                    {
                        entries = entries.GetRange(entries.Count - 5000, 5000);
                    }

                    File.WriteAllText(_logFile, JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error escribiendo al sync log");
            }
        }

        public List<object> ObtenerLog(int ultimas = 100)
        {
            try
            {
                if (!File.Exists(_logFile)) return new();
                var json = File.ReadAllText(_logFile);
                var entries = JsonSerializer.Deserialize<List<object>>(json) ?? new();
                return entries.GetRange(Math.Max(0, entries.Count - ultimas), Math.Min(ultimas, entries.Count));
            }
            catch
            {
                return new();
            }
        }

        // ====================================================================
        // FIRMA HMAC
        // ====================================================================

        public static string ComputeHmac(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // ====================================================================
        // BACKGROUND LOOP: procesa cola de pendientes al arrancar
        // ====================================================================

        public void StartPendingProcessor()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));

                        List<PendingWebhook> aReintentar;
                        lock (_lock)
                        {
                            var ahora = DateTime.UtcNow;
                            aReintentar = _pending.FindAll(p => p.NextRetryAt <= ahora);
                        }

                        foreach (var pw in aReintentar)
                        {
                            _ = Task.Run(() => EnviarConRetry(pw));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en processor de webhooks pendientes");
                    }
                }
            });
        }
    }

    // ====================================================================
    // MODELOS
    // ====================================================================

    public class WebhookRegistration
    {
        public string Id { get; set; } = "";
        public string Evento { get; set; } = "";
        public string Url { get; set; } = "";
        public string Secret { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool Active { get; set; } = true;
    }

    public class PendingWebhook
    {
        public string Id { get; set; } = "";
        public string Evento { get; set; } = "";
        public string Url { get; set; } = "";
        public string Secret { get; set; } = "";
        public string PayloadJson { get; set; } = "";
        public int Attempts { get; set; }
        public DateTime NextRetryAt { get; set; }
        public DateTime OriginalTimestamp { get; set; }
        public string? LastError { get; set; }
    }
}