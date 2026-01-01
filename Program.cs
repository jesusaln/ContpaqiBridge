var builder = WebApplication.CreateBuilder(args);

// Configure Proxy/Services
builder.Services.AddControllers();
builder.Services.AddSingleton<ContpaqiBridge.Services.IContpaqiSdkService, ContpaqiBridge.Services.ContpaqiSdkService>();

var app = builder.Build();


app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:5000");
