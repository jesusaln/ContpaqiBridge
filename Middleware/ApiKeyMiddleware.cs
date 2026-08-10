using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContpaqiBridge.Middleware
{
    /// <summary>
    /// Middleware que valida que cada petición incluya un API Key válido.
    /// Se puede enviar como:
    ///   - Header: X-Api-Key: <clave>
    ///   - Query: ?api_key=<clave>
    /// Endpoints excluidos (sin auth): /swagger, /api/Status/health
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyMiddleware> _logger;
        private readonly string _configuredKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration config, ILogger<ApiKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _configuredKey = config["Bridge:ApiKey"] ?? "";

            if (string.IsNullOrEmpty(_configuredKey))
            {
                _logger.LogWarning("Bridge:ApiKey no está configurado en appsettings. El servicio NO aceptará llamadas autenticadas hasta que se configure.");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // Endpoints públicos (sin auth)
            bool esPublico =
                path.StartsWith("/swagger") ||
                path.StartsWith("/api/docs") ||
                path == "/" ||
                path == "/api/status/health" ||
                path.StartsWith("/api/status/health/");

            if (esPublico)
            {
                await _next(context);
                return;
            }

            // Si no hay clave configurada, rechazar todo por seguridad
            if (string.IsNullOrEmpty(_configuredKey))
            {
                _logger.LogWarning($"Petición rechazada: API Key no configurada en el servidor. Path={path}");
                context.Response.StatusCode = 503;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "API Key no configurada en el servidor. Configure Bridge:ApiKey en appsettings.json."
                });
                return;
            }

            // Obtener API Key del request
            string providedKey = context.Request.Headers["X-Api-Key"].FirstOrDefault() ?? "";
            if (string.IsNullOrEmpty(providedKey))
            {
                providedKey = context.Request.Query["api_key"].FirstOrDefault() ?? "";
            }

            if (string.IsNullOrEmpty(providedKey))
            {
                _logger.LogWarning($"Petición sin API Key rechazada desde {context.Connection.RemoteIpAddress}. Path={path}");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "API Key requerida. Envíela en header 'X-Api-Key' o query '?api_key='."
                });
                return;
            }

            // Comparación timing-safe
            if (!CryptographicEquals(providedKey, _configuredKey))
            {
                _logger.LogWarning($"Petición con API Key inválida desde {context.Connection.RemoteIpAddress}. Path={path}");
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = "API Key inválida."
                });
                return;
            }

            await _next(context);
        }

        private static bool CryptographicEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}