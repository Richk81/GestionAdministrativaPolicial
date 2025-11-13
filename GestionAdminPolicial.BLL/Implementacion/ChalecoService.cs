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
    public class ChalecoService : IChalecoService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Chaleco> _repositorio;
        private readonly IReporteService _reporteService; // <-- nuevo 10 NOVIEMBRE

        // Constructor de la clase ChalecoService
        public ChalecoService(
            IGenericRepository<Chaleco> repositorio,
            IReporteService reporteService  // <-- inyectar aquí
)
        {
            _repositorio = repositorio;
            _reporteService = reporteService;
        }

        // Lista los chalecos Activos con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Chaleco>> ListarPaginado(DataTableRequest request)
        {
            IQueryable<Chaleco> query = await _repositorio.Consultar();

            query = query
                .Include(c => c.IdPersonalNavigation)
                .AsSplitQuery()
                .Where(c => c.Eliminado == false);

            // Total de registros activos
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();
                query = query.Where(c =>
                        (c.SerieChaleco != null && c.SerieChaleco.ToLower().Contains(search)) ||
                        (c.MarcaYmodelo != null && c.MarcaYmodelo.ToLower().Contains(search)) ||
                        (c.IdPersonalNavigation != null && c.IdPersonalNavigation.Legajo != null &&
                            c.IdPersonalNavigation.Legajo.ToLower().Contains(search)) ||
                        (c.IdPersonalNavigation != null && c.IdPersonalNavigation.ApellidoYnombre != null &&
                            c.IdPersonalNavigation.ApellidoYnombre.ToLower().Contains(search))
            );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginado
            query = query
                .OrderBy(c => c.SerieChaleco)
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            return new DataTableResponse<Chaleco>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Lista los chalecos Eliminados con paginado, búsqueda y ordenamiento
        public async Task<DataTableResponse<Chaleco>> ListarPaginadoEliminados(DataTableRequest request)
        {
            IQueryable<Chaleco> query = await _repositorio.Consultar();

            // 🔹 Solo los chalecos eliminados
            query = query
                .Include(c => c.IdPersonalNavigation)
                .AsSplitQuery()
                .Where(c => c.Eliminado == true);

            // Total de registros eliminados
            int totalRecords = await query.CountAsync();

            // 🔍 Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();

                query = query.Where(c =>
                    (c.SerieChaleco != null && c.SerieChaleco.ToLower().Contains(search)) ||
                    (c.MarcaYmodelo != null && c.MarcaYmodelo.ToLower().Contains(search)) ||
                    (c.IdPersonalNavigation != null &&
                        !string.IsNullOrEmpty(c.IdPersonalNavigation.Legajo) &&
                        c.IdPersonalNavigation.Legajo.ToLower().Contains(search)) ||
                    (c.IdPersonalNavigation != null &&
                        !string.IsNullOrEmpty(c.IdPersonalNavigation.ApellidoYnombre) &&
                        c.IdPersonalNavigation.ApellidoYnombre.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento (si querés más dinámico se puede agregar luego)
            query = query
                .OrderByDescending(c => c.FechaEliminacion)
                .Skip(request.Start)
                .Take(request.Length);

            // Ejecución de la consulta
            var data = await query.ToListAsync();

            Console.WriteLine($"[Eliminados] Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Retornar la estructura esperada por DataTables
            return new DataTableResponse<Chaleco>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        //Lista los chalecos Activos con sus relaciones TOTAL sin paginar
        public async Task<List<Chaleco>> Lista()
        {
            IQueryable<Chaleco> query = await _repositorio.Consultar();

            query = query
                .Where(c => c.Eliminado == false) //Chalecos Activos
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdPersonalNavigation); // Ejemplo: si el chaleco pertenece a un personal

            return await query.ToListAsync();
        }

        // Lógica del método listar Chalecos Eliminados (lógicos)
        public async Task<List<Chaleco>> ListaEliminados()
        {
            IQueryable<Chaleco> query = await _repositorio.Consultar();

            query = query
                .Where(c => c.Eliminado == true) // Chalecos eliminados lógicamente
                
                // Incluye relaciones necesarias
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdPersonalNavigation); 

            return await query.ToListAsync();
        }

        // Método para Crear un NUEVO CHALECO
        public async Task<Chaleco> Crear(Chaleco entidad)
        {
            try
            {
                //Validar que no exista otro chaleco con el mismo Numero de Serie
                if (!string.IsNullOrEmpty(entidad.SerieChaleco))
                {
                    IQueryable<Chaleco> queryExistente = await _repositorio.Consultar(c => c.SerieChaleco == entidad.SerieChaleco);

                    if (await queryExistente.AnyAsync())
                        throw new Exception($"Ya existe un chaleco con el N° de Serie: '{entidad.SerieChaleco}'.");
                }

                // Valores por defecto
                entidad.Eliminado = false;
                entidad.FechaRegistro = DateTime.Now;
                entidad.FechaEliminacion = null;

                // Crear chaleco en la base de datos
                Chaleco chalecoCreado = await _repositorio.Crear(entidad);

                if (chalecoCreado.IdChaleco == 0)
                    throw new TaskCanceledException("No se pudo crear el chaleco.");

                // Registrar el reporte usando el IdUsuario que ya viene en entidad
                if (entidad.IdUsuario.HasValue)
                {
                    await _reporteService.RegistrarReporteAsync(
                        tipoRecurso: "Chaleco",
                        idRecurso: chalecoCreado.IdChaleco.ToString(),
                        accion: "Alta",
                        idUsuario: entidad.IdUsuario.Value, // <-- usuario logueado
                        observaciones: "Alta de chaleco en el sistema"
                    );
                }

                // Recuperar la entidad con sus relaciones (Usuario y Personal)
                IQueryable<Chaleco> query = await _repositorio.Consultar(c => c.IdChaleco == chalecoCreado.IdChaleco);

                chalecoCreado = await query
                    .Include(c => c.IdUsuarioNavigation)
                    .Include(c => c.IdPersonalNavigation)
                    .FirstOrDefaultAsync();

                return chalecoCreado ?? throw new Exception("El chaleco no pudo ser recuperado después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el chaleco: " + ex.Message, ex);
            }
        }

        // Metodo para Editar un chaleco existente en la base de datos.
        public async Task<Chaleco> Editar(Chaleco entidad)
        {
            try
            {
                // Recuperar chaleco existente con relaciones
                IQueryable<Chaleco> query = await _repositorio.Consultar(c => c.IdChaleco == entidad.IdChaleco);

                Chaleco chalecoExistente = await query
                    .Include(c => c.IdUsuarioNavigation)
                    .Include(c => c.IdPersonalNavigation)
                    .FirstOrDefaultAsync();

                if (chalecoExistente == null)
                    throw new Exception("El Chaleco no existe en la Base de datos.");

                // Validar que no exista otro chaleco con la misma serie (si se modificó)
                if (!string.IsNullOrEmpty(entidad.SerieChaleco) && entidad.SerieChaleco != chalecoExistente.SerieChaleco)
                {
                    IQueryable<Chaleco> queryDuplicada = await _repositorio.Consultar(c => c.SerieChaleco == entidad.SerieChaleco);
                    if (await queryDuplicada.AnyAsync())
                        throw new Exception($"Ya existe un chaleco con el N° de Serie: '{entidad.SerieChaleco}'.");
                }

                // Actualizar propiedades principales
                chalecoExistente.SerieChaleco = entidad.SerieChaleco;
                chalecoExistente.MarcaYmodelo = entidad.MarcaYmodelo;
                chalecoExistente.Talle = entidad.Talle;
                chalecoExistente.AnoFabricacion = entidad.AnoFabricacion;
                chalecoExistente.AnoVencimiento = entidad.AnoVencimiento;
                chalecoExistente.EstadoChaleco = entidad.EstadoChaleco;
                chalecoExistente.Observaciones = entidad.Observaciones;
                chalecoExistente.IdPersonal = entidad.IdPersonal;
                chalecoExistente.IdUsuario = entidad.IdUsuario;

                // Guardar cambios en la base de datos
                bool respuesta = await _repositorio.Editar(chalecoExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar el chaleco.");

                // Recargar la entidad actualizada con sus relaciones
                IQueryable<Chaleco> queryFinal = await _repositorio.Consultar(c => c.IdChaleco == chalecoExistente.IdChaleco);

                Chaleco actualizado = await queryFinal
                    .Include(c => c.IdUsuarioNavigation)
                    .Include(c => c.IdPersonalNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("El chaleco se actualizó pero no pudo ser recuperado.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el chaleco: " + ex.Message, ex);
            }
        }

        // Método para eliminar (lógicamente) un chaleco
        public async Task<bool> Eliminar(int idChaleco, int idUsuario)
        {
            try
            {
                // Obtener chaleco existente
                IQueryable<Chaleco> query = await _repositorio.Consultar(c => c.IdChaleco == idChaleco);
                Chaleco chalecoExistente = await query.FirstOrDefaultAsync();

                if (chalecoExistente == null)
                    throw new Exception("El chaleco no existe en la base de datos.");

                // Aplicar eliminación lógica
                chalecoExistente.Eliminado = true;
                chalecoExistente.FechaEliminacion = DateTime.Now;
                chalecoExistente.IdUsuario = idUsuario; // registra quién lo eliminó

                // Guardar cambios primero
                bool resultado = await _repositorio.Editar(chalecoExistente);

                if (!resultado)
                    throw new Exception("No se pudo eliminar el chaleco.");

                // Registrar el reporte de baja
                await _reporteService.RegistrarReporteAsync(
                    tipoRecurso: "Chaleco",
                    idRecurso: chalecoExistente.IdChaleco.ToString(),
                    accion: "Baja",
                    idUsuario: idUsuario,
                    observaciones: $"El Chaleco con Serie N° '{chalecoExistente.SerieChaleco}' fue eliminado."
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el chaleco: " + ex.Message, ex);
            }
        }

        /// Restablece un chaleco que fue eliminado lógicamente.
        public async Task<bool> Restablecer(int idChaleco, int idUsuario)
        {
            try
            {
                // Obtener chaleco existente
                IQueryable<Chaleco> query = await _repositorio.Consultar(c => c.IdChaleco == idChaleco);
                Chaleco chalecoExistente = await query.FirstOrDefaultAsync();

                if (chalecoExistente == null)
                    throw new Exception("El chaleco no existe en la base de datos.");

                // Marcar como no eliminado y registrar quién lo restablece
                chalecoExistente.Eliminado = false;
                chalecoExistente.FechaEliminacion = null;
                chalecoExistente.IdUsuario = idUsuario;

                // Guardar cambios
                return await _repositorio.Editar(chalecoExistente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restablecer el chaleco: " + ex.Message, ex);
            }
        }

        // Metodo que Obtiene un chaleco específico por su ID, incluyendo sus relaciones (Usuario y PersonalPolicial).
        public async Task<Chaleco> ObtenerPorId(int idChaleco)
        {
            try
            {
                IQueryable<Chaleco> query = await _repositorio.Consultar(c => c.IdChaleco == idChaleco);

                Chaleco chaleco = await query
                    .Include(c => c.IdUsuarioNavigation)
                    .Include(c => c.IdPersonalNavigation)
                    .FirstOrDefaultAsync();

                if (chaleco == null)
                    throw new Exception($"No se encontró el chaleco con Id {idChaleco}.");

                return chaleco;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el chaleco: " + ex.Message, ex);
            }
        }


        // Método que Asigna o Desasigna un chaleco de forma parcial (PATCH real)
        public async Task<bool> Asignar(int idChaleco, int? idPersonal)
        {
            try
            {
                Chaleco chaleco = await ObtenerPorId(idChaleco);

                if (chaleco == null)
                    throw new Exception("El chaleco no existe.");

                if (idPersonal.HasValue)
                {
                    int? valorId = idPersonal; // Nullable seguro para EF Core

                    bool yaAsignado = await _repositorio.Existe(c =>
                                        c.IdPersonal == valorId &&
                                        !(c.Eliminado ?? false) &&
                                        c.IdChaleco != idChaleco);


                    if (yaAsignado)
                        throw new Exception("Este personal ya tiene un chaleco asignado :xd");

                    chaleco.IdPersonal = valorId;
                }
                else
                {
                    chaleco.IdPersonal = null;
                    chaleco.IdPersonalNavigation = null; // 🔐 Muy importante
                }// 🧪Verificá que realmente sea null
                Console.WriteLine("IdPersonal: " + (chaleco.IdPersonal?.ToString() ?? "null"));
                Console.WriteLine("IdPersonalNavigation: " + (chaleco.IdPersonalNavigation != null ? "NO ES NULL" : "ES NULL"));


                // Actualización parcial: solo IdPersonal
                await _repositorio.ActualizarCampo(chaleco, c => c.IdPersonal);

                return true;
            }
            catch (Exception ex)
            {
                Exception innerEx = ex;
                string mensaje = "";
                while (innerEx != null)
                {
                    mensaje += innerEx.Message + " → ";
                    innerEx = innerEx.InnerException;
                }

                throw new Exception("ERROR al Asignar: " + mensaje);
            }
        }

        public Task<bool> Desasignar(int idChaleco)
        {
            return Asignar(idChaleco, null);
        }

        // Para Dashboard - Cantidad de Chalecos activo
        public async Task<IQueryable<Chaleco>> Consultar()
        {
            return await _repositorio.Consultar();
        }
    }
}
