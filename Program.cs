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

app.Run("http://0.0.0.0:5000");