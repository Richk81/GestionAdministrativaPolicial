using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class PersonalPolicialService : IPersonalPolicialService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<PersonalPolicial> _repositorio;
        private readonly IReporteService _reporteService; // <-- nuevo 10 NOVIEMBRE


        // Constructor de la clase PersonalPolicialService
        public PersonalPolicialService(
            IGenericRepository<PersonalPolicial> repositorio,
            IReporteService reporteService  // <-- inyectar aquí
            )
        {
            _repositorio = repositorio;
            _reporteService = reporteService;
        }

        // Lista el Personal Policial Activo con paginación, búsqueda y ordenamiento
        public async Task<DataTableResponse<PersonalPolicial>> ListarPaginado(DataTableRequest request)
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation)
                .AsSplitQuery()
                .Where(p => p.Trasladado == false);

            // Total de registros activos
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador)
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.ToLower();
                query = query.Where(p =>
                        (p.ApellidoYnombre != null && p.ApellidoYnombre.ToLower().Contains(search)) ||
                        (p.Dni != null && p.Dni.ToLower().Contains(search)) ||
                        (p.Legajo != null && p.Legajo.ToLower().Contains(search)) ||
                        (p.Especialidad != null && p.Especialidad.ToLower().Contains(search)) ||
                        (p.Telefono != null && p.Telefono.ToLower().Contains(search))
                );
            }

            // Total después de aplicar búsqueda
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginado x Legajo Ascendente
            query = query
                .OrderBy(p => p.Legajo)
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            return new DataTableResponse<PersonalPolicial>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        // Lista el Personal Policial Trasladado con paginación, búsqueda y ordenamiento
        public async Task<DataTableResponse<PersonalPolicial>> ListarPaginadoTrasladados(DataTableRequest request)
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            // Incluye relaciones y filtra solo los trasladados
            query = query
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation)
                .AsSplitQuery()
                .Where(p => p.Trasladado == true);

            // Total de registros trasladados
            int totalRecords = await query.CountAsync();

            // Filtro global (buscador) — adaptado para búsqueda por mes (texto) y año
            if (!string.IsNullOrEmpty(request.Search?.Value))
            {
                string search = request.Search.Value.Trim().ToLower();

                // Diccionario de meses en español
                var meses = new Dictionary<string, int>
                {
                    { "enero", 1 }, { "febrero", 2 }, { "marzo", 3 }, { "abril", 4 },
                    { "mayo", 5 }, { "junio", 6 }, { "julio", 7 }, { "agosto", 8 },
                    { "septiembre", 9 }, { "setiembre", 9 }, // soporta ambas variantes
                    { "octubre", 10 }, { "noviembre", 11 }, { "diciembre", 12 }
                };

                // Detectar si la búsqueda es un mes o un año
                int? mesBuscado = meses.ContainsKey(search) ? meses[search] : (int?)null;
                int? anioBuscado = int.TryParse(search, out int anioTemp) ? anioTemp : (int?)null;

                query = query.Where(p =>
                    (p.ApellidoYnombre != null && p.ApellidoYnombre.ToLower().Contains(search)) ||
                    (p.Dni != null && p.Dni.ToLower().Contains(search)) ||
                    (p.Legajo != null && p.Legajo.ToLower().Contains(search)) ||
                    (p.Telefono != null && p.Telefono.ToLower().Contains(search)) ||

                    // Comparación por mes en texto
                    (mesBuscado.HasValue && p.FechaEliminacion.HasValue &&
                        p.FechaEliminacion.Value.Month == mesBuscado.Value) ||

                    // Comparación por año
                    (anioBuscado.HasValue && p.FechaEliminacion.HasValue &&
                        p.FechaEliminacion.Value.Year == anioBuscado.Value)
                );
            }

            // Total después del filtrado
            int filteredRecords = await query.CountAsync();

            // Ordenamiento y paginación (por FechaEliminacion descendente)
            query = query
                .OrderByDescending(p => p.FechaEliminacion)  // más reciente primero
                .ThenBy(p => p.Legajo)                       // opcional: orden secundario
                .Skip(request.Start)
                .Take(request.Length);

            var data = await query.ToListAsync();

            Console.WriteLine($"Trasladados -> Total: {totalRecords}, Filtrados: {filteredRecords}, Data: {data.Count}");

            // Retorna en formato compatible con DataTables
            return new DataTableResponse<PersonalPolicial>
            {
                Draw = request.Draw,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                Data = data
            };
        }

        //Lógica del método listar PersonalPolicial Activo
        public async Task<List<PersonalPolicial>> Lista()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == false) // activos
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        //Lógica del método listar PersonalPolicial Trasladados (eliminados lógicamente)
        public async Task<List<PersonalPolicial>> ListaTrasladados()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == true) // trasladados
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        //METODO para Crear un NUEVO PERSONAL POLICIAL (registrar nuevo)
        public async Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                //Validar que no exista otro PERSONAL con el mismo Numero de LEGAJO
                if (!string.IsNullOrEmpty(entidad.Legajo))
                {
                    IQueryable<PersonalPolicial> queryExistente = await _repositorio.Consultar(p => p.Legajo == entidad.Legajo);

                    if (await queryExistente.AnyAsync())
                        throw new Exception($"Ya existe un Personal con el N° de LEGAJO: '{entidad.Legajo}'.");
                }

                // Valores por defecto
                entidad.Trasladado = false;
                entidad.FechaEliminacion = null;

                //Si no se usa imagen, dejar los campos en null
                entidad.NombreImagen = null;
                entidad.UrlImagen = null;

                //Normalizar valores en Armas antes de guardar
                if (entidad.Armas != null && entidad.Armas.Any())
                {
                    foreach (var arma in entidad.Armas)
                    {
                        arma.NumeroSerie = string.IsNullOrEmpty(arma.NumeroSerie) ? "No Provista" : arma.NumeroSerie;
                        arma.Marca = string.IsNullOrEmpty(arma.Marca) ? "No Provista" : arma.Marca;
                    }
                }

                //Crear Personal con hijos (Armas + Domicilios) en UNA sola operación
                PersonalPolicial personalCreado = await _repositorio.Crear(entidad);

                if (personalCreado.IdPersonal == 0)
                    throw new TaskCanceledException("No se pudo crear el personal policial.");

                // Registrar el reporte usando el IdUsuario que ya viene en entidad
                if (entidad.IdUsuario.HasValue)
                {
                    await _reporteService.RegistrarReporteAsync(
                        tipoRecurso: "Personal Policial",
                        idRecurso: personalCreado.IdPersonal.ToString(),
                        accion: "Alta",
                        idUsuario: entidad.IdUsuario.Value, // <-- usuario logueado
                        observaciones: "Alta de Personal Policial en el sistema"
                    );
                }

                //Traer la entidad ya con sus relaciones
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == personalCreado.IdPersonal);

                personalCreado = await query
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                return personalCreado ?? throw new Exception("El personal policial no pudo ser recuperado después de crearse.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el personal policial: " + ex.Message, ex);
            }
        }


        //MÉTODO para Editar un PERSONAL POLICIAL existente
        public async Task<PersonalPolicial> Editar(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                //Buscar la entidad actual en BD
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == entidad.IdPersonal);

                PersonalPolicial personalExistente = await query
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (personalExistente == null)
                    throw new Exception("El personal policial no existe en la base de datos.");

                //Actualizar propiedades principales
                personalExistente.Legajo = entidad.Legajo;
                personalExistente.ApellidoYnombre = entidad.ApellidoYnombre;
                personalExistente.Grado = entidad.Grado;
                personalExistente.Chapa = entidad.Chapa;
                personalExistente.Sexo = entidad.Sexo;
                personalExistente.Funcion = entidad.Funcion;
                personalExistente.Horario = entidad.Horario;
                personalExistente.SituacionRevista = entidad.SituacionRevista;
                personalExistente.FechaNacimiento = entidad.FechaNacimiento;
                personalExistente.Telefono = entidad.Telefono;
                personalExistente.TelefonoEmergencia = entidad.TelefonoEmergencia;
                personalExistente.Dni = entidad.Dni;
                personalExistente.SubsidioSalud = entidad.SubsidioSalud;
                personalExistente.EstudiosCurs = entidad.EstudiosCurs;
                personalExistente.EstadoCivil = entidad.EstadoCivil;
                personalExistente.Especialidad = entidad.Especialidad;
                personalExistente.AltaEnDivision = entidad.AltaEnDivision;
                personalExistente.AltaEnPolicia = entidad.AltaEnPolicia;
                personalExistente.DestinoAnterior = entidad.DestinoAnterior;
                personalExistente.Email = entidad.Email;

                //Ya no se maneja imagen: dejar los campos como null
                personalExistente.NombreImagen = null;
                personalExistente.UrlImagen = null;

                // Actualizar Armas existentes
                foreach (var arma in entidad.Armas)
                {
                    var armaExistente = personalExistente.Armas.FirstOrDefault(a => a.IdArma == arma.IdArma);
                    if (armaExistente != null)
                    {
                        armaExistente.Marca = string.IsNullOrEmpty(arma.Marca) ? "No Provista" : arma.Marca;
                        armaExistente.NumeroSerie = string.IsNullOrEmpty(arma.NumeroSerie) ? "No Provista" : arma.NumeroSerie;
                    }
                }

                // ✅ Actualizar Domicilios existentes
                foreach (var dom in entidad.Domicilios)
                {
                    var domExistente = personalExistente.Domicilios.FirstOrDefault(d => d.IdDomicilio == dom.IdDomicilio);
                    if (domExistente != null)
                    {
                        domExistente.CalleBarrio = string.IsNullOrEmpty(dom.CalleBarrio) ? "No Registrado" : dom.CalleBarrio;
                        domExistente.Localidad = string.IsNullOrEmpty(dom.Localidad) ? "No Registrado" : dom.Localidad;
                        domExistente.ComisariaJuris = string.IsNullOrEmpty(dom.ComisariaJuris) ? "No Registrado" : dom.ComisariaJuris;
                    }
                }

                // ✅ Guardar cambios
                bool respuesta = await _repositorio.Editar(personalExistente);

                if (!respuesta)
                    throw new Exception("No se pudo actualizar el personal policial.");

                // ✅ Recargar la entidad actualizada con sus relaciones
                IQueryable<PersonalPolicial> queryFinal = await _repositorio.Consultar(p => p.IdPersonal == personalExistente.IdPersonal);

                PersonalPolicial actualizado = await queryFinal
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if (actualizado == null)
                    throw new Exception("El personal policial se actualizó pero no pudo ser recuperado.");

                return actualizado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el personal policial: " + ex.Message, ex);
            }
        }

        public async Task<bool> Trasladar(int idPersonal, int idUsuario)
        {
            try
            {    // Obtener PersonalPolicial existente
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);
                PersonalPolicial personalExistente = await query.FirstOrDefaultAsync();

                if (personalExistente == null)
                    throw new Exception("El personal policial no existe en la base de datos.");

                // Aquí aplicamos el traslado
                personalExistente.Trasladado = true;
                personalExistente.FechaEliminacion = DateTime.Now;
                personalExistente.IdUsuario = idUsuario;

                // Editar en base de datos
                bool resultado = await _repositorio.Editar(personalExistente);

                if (!resultado)
                    throw new Exception("No se pudo trasladar el personal policial.");

                // Registrar el reporte de Traslado
                await _reporteService.RegistrarReporteAsync(
                    tipoRecurso: "Personal Policial",
                    idRecurso: personalExistente.IdPersonal.ToString(),
                    accion: "Traslado",
                    idUsuario: idUsuario,
                    observaciones: $"El personal {personalExistente.ApellidoYnombre} (Grado: {personalExistente.Grado}, Legajo: {personalExistente.Legajo}) fue trasladado."
                );

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al trasladar personal: " + ex.Message, ex);
            }
        }

        public async Task<bool> Restituir(int idPersonal, int idUsuario)
        {
            try
            {
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);
                PersonalPolicial personal = await query.FirstOrDefaultAsync();

                if (personal == null)
                    throw new Exception("El personal no existe.");

                // 👇 Marcar como no trasladado y registrar quién hizo la acción
                personal.Trasladado = false;
                personal.FechaEliminacion = null;
                personal.IdUsuario = idUsuario;

                return await _repositorio.Editar(personal);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al restituir personal: " + ex.Message, ex);
            }
        }

        public async Task<PersonalPolicial> ObtenerPorId(int idPersonal)
        {
            // Obtenemos IQueryable desde el repositorio
            var query = await _repositorio.Consultar(p => p.IdPersonal == idPersonal);

            // Incluimos las relaciones
            var personal = query
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation)
                .FirstOrDefault(); // materializamos la consulta

            return personal;
        }

        // Para Dashboard - Cantidad de personal activo
        public async Task<IQueryable<PersonalPolicial>> Consultar()
        {
            return await _repositorio.Consultar();
        }
    }
}
