using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.DBContext;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class EscopetaService : IEscopetaService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Escopeta> _repositorio;
        private readonly IReporteService _reporteService;

        // Constructor de la clase VehiculoService
        public EscopetaService(
            IGenericRepository<Escopeta> repositorio,
            IReporteService reporteService  // <-- inyectar aquí
)
        {
            _repositorio = repositorio;
            _reporteService = reporteService;
        }

        // Lista las Escopetas Activas con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Escopeta>> ListarPaginado(DataTableRequest request)
        {
            IQueryable<Escopeta> query = await _repositorio.Consultar();

            query = query
                .AsSplitQuery()
                .Where(e => e.Eliminado == false);

            // Total de registros activos
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();
                query = query.Where(e =>
                    (e.SerieEscopeta != null && e.SerieEscopeta.ToLower().Contains(search)) ||
                    (e.MarcayModelo != null && e.MarcayModelo.ToLower().Contains(search)) ||
                    (e.EstadoEscopeta != null && e.EstadoEscopeta.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginado
            query = query
                .OrderBy(e => e.SerieEscopeta) // ✅ Ordena por SerieEscopeta ascendente
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            return new DataTableResponse<Escopeta>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Lista las escopetas eliminadas con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Escopeta>> ListarPaginadoEliminados(DataTableRequest request)
        {
            IQueryable<Escopeta> query = await _repositorio.Consultar();

            // 🔹 Solo las escopetas eliminadas
            query = query
                .AsSplitQuery()
                .Where(e => e.Eliminado == true);

            // Total de registros eliminados
            int totalRecords = await query.CountAsync();

            // 🔍 Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(e =>
                    (e.SerieEscopeta != null && e.SerieEscopeta.ToLower().Contains(search)) ||
                    (e.MarcayModelo != null && e.MarcayModelo.ToLower().Contains(search)) ||
                    (e.EstadoEscopeta != null && e.EstadoEscopeta.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento por fecha de eliminación descendente
            query = query
                .OrderByDescending(e => e.FechaEliminacion)
                .Skip(request.Start)
                .Take(request.Length);

            // Ejecución de la consulta
            var data = await query.ToListAsync();

            Console.WriteLine($"[Escopetas Eliminadas] Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Retornar la estructura esperada por DataTables
            return new DataTableResponse<Escopeta>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Método para Crear una NUEVA ESCOPETA
        public async Task<Escopeta> Crear(Escopeta entidad)
        {
            try
            {
                // Validar que no exista otra escopeta con el mismo número de serie
                if (!string.IsNullOrEmpty(entidad.SerieEscopeta))
                {
                    IQueryable<Escopeta> queryExistente = await _repositorio.Consultar(e => e.SerieEscopeta == entidad.SerieEscopeta);

                    if (await queryExistente.AnyAsync())
                        throw new Exception($"Ya existe una escopeta con el N° de Serie: '{entidad.SerieEscopeta}'.");
                }

                // Valores por defecto
                entidad.Eliminado = false;
                entidad.FechaRegistro = DateTime.Now;
                entidad.FechaEliminacion = null;

                // Crear escopeta en la base de datos
                Escopeta escopetaCreada = await _repositorio.Crear(entidad);

                if (escopetaCreada.IdEscopeta == 0)
                    throw new TaskCanceledException("No se pudo crear la escopeta.");

                // Registrar el reporte de alta del recurso
                if (entidad.IdUsuario.HasValue)
                {
                    await _reporteService.RegistrarReporteAsync(
                        tipoRecurso: "Escopeta",
                        idRecurso: escopetaCreada.IdEscopeta.ToString(),
                        accion: "Alta",
                        idUsuario: entidad.IdUsuario.Value, // Usuario logueado
                        observaciones: "Alta de escopeta en el sistema"
                    );
                }

                // Recuperar la entidad con sus relaciones (Usuario, si aplica)
                IQueryable<Escopeta> query = await _repositorio.Consultar(e => e.IdEscopeta == escopetaCreada.IdEscopeta);

                escopetaCreada = await query
                    .Include(e => e.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                return escopetaCreada ?? throw new Exception("La escopeta no pudo ser recuperada después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la escopeta: " + ex.Message, ex);
            }
        }

        // Método para EDITAR una ESCOPETA existente
        public async Task<Escopeta> Editar(Escopeta entidad)
        {
            try
            {
                // Recuperar la escopeta existente con sus relaciones
                IQueryable<Escopeta> query = await _repositorio.Consultar(e => e.IdEscopeta == entidad.IdEscopeta);

                Escopeta escopetaExistente = await query
                    .Include(e => e.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (escopetaExistente == null)
                    throw new Exception("La escopeta no existe en la base de datos.");

                // Validar que no exista otra escopeta con el mismo número de serie (si fue modificado)
                if (!string.IsNullOrEmpty(entidad.SerieEscopeta) && entidad.SerieEscopeta != escopetaExistente.SerieEscopeta)
                {
                    IQueryable<Escopeta> queryDuplicada = await _repositorio.Consultar(e => e.SerieEscopeta == entidad.SerieEscopeta);
                    if (await queryDuplicada.AnyAsync())
                        throw new Exception($"Ya existe una escopeta con el N° de Serie: '{entidad.SerieEscopeta}'.");
                }

                // Actualizar las propiedades principales
                escopetaExistente.SerieEscopeta = entidad.SerieEscopeta;
                escopetaExistente.MarcayModelo = entidad.MarcayModelo;
                escopetaExistente.EstadoEscopeta = entidad.EstadoEscopeta;
                escopetaExistente.Observaciones = entidad.Observaciones;
                escopetaExistente.IdUsuario = entidad.IdUsuario;

                // Guardar los cambios en la base de datos
                bool respuesta = await _repositorio.Editar(escopetaExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar la escopeta.");

                // Recargar la entidad actualizada con sus relaciones
                IQueryable<Escopeta> queryFinal = await _repositorio.Consultar(e => e.IdEscopeta == escopetaExistente.IdEscopeta);

                Escopeta actualizado = await queryFinal
                    .Include(e => e.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("La escopeta se actualizó pero no pudo ser recuperada.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar la escopeta: " + ex.Message, ex);
            }
        }

        // Método para eliminar (lógicamente) una ESCOPETA
        public async Task<bool> Eliminar(int idEscopeta, int idUsuario)
        {
            try
            {
                // Obtener la escopeta existente
                IQueryable<Escopeta> query = await _repositorio.Consultar(e => e.IdEscopeta == idEscopeta);
                Escopeta escopetaExistente = await query.FirstOrDefaultAsync();

                if (escopetaExistente == null)
                    throw new Exception("La escopeta no existe en la base de datos.");

                // Aplicar eliminación lógica
                escopetaExistente.Eliminado = true;
                escopetaExistente.FechaEliminacion = DateTime.Now;
                escopetaExistente.IdUsuario = idUsuario; // registra quién la eliminó

                // Guardar cambios en la base de datos
                bool resultado = await _repositorio.Editar(escopetaExistente);

                if (!resultado)
                    throw new Exception("No se pudo eliminar la escopeta.");

                // Registrar el reporte de baja
                await _reporteService.RegistrarReporteAsync(
                    tipoRecurso: "Escopeta",
                    idRecurso: escopetaExistente.IdEscopeta.ToString(),
                    accion: "Baja",
                    idUsuario: idUsuario,
                    observaciones: "Eliminación lógica de la escopeta"
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la escopeta: " + ex.Message, ex);
            }
        }

        // Restablece una escopeta que fue eliminada lógicamente.
        public async Task<bool> Restablecer(int idEscopeta, int idUsuario)
        {
            try
            {
                // Obtener escopeta existente
                IQueryable<Escopeta> query = await _repositorio.Consultar(e => e.IdEscopeta == idEscopeta);
                Escopeta escopetaExistente = await query.FirstOrDefaultAsync();

                if (escopetaExistente == null)
                    throw new Exception("La escopeta no existe en la base de datos.");

                // Marcar como no eliminada y registrar quién la restablece
                escopetaExistente.Eliminado = false;
                escopetaExistente.FechaEliminacion = null;
                escopetaExistente.IdUsuario = idUsuario;

                // Guardar cambios
                return await _repositorio.Editar(escopetaExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restablecer la escopeta: " + ex.Message, ex);
            }
        }

        // Método que obtiene una escopeta específica por su ID, incluyendo sus relaciones (Usuario y PersonalPolicial).
        public async Task<Escopeta> ObtenerPorId(int idEscopeta)
        {
            try
            {
                IQueryable<Escopeta> query = await _repositorio.Consultar(e => e.IdEscopeta == idEscopeta);

                Escopeta escopeta = await query
                    .Include(e => e.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (escopeta == null)
                    throw new Exception($"No se encontró la escopeta con Id {idEscopeta}.");

                return escopeta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la escopeta: " + ex.Message, ex);
            }
        }

    }
}
