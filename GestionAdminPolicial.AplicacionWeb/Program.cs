//--n°9 Automapper
using GestionAdminPolicial.AplicacionWeb.Utilidades.Automapper;

using GestionAdminPolicial.IOC;

using GestionAdminPolicial.AplicacionWeb.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar autenticación y autorización
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login"; // Ruta de inicio de sesión
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de expiración de la sesión
    });

// Inyectar dependencias
builder.Services.InyectarDependencia(builder.Configuration);

// Configurar Swagger --n°9 Automapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

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

app.UseAuthentication(); // Añadir autenticación antes de autorización

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}"); // Formulario con el cual inicia la aplicación

app.Run();
