using TelegramBot.Data.EF;
using TelegramBot.Logica;
using TelegramBot.Logica.Interfaces;
using TelegramBot.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<TelegramBotContext>();
builder.Services.AddScoped<IServicioClima, ServicioClimaHttp>();
builder.Services.AddScoped<IServicioPreguntas, ServicioPreguntas>();
builder.Services.AddHttpClient<CohereMicroservicioClient>()
    .AddTypedClient((httpClient, sp) =>
    {
        var baseUrl = "https://localhost:7252"; 
        return new CohereMicroservicioClient(httpClient, baseUrl);
    });
builder.Services.AddHttpClient<TelegramBotMicroservicioClient>()
    .AddTypedClient((httpClient, sp) =>
    {
        var baseUrl = "https://localhost:7281"; 
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Privacy}/{id?}");

app.MapControllers();

app.Run();
