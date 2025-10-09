using GestionAdminPolicial.AplicacionWeb.Utilidades.Automapper;
using GestionAdminPolicial.IOC;
using GestionAdminPolicial.AplicacionWeb.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Controladores con configuración JSON
builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

//Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

//Inyección de dependencias
builder.Services.InyectarDependencia(builder.Configuration);

//AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Versionado de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

//Swagger con soporte para versionado
builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"GestionAdminPolicial API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = "Documentación de la API versionada",
        });
    }

    //Comentarios XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

//Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Swagger UI
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                $"API {description.GroupName.ToUpperInvariant()}");
    }
});

//Activa las rutas de tus controladores API
app.MapControllers();

//Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
