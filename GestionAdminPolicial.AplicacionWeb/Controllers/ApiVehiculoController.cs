using Asp.Versioning;
//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models; // <-- Importar ResponseLista
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Implementacion;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    /// <summary>
    /// Controlador API para gestionar a los Vehiculos
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiVehiculoController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoServicio;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="vehiculoServicio">Servicio de lógica de negocio para Chalecos.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiVehiculoController(IVehiculoService vehiculoServicio,
            IMapper mapper
            )
        {
            _vehiculoServicio = vehiculoServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de vehículos activos.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de registros de manera eficiente
        /// desde el servidor.
        ///
        /// El cuerpo de la solicitud debe contener un objeto <see cref="DataTableRequest"/> con los parámetros
        /// necesarios para el filtrado, orden y paginación.
        ///
        /// La respuesta devuelve un objeto <see cref="DataTableResponse{T}"/> que incluye:
        /// <list type="bullet">
        ///   <item><description><c>draw</c>: Número de solicitud enviado por DataTables.</description></item>
        ///   <item><description><c>recordsTotal</c>: Total de registros existentes sin filtrar.</description></item>
        ///   <item><description><c>recordsFiltered</c>: Total de registros que cumplen el criterio de búsqueda.</description></item>
        ///   <item><description><c>data</c>: Lista de registros paginados en formato JSON.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="request">
        /// Objeto con los parámetros de búsqueda, orden y paginación enviados por DataTables.
        /// </param>
        /// <returns>
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo los vehículos encontrados.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="400">Solicitud inválida (parámetros incorrectos o incompletos).</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">
        /// Puede lanzar una excepción si ocurre un error durante la consulta a la base de datos.
        /// </exception>
        [HttpPost("ListarPaginado")]
        public async Task<IActionResult> ListarPaginado([FromBody] DataTableRequest request)
        {
            if (request == null)
                return BadRequest("El request es nulo.");

            var resultado = await _vehiculoServicio.ListarPaginado(request);
            var listaVM = _mapper.Map<List<VMVehiculo>>(resultado.Data);

            return Ok(new DataTableResponse<VMVehiculo>
            {
                Draw = request.Draw,
                RecordsTotal = resultado.RecordsTotal,
                RecordsFiltered = resultado.RecordsFiltered,
                Data = listaVM
            });
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de vehículos eliminados.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de registros de vehículos eliminados
        /// desde el servidor.
        ///
        /// El cuerpo de la solicitud debe contener un objeto <see cref="DataTableRequest"/> con los parámetros
        /// necesarios para el filtrado, orden y paginación.
        ///
        /// La respuesta devuelve un objeto <see cref="DataTableResponse{T}"/> que incluye:
        /// <list type="bullet">
        ///   <item><description><c>draw</c>: Número de solicitud enviado por DataTables.</description></item>
        ///   <item><description><c>recordsTotal</c>: Total de registros existentes sin filtrar.</description></item>
        ///   <item><description><c>recordsFiltered</c>: Total de registros que cumplen el criterio de búsqueda.</description></item>
        ///   <item><description><c>data</c>: Lista de registros paginados en formato JSON.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="request">
        /// Objeto con los parámetros de búsqueda, orden y paginación enviados por DataTables.
        /// </param>
        /// <returns>
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo los vehículos eliminados encontrados.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="400">Solicitud inválida (parámetros incorrectos o incompletos).</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">
        /// Puede lanzar una excepción si ocurre un error durante la consulta a la base de datos.
        /// </exception>
        [HttpPost("ListarPaginadoEliminados")]
        public async Task<IActionResult> ListarPaginadoEliminados([FromBody] DataTableRequest request)
        {
            if (request == null)
                return BadRequest("El request es nulo.");

            try
            {
                // Llamada al servicio que obtiene los vehículos eliminados
                var resultado = await _vehiculoServicio.ListarPaginadoEliminados(request);

                // Mapear a ViewModel si usás AutoMapper
                var listaVM = _mapper.Map<List<VMVehiculo>>(resultado.Data);

                // Retornar respuesta compatible con DataTables
                return Ok(new DataTableResponse<VMVehiculo>
                {
                    Draw = request.Draw,
                    RecordsTotal = resultado.RecordsTotal,
                    RecordsFiltered = resultado.RecordsFiltered,
                    Data = listaVM
                });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                // Podés registrar el error con tu sistema de logs
                return StatusCode(500, $"Error al obtener los vehículos eliminados: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo vehículo en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un vehículo nuevo y devuelve la entidad creada.  
        /// Antes de crear, valida que no exista otro vehículo con el mismo número TUC o Dominio para evitar duplicados.  
        /// El usuario autenticado será asignado automáticamente al registro, y se generará un reporte de alta del recurso.
        /// </remarks>
        /// <param name="modelo">Objeto de tipo <see cref="VMVehiculo"/> que contiene los datos del vehículo a crear.</param>
        /// <returns>
        /// Retorna un objeto JSON de tipo <see cref="GenericResponse{VMVehiculo}"/> que incluye:
        /// <list type="bullet">
        /// <item><description>Estado de la operación (true/false).</description></item>
        /// <item><description>Mensaje descriptivo en caso de error o validación.</description></item>
        /// <item><description>Objeto <see cref="VMVehiculo"/> creado si la operación fue exitosa.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Operación exitosa: vehículo creado correctamente o error de validación controlado.</response>
        /// <response code="400">Solicitud inválida (por ejemplo, modelo incompleto o mal formado).</response>
        /// <response code="500">Error interno del servidor al intentar crear el vehículo.</response>
        [HttpPost("Crear")]
        [ProducesResponseType(typeof(GenericResponse<VMVehiculo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] VMVehiculo modelo)
        {
            GenericResponse<VMVehiculo> genericResponse = new();

            try
            {
                // 🔹 Obtener el usuario autenticado
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        modelo.IdUsuario = int.Parse(idUsuario);
                }

                // Mapear ViewModel → Entidad
                var vehiculoEntidad = _mapper.Map<Vehiculo>(modelo);

                // Crear el vehículo usando el servicio (incluye validación TUC y Dominio)
                var vehiculoCreado = await _vehiculoServicio.Crear(vehiculoEntidad);

                // Mapear Entidad → ViewModel para devolver
                modelo = _mapper.Map<VMVehiculo>(vehiculoCreado);

                // Respuesta exitosa
                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                // Captura y retorna errores controlados
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            // Siempre devuelve HTTP 200 con el GenericResponse (control centralizado)
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Edita un vehículo existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar los datos de un vehículo registrado en el sistema.  
        /// Valida que no exista otro vehículo con el mismo número de TUC (si fue modificado).  
        /// Devuelve la entidad actualizada con sus relaciones cargadas (Usuario, si corresponde).
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMVehiculo"/> con los datos a actualizar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{VMVehiculo}"/> con:
        /// - <c>Estado</c>: true si se actualizó correctamente, false si hubo error.  
        /// - <c>Mensaje</c>: descripción del error si aplica.  
        /// - <c>Objeto</c>: el vehículo actualizado.
        /// </returns>
        /// <response code="200">Vehículo editado correctamente o error de validación controlado.</response>
        /// <response code="400">Solicitud inválida (por ejemplo, modelo incompleto o mal formado).</response>
        /// <response code="500">Error interno del servidor al editar el vehículo.</response>
        [HttpPut("Editar")]
        [ProducesResponseType(typeof(GenericResponse<VMVehiculo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromBody] VMVehiculo modelo)
        {
            GenericResponse<VMVehiculo> genericResponse = new();

            try
            {
                // 🔹 Obtener el usuario autenticado
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        modelo.IdUsuario = int.Parse(idUsuario);
                }

                // Mapear ViewModel → Entidad
                var vehiculoEntidad = _mapper.Map<Vehiculo>(modelo);

                // Editar vehículo usando el servicio (incluye validación de TUC duplicado)
                var vehiculoEditado = await _vehiculoServicio.Editar(vehiculoEntidad);

                // Mapear Entidad → ViewModel para devolver
                modelo = _mapper.Map<VMVehiculo>(vehiculoEditado);

                // Respuesta exitosa
                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                // Captura y retorna errores controlados
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            // Devuelve HTTP 200 con el objeto de respuesta genérica
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Elimina lógicamente un vehículo, marcándolo como eliminado y registrando quién realizó la acción.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza una eliminación lógica: no borra físicamente el vehículo,  
        /// sino que establece <c>Eliminado = true</c>, guarda la fecha de eliminación  
        /// y registra el usuario que ejecutó la acción.  
        /// Además, se genera un reporte automático del evento de baja.
        /// </remarks>
        /// <param name="idVehiculo">ID del vehículo a eliminar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{string}"/> con:  
        /// - <c>Estado</c>: true si se eliminó correctamente, false si hubo error.  
        /// - <c>Mensaje</c>: descripción del resultado de la operación.
        /// </returns>
        /// <response code="200">Vehículo eliminado correctamente.</response>
        /// <response code="400">Error de validación o vehículo no encontrado.</response>
        /// <response code="500">Error interno del servidor al eliminar el vehículo.</response>
        [HttpPatch("Eliminar/{idVehiculo}")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int idVehiculo)
        {
            GenericResponse<string> genericResponse = new();

            try
            {
                int idUsuarioInt = 0;
                ClaimsPrincipal claimUser = HttpContext.User;

                // Obtener el ID del usuario autenticado (si aplica)
                if (claimUser.Identity?.IsAuthenticated == true)
                {
                    string? idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        idUsuarioInt = int.Parse(idUsuario);
                }

                // Llamada al servicio para eliminar lógicamente el vehículo
                bool resultado = await _vehiculoServicio.Eliminar(idVehiculo, idUsuarioInt);

                // Construcción de la respuesta
                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "El vehículo fue eliminado correctamente."
                    : "No se pudo eliminar el vehículo.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Restablece un vehículo que fue eliminado lógicamente.
        /// </summary>
        /// <remarks>
        /// Este endpoint marca el vehículo como activo nuevamente, anulando la eliminación lógica
        /// y registrando el usuario que realizó la acción.
        /// </remarks>
        /// <param name="idVehiculo">ID del vehículo a restablecer.</param>
        /// <param name="idUsuario">ID del usuario que realiza la acción.</param>
        /// <returns>JSON con estado y mensaje de la operación.</returns>
        /// <response code="200">Vehículo restablecido correctamente.</response>
        /// <response code="400">Error de validación o vehículo no encontrado.</response>
        /// <response code="500">Error interno del servidor al restablecer el vehículo.</response>
        [HttpPatch("Restablecer/{idVehiculo}/{idUsuario}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Restablecer(int idVehiculo, int idUsuario)
        {
            var response = new GenericResponse<bool>();

            try
            {
                bool resultado = await _vehiculoServicio.Restablecer(idVehiculo, idUsuario);

                response.Estado = true;
                response.Objeto = resultado;
                response.Mensaje = resultado
                    ? "Vehículo restablecido correctamente."
                    : "No se pudo restablecer el vehículo.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Obtiene un vehículo específico por su ID, incluyendo sus relaciones (Usuario y Personal Policial).
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve la información completa del vehículo solicitado, lista para ser consumida
        /// por componentes como DataTables o formularios de detalle.
        /// </remarks>
        /// <param name="idVehiculo">ID del vehículo a consultar.</param>
        /// <returns>JSON con estado, mensaje y objeto del vehículo.</returns>
        /// <response code="200">Vehículo obtenido correctamente.</response>
        /// <response code="400">Vehículo no encontrado o error de validación.</response>
        /// <response code="500">Error interno del servidor al obtener el vehículo.</response>
        [HttpGet("Obtener/{idVehiculo}")]
        [ProducesResponseType(typeof(GenericResponse<VMVehiculo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int idVehiculo)
        {
            var response = new GenericResponse<VMVehiculo>();

            try
            {
                // Obtener vehículo usando el servicio
                var vehiculo = await _vehiculoServicio.ObtenerPorId(idVehiculo);

                // Mapear entidad a ViewModel
                response.Objeto = _mapper.Map<VMVehiculo>(vehiculo);
                response.Estado = true;
                response.Mensaje = "Vehículo obtenido correctamente.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}
