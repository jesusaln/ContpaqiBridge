using ContpaqiBridge.Middleware;
using ContpaqiBridge.Services;

var builder = WebApplication.CreateBuilder(args);

// ============ Configuración de servicios ============

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Singleton del servicio SDK (es thread-safe con _lock interno)
builder.Services.AddSingleton<IContpaqiSdkService, ContpaqiSdkService>();

// Servicio de webhooks con HMAC, retry automático y sync log
builder.Services.AddSingleton<WebhookService>();

// CORS para que frontends web puedan consumir la API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// ============ Pipeline HTTP ============

// CORS antes de auth
app.UseCors();

// Middleware de autenticación por API Key
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Health check público (no requiere API Key)
app.MapHealthChecks("/api/Status/health");

// Arrancar processor de webhooks pendientes
var webhookService = app.Services.GetRequiredService<WebhookService>();
webhookService.StartPendingProcessor();

// Warm-up del SDK de CONTPAQi en background.
// La primera llamada a fInicializaSDK() tarda ~13s por la carga de DLLs nativas
// (MGW_SDK.dll, CACSql.dll) y conexión con SQL Server. Sin warm-up, la primera
// petición HTTP tarda esos 13s. Con warm-up, las peticiones reales son <1s.
_ = Task.Run(() =>
{
    try
    {
        // Forzar carga del singleton (lazy) y pre-inicializar el SDK en background.
        // InicializarSDK() abre empresa del config si existe, pero como solo queremos
        // pre-calentar las DLLs nativas sin tocar SQL, usamos GetStatus() que solo
        // carga la sesión.
        var sdk = app.Services.GetRequiredService<IContpaqiSdkService>();
        var t0 = DateTime.UtcNow;
        sdk.InicializarSDK(); // Fuerza fInicializaSDK() (lento, ~13s primera vez)
        var t1 = DateTime.UtcNow;
        Console.WriteLine($"[WARMUP] SDK pre-inicializado en {(t1 - t0).TotalSeconds:F1}s");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[WARMUP] Error: {ex.Message}");
    }
});

// Usa ASPNETCORE_URLS / --urls para configurar el puerto. Si no se especifica,
// .NET usa el default (5000). Esto permite levantar múltiples instancias
// del bridge en distintos puertos (5000, 5001, 5002) para paralelizar.
app.Run();