using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GestionAdminPolicial.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.DAL.Implementacion;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.BLL.Implementacion;

namespace GestionAdminPolicial.IOC
{
    public static class Dependenn
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GestionOfPolicialContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"));
            });

            //Trabajamos con la clase Generica donde estan las funciones Genericas
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //Es un ejemplo de como se inyecta una dependencia de un repositorio especifico
            //services.AddScoped<IPersonalRepository, PersonalRepository>();

            services.AddScoped<ICorreoService, CorreoService>();
            
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            
            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();

            services.AddScoped<IDependenciaPolService, DependenciaPolService>();

            services.AddScoped<IMenuService, MenuService>();

            services.AddScoped<IPersonalPolicialService, PersonalPolicialService>();

            services.AddScoped<IChalecoService, ChalecoService>();

            services.AddScoped<IReporteService, ReporteService>();

            services.AddScoped<IRadioService, RadioService>();

            services.AddScoped<IEscopetaService, EscopetaService>();

            services.AddScoped<IVehiculoService, VehiculoService>();
        }
    }
}