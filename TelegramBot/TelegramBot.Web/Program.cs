using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;
using Serilog.Settings.Configuration;
using TelegramBot.Data.EF;
using TelegramBot.Logica;
using TelegramBot.Logica.Interfaces;
using TelegramBot.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog lo antes posible
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando configuración de servicios");

    // Servicios generales
    builder.Services.AddControllersWithViews();
    builder.Services.AddHealthChecks();

    // Servicios propios
    builder.Services.AddScoped<TelegramBotContext>();
    builder.Services.AddScoped<IServicioClima, ServicioClimaHttp>();
    builder.Services.AddScoped<IServicioPreguntas, ServicioPreguntas>();

    // Microservicios vía HttpClient
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

    // Configuración del pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
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

    Log.Information("Aplicación iniciada correctamente");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}
