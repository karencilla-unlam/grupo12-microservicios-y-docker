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
    Log.Information("Iniciando aplicación");

    // Health checks
    builder.Services.AddHealthChecks();

    // Services
    builder.Services.AddControllersWithViews();
    builder.Services.AddScoped<TelegramBotContext>();
    builder.Services.AddHttpClient<ICohereLogica, CohereLogica>();
    builder.Services.AddScoped<IServicioClima, ServicioClimaHttp>();
    builder.Services.AddScoped<IServicioPreguntas, ServicioPreguntas>();
    builder.Services.AddScoped<IServicioTelegramBotClient, ServicioTelegramBotClient>();
    builder.Services.AddScoped<CohereLogica>();
    // builder.Services.AddScoped<IServicioDeSalud, ServicioDeSalud>();

    var app = builder.Build();

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
