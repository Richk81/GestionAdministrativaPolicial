using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

// Repositorio para ordenamiento dinámico
using GestionAdminPolicial.BLL.Utilidades;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly IGenericRepository<Reporte> _repoReporte;
        private readonly IGenericRepository<PersonalPolicial> _repoPersonal;
        private readonly IGenericRepository<Chaleco> _repoChaleco;
        private readonly IGenericRepository<Escopeta> _repoEscopeta;
        private readonly IGenericRepository<Radio> _repoRadio;
        private readonly IGenericRepository<Vehiculo> _repoVehiculo;


        public ReporteService(
            IGenericRepository<Reporte> repoReporte,
            IGenericRepository<PersonalPolicial> repoPersonal,
            IGenericRepository<Chaleco> repoChaleco,
            IGenericRepository<Escopeta> repoEscopeta,
            IGenericRepository<Radio> repoRadio,
            IGenericRepository<Vehiculo> repoVehiculo
        )
        {
            _repoReporte = repoReporte;
            _repoPersonal = repoPersonal;
            _repoChaleco = repoChaleco;
            _repoEscopeta = repoEscopeta;
            _repoRadio = repoRadio;
            _repoVehiculo = repoVehiculo;
        }


        //Metodo para registrar un reporte (lo utilizo en otros servicios)
        //--->No individual por eso no va en el controlador ApiReportesController
        public async Task RegistrarReporteAsync(string tipoRecurso, string idRecurso, string accion, int idUsuario, string observaciones = null)
        {
            var reporte = new Reporte
            {
                TipoRecurso = tipoRecurso,
                IdRecurso = idRecurso,
                Accion = accion,
                FechaAccion = DateTime.Now,
                IdUsuario = idUsuario,
                Observaciones = observaciones
            };

            await _repoReporte.Crear(reporte);
        }

        // método para listar reportes con paginación, búsqueda y orden
        public async Task<DataTableResponse<Reporte>> ListarPaginado(DataTableRequest request)
        {
            var query = await _repoReporte.Consultar();
            query = query.Include(r => r.IdUsuarioNavigation);

            // Total de registros sin filtro
            int totalRecords = await query.CountAsync();

            // Filtro global (búsqueda)
            if (!string.IsNullOrWhiteSpace(request.Search?.Value))
            {
                string searchValue = request.Search.Value.ToLower();

                query = query.Where(r =>
                    (r.TipoRecurso ?? "").ToLower().Contains(searchValue) ||
                    (r.IdRecurso ?? "").ToLower().Contains(searchValue) ||
                    (r.Accion ?? "").ToLower().Contains(searchValue) ||
                    (r.Observaciones ?? "").ToLower().Contains(searchValue) ||
                    (r.IdUsuarioNavigation!.Nombre ?? "").ToLower().Contains(searchValue)
                );
            }

            // Total luego del filtro
            int filteredRecords = await query.CountAsync();

            // Ordenamiento dinámico
            if (request.Order != null && request.Order.Count > 0)
            {
                var order = request.Order.First();
                var columnName = request.Columns[order.Column].Data ?? "FechaAccion";

                query = order.Dir == "asc"
                    ? query.OrderByDynamic(columnName, true)
                    : query.OrderByDynamic(columnName, false);
            }
            else
            {
                query = query.OrderByDescending(r => r.FechaAccion);
            }

            // Paginación
            var data = await query
                .Skip(request.Start)
                .Take(request.Length)
                .ToListAsync();

            // Log de control en consola
            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Respuesta
            return new DataTableResponse<Reporte>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = totalRecords,
                Data = data
            };
        }


    }
}

