using Serilog;
using Serilog.Events;
//using Serilog.Sinks.Console;
using Serilog.Formatting.Compact;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;
using Serilog.Settings.Configuration;
using TelegramBot.Data.EF;
using TelegramBot.Logica;
using TelegramBot.Logica.Interfaces;
using TelegramBot.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog desde appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Reemplaza el logger predeterminado por Serilog
builder.Host.UseSerilog();

builder.Services.AddHealthChecks();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TelegramBotContext>();
builder.Services.AddScoped<IServicioClima, ServicioClimaHttp>();
//builder.Services.AddScoped<IServicioDeSalud, ServicioDeSalud>();
builder.Services.AddScoped<IServicioPreguntas, ServicioPreguntas>();
builder.Services.AddHttpClient<CohereMicroservicioClient>()
    .AddTypedClient((httpClient, sp) =>
    {
        var baseUrl = "https://localhost:32769"; 
        return new CohereMicroservicioClient(httpClient, baseUrl);
    });
builder.Services.AddHttpClient<TelegramBotMicroservicioClient>()
    .AddTypedClient((httpClient, sp) =>
    {
        var baseUrl = "https://localhost:32771"; 
        return new TelegramBotMicroservicioClient(httpClient, baseUrl);
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHealthChecks("/healthz");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Registrar shutdown limpio de Serilog
app.Run();

// Asegurar cierre de Serilog al finalizar la app
Log.CloseAndFlush();