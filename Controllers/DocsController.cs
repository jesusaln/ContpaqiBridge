using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text;

namespace ContpaqiBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;

        public DocsController(IConfiguration config, IHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        /// <summary>
        /// Devuelve el manifiesto OpenAPI 3.0 (simplificado) generado por reflexión.
        /// Compatible con clientes que sepan leer OpenAPI.
        /// </summary>
        [HttpGet("openapi.json")]
        public IActionResult GetOpenApi()
        {
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .OrderBy(t => t.Name)
                .ToList();

            var paths = new Dictionary<string, object>();

            foreach (var ctrl in controllers)
            {
                var routeAttr = ctrl.GetCustomAttribute<RouteAttribute>();
                string ctrlRoute = routeAttr?.Template?.Replace("[controller]", ctrl.Name.Replace("Controller", ""))
                    ?? "api/" + ctrl.Name.Replace("Controller", "");

                foreach (var method in ctrl.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var httpAttrs = method.GetCustomAttributes()
                        .Where(a => a.GetType().Name.StartsWith("Http"))
                        .ToList();
                    if (httpAttrs.Count == 0) continue;

                    string verb = "get";
                    string subRoute = "";
                    foreach (var a in httpAttrs)
                    {
                        var name = a.GetType().Name;
                        if (name == "HttpGetAttribute") verb = "get";
                        else if (name == "HttpPostAttribute") verb = "post";
                        else if (name == "HttpPutAttribute") verb = "put";
                        else if (name == "HttpDeleteAttribute") verb = "delete";
                        else if (name == "HttpPatchAttribute") verb = "patch";

                        var tpl = a.GetType().GetProperty("Template")?.GetValue(a) as string;
                        if (!string.IsNullOrEmpty(tpl)) subRoute = tpl;
                    }

                    string fullPath = "/" + ctrlRoute.TrimStart('/') + "/" + subRoute.TrimStart('/');
                    fullPath = fullPath.Replace("//", "/").TrimEnd('/');
                    if (string.IsNullOrEmpty(fullPath) || fullPath == "/") continue;

                    var summary = method.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description
                        ?? method.Name;

                    paths[fullPath] = new Dictionary<string, object>
                    {
                        [verb] = new Dictionary<string, object>
                        {
                            ["summary"] = summary,
                            ["operationId"] = $"{ctrl.Name}_{method.Name}",
                            ["tags"] = new[] { ctrl.Name.Replace("Controller", "") },
                            ["security"] = new[] { new { ApiKey = new string[0] } },
                            ["responses"] = new Dictionary<string, object>
                            {
                                ["200"] = new { description = "OK" },
                                ["400"] = new { description = "Bad Request" },
                                ["401"] = new { description = "API Key requerida" },
                                ["403"] = new { description = "API Key inválida" },
                                ["500"] = new { description = "Error interno" }
                            }
                        }
                    };
                }
            }

            var openapi = new
            {
                openapi = "3.0.1",
                info = new
                {
                    title = "Contpaqi Bridge API",
                    version = "v1",
                    description = "API REST que expone el SDK de CONTPAQi Comercial Premium a sistemas en la nube."
                },
                servers = new[]
                {
                    new { url = "http://0.0.0.0:5000", description = "Local" }
                },
                components = new
                {
                    securitySchemes = new Dictionary<string, object>
                    {
                        ["ApiKey"] = new
                        {
                            type = "apiKey",
                            name = "X-Api-Key",
                            @in = "header",
                            description = "API Key del bridge. Configurar en appsettings.json (Bridge:ApiKey)."
                        }
                    }
                },
                paths
            };

            return Ok(openapi);
        }

        /// <summary>
        /// Devuelve un README HTML navegable con todos los endpoints.
        /// Accesible sin API Key.
        /// </summary>
        [HttpGet]
        [HttpGet("index")]
        public IActionResult Index()
        {
            bool apiKeyConfigured = !string.IsNullOrEmpty(_config["Bridge:ApiKey"]);
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html><html lang='es'><head><meta charset='UTF-8'>");
            sb.AppendLine("<title>Contpaqi Bridge API</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{font-family:Segoe UI,Arial,sans-serif;max-width:1100px;margin:20px auto;padding:20px;color:#222;background:#f7f7f9}");
            sb.AppendLine("h1{color:#2c3e50;border-bottom:3px solid #3498db;padding-bottom:10px}");
            sb.AppendLine("h2{color:#34495e;margin-top:30px}");
            sb.AppendLine("code,pre{background:#2c3e50;color:#ecf0f1;padding:2px 6px;border-radius:3px;font-family:Consolas,monospace}");
            sb.AppendLine("pre{padding:15px;overflow:auto}");
            sb.AppendLine("table{border-collapse:collapse;width:100%;background:white;margin:15px 0;box-shadow:0 1px 3px rgba(0,0,0,0.1)}");
            sb.AppendLine("th,td{border:1px solid #ddd;padding:10px;text-align:left}");
            sb.AppendLine("th{background:#3498db;color:white}");
            sb.AppendLine(".tag{display:inline-block;padding:3px 8px;border-radius:3px;font-size:11px;font-weight:bold;color:white;margin-right:8px}");
            sb.AppendLine(".get{background:#61affe}.post{background:#49cc90}.put{background:#fca130}.delete{background:#f93e3e}");
            sb.AppendLine(".warning{background:#fff3cd;border-left:4px solid #ffc107;padding:15px;margin:15px 0}");
            sb.AppendLine(".info{background:#d1ecf1;border-left:4px solid #17a2b8;padding:15px;margin:15px 0}");
            sb.AppendLine(".endpoint{background:white;padding:15px;margin:10px 0;border-radius:5px;border-left:4px solid #3498db}");
            sb.AppendLine("</style></head><body>");

            sb.AppendLine("<h1>Contpaqi Bridge API</h1>");
            sb.AppendLine("<p>API REST que expone el SDK de CONTPAQi Comercial Premium a sistemas en la nube. Permite crear, modificar, consultar documentos y cat&aacute;logos, as&iacute; como timbrar y cancelar CFDI 4.0.</p>");

            if (!apiKeyConfigured)
            {
                sb.AppendLine("<div class='warning'><strong>ADVERTENCIA:</strong> <code>Bridge:ApiKey</code> no est&aacute; configurado en <code>appsettings.json</code>. Todas las llamadas autenticadas ser&aacute;n rechazadas hasta que se configure.</div>");
            }
            else
            {
                sb.AppendLine("<div class='info'><strong>API Key configurada.</strong> Todas las llamadas deben incluir el header <code>X-Api-Key</code> o el query <code>?api_key=</code>.</div>");
            }

            sb.AppendLine("<h2>Endpoints Disponibles</h2>");

            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
                .OrderBy(t => t.Name);

            foreach (var ctrl in controllers)
            {
                var routeAttr = ctrl.GetCustomAttribute<RouteAttribute>();
                string ctrlRoute = routeAttr?.Template?.Replace("[controller]", ctrl.Name.Replace("Controller", ""))
                    ?? "api/" + ctrl.Name.Replace("Controller", "");

                sb.AppendLine($"<h3>{ctrl.Name.Replace("Controller", "")}</h3>");

                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetCustomAttributes().Any(a => a.GetType().Name.StartsWith("Http")))
                    .OrderBy(m => m.Name);

                foreach (var method in methods)
                {
                    var httpAttr = method.GetCustomAttributes().First(a => a.GetType().Name.StartsWith("Http"));
                    string verb = httpAttr.GetType().Name.Replace("Http", "").Replace("Attribute", "").ToLower();
                    if (verb == "") verb = "get";
                    var tpl = httpAttr.GetType().GetProperty("Template")?.GetValue(httpAttr) as string;
                    string fullPath = "/" + ctrlRoute.TrimStart('/') + (string.IsNullOrEmpty(tpl) ? "" : "/" + tpl.TrimStart('/'));
                    fullPath = fullPath.Replace("//", "/").TrimEnd('/');

                    var desc = method.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? method.Name;

                    sb.AppendLine("<div class='endpoint'>");
                    sb.Append($"<span class='tag {verb}'>{verb.ToUpper()}</span>");
                    sb.AppendLine($"<code>{fullPath}</code>");
                    sb.AppendLine($"<p style='margin-top:8px;color:#555'>{System.Net.WebUtility.HtmlEncode(desc)}</p>");
                    sb.AppendLine("</div>");
                }
            }

            sb.AppendLine("<h2>Ejemplo de uso con curl</h2>");
            sb.AppendLine("<pre>curl -H \"X-Api-Key: TU_CLAVE\" http://localhost:5000/api/Empresas</pre>");

            sb.AppendLine("<h2>Manifiesto OpenAPI</h2>");
            sb.AppendLine("<p>Disponible en <a href='/api/Docs/openapi.json'><code>/api/Docs/openapi.json</code></a> para importarlo a Postman, Insomnia, etc.</p>");

            sb.AppendLine("</body></html>");

            return Content(sb.ToString(), "text/html; charset=utf-8");
        }
    }
}