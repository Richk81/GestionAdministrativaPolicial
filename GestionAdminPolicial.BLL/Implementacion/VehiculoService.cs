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
    public class VehiculoService : IVehiculoService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Vehiculo> _repositorio;
        private readonly IReporteService _reporteService;

        // Constructor de la clase VehiculoService
        public VehiculoService(
            IGenericRepository<Vehiculo> repositorio,
            IReporteService reporteService  // <-- inyectar aquí
)
        {
            _repositorio = repositorio;
            _reporteService = reporteService;
        }

        // Lista los Vehículos Activos con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Vehiculo>> ListarPaginado(DataTableRequest request)
        {
            IQueryable<Vehiculo> query = await _repositorio.Consultar();

            query = query
                .AsSplitQuery()
                .Where(v => v.Eliminado == false);

            // Total de registros activos
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();
                query = query.Where(v =>
                    (v.Tuc != null && v.Tuc.ToLower().Contains(search)) ||
                    (v.Dominio != null && v.Dominio.ToLower().Contains(search)) ||
                    (v.MarcayModelo != null && v.MarcayModelo.ToLower().Contains(search)) ||
                    (v.MotorNumero != null && v.MotorNumero.ToLower().Contains(search)) ||
                    (v.ChasisNumero != null && v.ChasisNumero.ToLower().Contains(search)) ||
                    (v.EstadoVehiculo != null && v.EstadoVehiculo.ToLower().Contains(search)) ||
                    (v.Tipo != null && v.Tipo.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginado
            query = query
                .OrderBy(v => v.Tuc) // Podés cambiarlo por el campo que prefieras
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            return new DataTableResponse<Vehiculo>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Lista los Vehículos Eliminados con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Vehiculo>> ListarPaginadoEliminados(DataTableRequest request)
        {
            IQueryable<Vehiculo> query = await _repositorio.Consultar();

            // 🔹 Solo los vehículos eliminados
            query = query
                .AsSplitQuery()
                .Where(v => v.Eliminado == true);

            // Total de registros eliminados
            int totalRecords = await query.CountAsync();

            // 🔍 Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(v =>
                    (v.Tuc != null && v.Tuc.ToLower().Contains(search)) ||
                    (v.Dominio != null && v.Dominio.ToLower().Contains(search)) ||
                    (v.MarcayModelo != null && v.MarcayModelo.ToLower().Contains(search)) ||
                    (v.Tipo != null && v.Tipo.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // 🔽 Ordenamiento por fecha de eliminación descendente (más recientes primero)
            query = query
                .OrderByDescending(v => v.FechaEliminacion)
                .Skip(request.Start)
                .Take(request.Length);

            // Ejecución de la consulta
            var data = await query.ToListAsync();

            Console.WriteLine($"[Vehículos Eliminados] Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Retornar la estructura esperada por DataTables
            return new DataTableResponse<Vehiculo>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Método para Crear un NUEVO VEHÍCULO
        public async Task<Vehiculo> Crear(Vehiculo entidad)
        {
            try
            {
                // Validar que no exista otro vehículo con el mismo TUC o Dominio
                if (!string.IsNullOrEmpty(entidad.Tuc))
                {
                    IQueryable<Vehiculo> queryExistenteTuc = await _repositorio.Consultar(v => v.Tuc == entidad.Tuc);

                    if (await queryExistenteTuc.AnyAsync())
                        throw new Exception($"Ya existe un vehículo con el TUC: '{entidad.Tuc}'.");
                }

                if (!string.IsNullOrEmpty(entidad.Dominio))
                {
                    IQueryable<Vehiculo> queryExistenteDominio = await _repositorio.Consultar(v => v.Dominio == entidad.Dominio);

                    if (await queryExistenteDominio.AnyAsync())
                        throw new Exception($"Ya existe un vehículo con el dominio: '{entidad.Dominio}'.");
                }

                // Valores por defecto
                entidad.Eliminado = false;
                entidad.FechaRegistro = DateTime.Now;
                entidad.FechaEliminacion = null;

                // Crear el vehículo en la base de datos
                Vehiculo vehiculoCreado = await _repositorio.Crear(entidad);

                if (vehiculoCreado.IdVehiculo == 0)
                    throw new TaskCanceledException("No se pudo crear el vehículo.");

                // Registrar el reporte (alta del recurso)
                if (entidad.IdUsuario.HasValue)
                {
                    await _reporteService.RegistrarReporteAsync(
                        tipoRecurso: "Vehículo",
                        idRecurso: vehiculoCreado.IdVehiculo.ToString(),
                        accion: "Alta",
                        idUsuario: entidad.IdUsuario.Value, // usuario logueado
                        observaciones: "Alta de vehículo en el sistema"
                    );
                }

                // Recuperar la entidad con sus relaciones (Usuario)
                IQueryable<Vehiculo> query = await _repositorio.Consultar(v => v.IdVehiculo == vehiculoCreado.IdVehiculo);

                vehiculoCreado = await query
                    .Include(v => v.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                return vehiculoCreado ?? throw new Exception("El vehículo no pudo ser recuperado después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el vehículo: " + ex.Message, ex);
            }
        }

        // Método para EDITAR un VEHÍCULO existente
        public async Task<Vehiculo> Editar(Vehiculo entidad)
        {
            try
            {
                // Recuperar el vehículo existente con sus relaciones
                IQueryable<Vehiculo> query = await _repositorio.Consultar(v => v.IdVehiculo == entidad.IdVehiculo);

                Vehiculo vehiculoExistente = await query
                    .Include(v => v.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (vehiculoExistente == null)
                    throw new Exception("El vehículo no existe en la base de datos.");

                // Validar que no exista otro vehículo con el mismo TUC(si fue modificado)
                if (!string.IsNullOrEmpty(entidad.Tuc) && entidad.Tuc != vehiculoExistente.Tuc)
                {
                    IQueryable<Vehiculo> queryDuplicadoTuc = await _repositorio.Consultar(v => v.Tuc == entidad.Tuc);
                    if (await queryDuplicadoTuc.AnyAsync())
                        throw new Exception($"Ya existe un vehículo con el TUC: '{entidad.Tuc}'.");
                }

                // Actualizar las propiedades principales
                vehiculoExistente.Tuc = entidad.Tuc;
                vehiculoExistente.Tipo = entidad.Tipo;
                vehiculoExistente.Dominio = entidad.Dominio;
                vehiculoExistente.MarcayModelo = entidad.MarcayModelo;
                vehiculoExistente.MotorNumero = entidad.MotorNumero;
                vehiculoExistente.ChasisNumero = entidad.ChasisNumero;
                vehiculoExistente.AñoFabricacion = entidad.AñoFabricacion;
                vehiculoExistente.EstadoVehiculo = entidad.EstadoVehiculo;
                vehiculoExistente.LugarDeReparacion = entidad.LugarDeReparacion;
                vehiculoExistente.Observaciones = entidad.Observaciones;
                vehiculoExistente.KmActual = entidad.KmActual;
                vehiculoExistente.UltimoService = entidad.UltimoService;
                vehiculoExistente.IdUsuario = entidad.IdUsuario;

                // Guardar cambios en la base de datos
                bool respuesta = await _repositorio.Editar(vehiculoExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar el vehículo.");

                // Recargar la entidad actualizada con sus relaciones
                IQueryable<Vehiculo> queryFinal = await _repositorio.Consultar(v => v.IdVehiculo == vehiculoExistente.IdVehiculo);

                Vehiculo actualizado = await queryFinal
                    .Include(v => v.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("El vehículo se actualizó pero no pudo ser recuperado.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el vehículo: " + ex.Message, ex);
            }
        }

        // Método para eliminar (lógicamente) un VEHÍCULO
        public async Task<bool> Eliminar(int idVehiculo, int idUsuario)
        {
            try
            {
                // Obtener el vehículo existente
                IQueryable<Vehiculo> query = await _repositorio.Consultar(v => v.IdVehiculo == idVehiculo);
                Vehiculo vehiculoExistente = await query.FirstOrDefaultAsync();

                if (vehiculoExistente == null)
                    throw new Exception("El vehículo no existe en la base de datos.");

                // Aplicar eliminación lógica
                vehiculoExistente.Eliminado = true;
                vehiculoExistente.FechaEliminacion = DateTime.Now;
                vehiculoExistente.IdUsuario = idUsuario; // registra quién lo eliminó

                // Guardar los cambios
                bool resultado = await _repositorio.Editar(vehiculoExistente);

                if (!resultado)
                    throw new Exception("No se pudo eliminar el vehículo.");

                // Registrar el reporte de baja
                await _reporteService.RegistrarReporteAsync(
                    tipoRecurso: "Vehículo",
                    idRecurso: vehiculoExistente.IdVehiculo.ToString(),
                    accion: "Baja",
                    idUsuario: idUsuario,
                    observaciones: "Eliminación lógica del vehículo"
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el vehículo: " + ex.Message, ex);
            }
        }

        // Restablece un vehículo que fue eliminado lógicamente.
        public async Task<bool> Restablecer(int idVehiculo, int idUsuario)
        {
            try
            {
                // Obtener vehículo existente
                IQueryable<Vehiculo> query = await _repositorio.Consultar(v => v.IdVehiculo == idVehiculo);
                Vehiculo vehiculoExistente = await query.FirstOrDefaultAsync();

                if (vehiculoExistente == null)
                    throw new Exception("El vehículo no existe en la base de datos.");

                // Marcar como no eliminado y registrar quién lo restablece
                vehiculoExistente.Eliminado = false;
                vehiculoExistente.FechaEliminacion = null;
                vehiculoExistente.IdUsuario = idUsuario;

                // Guardar cambios
                return await _repositorio.Editar(vehiculoExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restablecer el vehículo: " + ex.Message, ex);
            }
        }

        // Método que obtiene un vehículo específico por su ID, incluyendo sus relaciones (Usuario y PersonalPolicial).
        public async Task<Vehiculo> ObtenerPorId(int idVehiculo)
        {
            try
            {
                IQueryable<Vehiculo> query = await _repositorio.Consultar(v => v.IdVehiculo == idVehiculo);

                Vehiculo vehiculo = await query
                    .Include(v => v.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (vehiculo == null)
                    throw new Exception($"No se encontró el vehículo con Id {idVehiculo}.");

                return vehiculo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el vehículo: " + ex.Message, ex);
            }
        }

    }
}
