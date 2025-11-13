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
    public class RadioService : IRadioService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Radio> _repositorio;
        private readonly IReporteService _reporteService;

        // Constructor de la clase VehiculoService
        public RadioService(
            IGenericRepository<Radio> repositorio,
            IReporteService reporteService  // <-- inyectar aquí
)
        {
            _repositorio = repositorio;
            _reporteService = reporteService;
        }

        // Lista los Radios Activos con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Radio>> ListarPaginado(DataTableRequest request)
        {
            IQueryable<Radio> query = await _repositorio.Consultar();

            query = query
                .AsSplitQuery()
                .Where(r => r.Eliminado == false);

            // Total de registros activos
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();
                query = query.Where(r =>
                    (r.SerieRadio != null && r.SerieRadio.ToLower().Contains(search)) ||
                    (r.MarcayModelo != null && r.MarcayModelo.ToLower().Contains(search)) ||
                    (r.EstadoRadio != null && r.EstadoRadio.ToLower().Contains(search)) ||
                    (r.Tipo != null && r.Tipo.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginado
            query = query
                .OrderBy(r => r.SerieRadio) // ✅ Ordena por SerieRadio ascendente
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            return new DataTableResponse<Radio>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Lista las radios eliminadas con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Radio>> ListarPaginadoEliminados(DataTableRequest request)
        {
            IQueryable<Radio> query = await _repositorio.Consultar();

            // 🔹 Solo las radios eliminadas
            query = query
                .AsSplitQuery()
                .Where(r => r.Eliminado == true);

            // Total de registros eliminados
            int totalRecords = await query.CountAsync();

            // 🔍 Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(r =>
                    (r.SerieRadio != null && r.SerieRadio.ToLower().Contains(search)) ||
                    (r.MarcayModelo != null && r.MarcayModelo.ToLower().Contains(search)) ||
                    (r.EstadoRadio != null && r.EstadoRadio.ToLower().Contains(search)) ||
                    (r.Tipo != null && r.Tipo.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento por fecha de eliminación descendente
            query = query
                .OrderByDescending(r => r.FechaEliminacion)
                .Skip(request.Start)
                .Take(request.Length);

            // Ejecución de la consulta
            var data = await query.ToListAsync();

            Console.WriteLine($"[Radios Eliminadas] Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Retornar la estructura esperada por DataTables
            return new DataTableResponse<Radio>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Método para Crear un NUEVO RADIO
        public async Task<Radio> Crear(Radio entidad)
        {
            try
            {
                // Validar que no exista otro radio con el mismo número de Serie
                if (!string.IsNullOrEmpty(entidad.SerieRadio))
                {
                    IQueryable<Radio> querySerie = await _repositorio.Consultar(r => r.SerieRadio == entidad.SerieRadio);

                    if (await querySerie.AnyAsync())
                        throw new Exception($"Ya existe un radio con el N° de Serie: '{entidad.SerieRadio}'.");
                }

                // Valores por defecto
                entidad.Eliminado = false;
                entidad.FechaRegistro = DateTime.Now;
                entidad.FechaEliminacion = null;

                // Crear el radio en la base de datos
                Radio radioCreado = await _repositorio.Crear(entidad);

                if (radioCreado.IdRadio == 0)
                    throw new TaskCanceledException("No se pudo crear el radio.");

                // Registrar el reporte (alta del recurso)
                if (entidad.IdUsuario.HasValue)
                {
                    await _reporteService.RegistrarReporteAsync(
                        tipoRecurso: "Radio",
                        idRecurso: radioCreado.IdRadio.ToString(),
                        accion: "Alta",
                        idUsuario: entidad.IdUsuario.Value, // usuario logueado
                        observaciones: "Alta de radio en el sistema"
                    );
                }

                // Recuperar la entidad con sus relaciones (Usuario)
                IQueryable<Radio> query = await _repositorio.Consultar(r => r.IdRadio == radioCreado.IdRadio);

                radioCreado = await query
                    .Include(r => r.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                return radioCreado ?? throw new Exception("El radio no pudo ser recuperado después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el radio: " + ex.Message, ex);
            }
        }

        // Método para EDITAR una RADIO existente
        public async Task<Radio> Editar(Radio entidad)
        {
            try
            {
                // Recuperar la radio existente con sus relaciones
                IQueryable<Radio> query = await _repositorio.Consultar(r => r.IdRadio == entidad.IdRadio);

                Radio radioExistente = await query
                    .Include(r => r.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (radioExistente == null)
                    throw new Exception("La radio no existe en la base de datos.");

                // Validar que no exista otra radio con la misma serie (si fue modificada)
                if (!string.IsNullOrEmpty(entidad.SerieRadio) && entidad.SerieRadio != radioExistente.SerieRadio)
                {
                    IQueryable<Radio> queryDuplicada = await _repositorio.Consultar(r => r.SerieRadio == entidad.SerieRadio);
                    if (await queryDuplicada.AnyAsync())
                        throw new Exception($"Ya existe una radio con el N° de Serie: '{entidad.SerieRadio}'.");
                }

                // Actualizar las propiedades principales
                radioExistente.SerieRadio = entidad.SerieRadio;
                radioExistente.MarcayModelo = entidad.MarcayModelo;
                radioExistente.EstadoRadio = entidad.EstadoRadio;
                radioExistente.Tipo = entidad.Tipo;
                radioExistente.Observaciones = entidad.Observaciones;
                radioExistente.IdUsuario = entidad.IdUsuario;

                // Guardar cambios en la base de datos
                bool respuesta = await _repositorio.Editar(radioExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar la radio.");

                // Recargar la entidad actualizada con sus relaciones
                IQueryable<Radio> queryFinal = await _repositorio.Consultar(r => r.IdRadio == radioExistente.IdRadio);

                Radio actualizado = await queryFinal
                    .Include(r => r.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("La radio se actualizó pero no pudo ser recuperada.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar la radio: " + ex.Message, ex);
            }
        }

        // Método para eliminar (lógicamente) una RADIO
        public async Task<bool> Eliminar(int idRadio, int idUsuario)
        {
            try
            {
                // Obtener la radio existente
                IQueryable<Radio> query = await _repositorio.Consultar(r => r.IdRadio == idRadio);
                Radio radioExistente = await query.FirstOrDefaultAsync();

                if (radioExistente == null)
                    throw new Exception("La radio no existe en la base de datos.");

                // Aplicar eliminación lógica
                radioExistente.Eliminado = true;
                radioExistente.FechaEliminacion = DateTime.Now;
                radioExistente.IdUsuario = idUsuario; // registra quién la eliminó

                // Guardar cambios en la base de datos
                bool resultado = await _repositorio.Editar(radioExistente);

                if (!resultado)
                    throw new Exception("No se pudo eliminar la radio.");

                // Registrar el reporte de baja
                await _reporteService.RegistrarReporteAsync(
                    tipoRecurso: "Radio",
                    idRecurso: radioExistente.IdRadio.ToString(),
                    accion: "Baja",
                    idUsuario: idUsuario,
                    observaciones: $"La radio con Serie N° '{radioExistente.SerieRadio}' fue eliminada."
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la radio: " + ex.Message, ex);
            }
        }

        // Restablece una radio que fue eliminada lógicamente.
        public async Task<bool> Restablecer(int idRadio, int idUsuario)
        {
            try
            {
                // Obtener radio existente
                IQueryable<Radio> query = await _repositorio.Consultar(r => r.IdRadio == idRadio);
                Radio radioExistente = await query.FirstOrDefaultAsync();

                if (radioExistente == null)
                    throw new Exception("La radio no existe en la base de datos.");

                // Marcar como no eliminada y registrar quién la restablece
                radioExistente.Eliminado = false;
                radioExistente.FechaEliminacion = null;
                radioExistente.IdUsuario = idUsuario;

                // Guardar cambios
                return await _repositorio.Editar(radioExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restablecer la radio: " + ex.Message, ex);
            }
        }

        // Método que obtiene una radio específica por su ID, incluyendo sus relaciones (Usuario y PersonalPolicial).
        public async Task<Radio> ObtenerPorId(int idRadio)
        {
            try
            {
                IQueryable<Radio> query = await _repositorio.Consultar(r => r.IdRadio == idRadio);

                Radio radio = await query
                    .Include(r => r.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (radio == null)
                    throw new Exception($"No se encontró la radio con Id {idRadio}.");

                return radio;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la radio: " + ex.Message, ex);
            }
        }

        // Para Dashboard - Cantidad de Radios activo
        public async Task<IQueryable<Radio>> Consultar()
        {
            return await _repositorio.Consultar();
        }
    }
}
