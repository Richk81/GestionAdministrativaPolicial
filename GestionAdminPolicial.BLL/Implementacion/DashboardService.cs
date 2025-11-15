using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionAdminPolicial.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;


using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using System;
using System.Linq;
using System.Threading.Tasks;

// Repositorio para ordenamiento dinámico
using GestionAdminPolicial.BLL.Utilidades;
using GestionAdminPolicial.Entity.DashBoard;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class DashboardService : IDashboardService
    {

        // Repositorios: asumimos que tu repo devuelve IQueryable y tiene método Consultar(...)
        private readonly IGenericRepository<PersonalPolicial> _repoPersonal;
        private readonly IGenericRepository<Chaleco> _repoChalecos;
        private readonly IGenericRepository<Vehiculo> _repoVehiculos;
        private readonly IGenericRepository<Escopeta> _repoEscopetas;
        private readonly IGenericRepository<Radio> _repoRadios;

        public DashboardService(
           IGenericRepository<PersonalPolicial> repoPersonal,
           IGenericRepository<Chaleco> repoChalecos,
           IGenericRepository<Vehiculo> repoVehiculos,
           IGenericRepository<Escopeta> repoEscopetas,
           IGenericRepository<Radio> repoRadios
       )
        {
            _repoPersonal = repoPersonal;
            _repoChalecos = repoChalecos;
            _repoVehiculos = repoVehiculos;
            _repoEscopetas = repoEscopetas;
            _repoRadios = repoRadios;
        }

        // TOTALES DEL DASHBOARD
        public async Task<DashboardDTO> ObtenerTotalesAsync()
        {
            var personal = await (await _repoPersonal.Consultar()).ToListAsync();
            var chalecos = await (await _repoChalecos.Consultar()).ToListAsync();
            var vehiculos = await (await _repoVehiculos.Consultar()).ToListAsync();
            var escopetas = await (await _repoEscopetas.Consultar()).ToListAsync();
            var radios = await (await _repoRadios.Consultar()).ToListAsync();

            // Ahora devolvemos totales reales
            return new DashboardDTO
            {
                TotalPersonal = personal.Where(p => p.Trasladado != true).Count(),
                TotalChalecos = chalecos.Where(c => c.Eliminado != true).Count(),
                TotalVehiculos = vehiculos.Where(v => v.Eliminado != true).Count(),
                TotalEscopetas = escopetas.Where(e => e.Eliminado != true).Count(),
                TotalRadios = radios.Where(r => r.Eliminado != true).Count()
            };
        }

        // ALTAS DE PERSONAL POR MES
        public async Task<List<ResumenMensualDTO>> AltasPersonalPorMesAsync(int anio)
        {
            IQueryable<PersonalPolicial> query =
                await _repoPersonal.Consultar(p => p.FechaRegistro.Value.Year == anio);

            var lista = await query
                .GroupBy(p => p.FechaRegistro.Value.Month)
                .Select(g => new ResumenMensualDTO
                {
                    Mes = g.Key,
                    NombreMes = new DateTime(anio, g.Key, 1).ToString("MMMM"),
                    Total = g.Count()
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();

            return lista;
        }

        // TRASLADOS (BAJAS) POR MES
        public async Task<List<ResumenMensualDTO>> TrasladosPersonalPorMesAsync(int anio)
        {
            IQueryable<PersonalPolicial> query =
                await _repoPersonal.Consultar(p => p.Trasladado == true &&
                                                   p.FechaEliminacion.HasValue &&
                                                   p.FechaEliminacion.Value.Year == anio);

            var lista = await query
                .GroupBy(p => p.FechaEliminacion.Value.Month)
                .Select(g => new ResumenMensualDTO
                {
                    Mes = g.Key,
                    NombreMes = new DateTime(anio, g.Key, 1).ToString("MMMM"),
                    Total = g.Count()
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();

            return lista;
        }
    }

}

