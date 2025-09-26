using SistemaVenta.AplicacionWeb.Utilidades.Automapper;

using SistemaVenta.IOC;

using SistemaVenta.AplicacionWeb.Utilidades.Extenciones; // Para los pdf son estos 3
using DinkToPdf;                                             
using DinkToPdf.Contracts;

using Microsoft.AspNetCore.Authentication.Cookies; // para el login

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login"; // Acceso es el controlador y el login es el metodo
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20); // despues de 20 min sale
    });

builder.Services.ConfigurarDependencias(builder.Configuration); // Register application dependencies
builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // Register AutoMapper profile

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/LibreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter),new SynchronizedConverter(new PdfTools()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
 
app.UseAuthentication(); // se usa la autenticacion

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}") // El programa se ejecuta desde el login
    .WithStaticAssets();

app.Run();
