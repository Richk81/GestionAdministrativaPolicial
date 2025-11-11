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
    /// Controlador API para gestionar a las RADIOS
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiRadioController : ControllerBase
    {
        private readonly IRadioService _radioServicio;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="radioServicio">Servicio de lógica de negocio para Chalecos.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiRadioController(IRadioService radioServicio, IMapper mapper)
        {
            _radioServicio = radioServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de radios activos.
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo los radios encontrados.
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

            var resultado = await _radioServicio.ListarPaginado(request);
            var listaVM = _mapper.Map<List<VMRadio>>(resultado.Data);

            return Ok(new DataTableResponse<VMRadio>
            {
                Draw = request.Draw,
                RecordsTotal = resultado.RecordsTotal,
                RecordsFiltered = resultado.RecordsFiltered,
                Data = listaVM
            });
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de radios eliminadas.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de registros de radios eliminadas
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo las radios eliminadas encontradas.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="400">Solicitud inválida (parámetros incorrectos o incompletos).</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">
        /// Puede lanzar una excepción si ocurre un error durante la consulta a la base de datos.
        /// </exception>
        [HttpPost("ListarPaginadoEliminadas")]
        public async Task<IActionResult> ListarPaginadoEliminadas([FromBody] DataTableRequest request)
        {
            if (request == null)
                return BadRequest("El request es nulo.");

            try
            {
                // Llamada al servicio que obtiene las radios eliminadas
                var resultado = await _radioServicio.ListarPaginadoEliminados(request);

                // Mapear a ViewModel con AutoMapper
                var listaVM = _mapper.Map<List<VMRadio>>(resultado.Data);

                // Retornar respuesta compatible con DataTables
                return Ok(new DataTableResponse<VMRadio>
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
                return StatusCode(500, $"Error al obtener las radios eliminadas: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo radio en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un radio nuevo y devuelve la entidad creada.  
        /// Antes de crear, valida que no exista otro radio con el mismo número de serie para evitar duplicados.  
        /// El usuario autenticado será asignado automáticamente al registro, y se generará un reporte de alta del recurso.
        /// </remarks>
        /// <param name="modelo">Objeto de tipo <see cref="VMRadio"/> que contiene los datos del radio a crear.</param>
        /// <returns>
        /// Retorna un objeto JSON de tipo <see cref="GenericResponse{VMRadio}"/> que incluye:
        /// <list type="bullet">
        /// <item><description>Estado de la operación (true/false).</description></item>
        /// <item><description>Mensaje descriptivo en caso de error o validación.</description></item>
        /// <item><description>Objeto <see cref="VMRadio"/> creado si la operación fue exitosa.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Operación exitosa: radio creado correctamente o error de validación controlado.</response>
        /// <response code="400">Solicitud inválida (por ejemplo, modelo incompleto o mal formado).</response>
        /// <response code="500">Error interno del servidor al intentar crear el radio.</response>
        [HttpPost("Crear")]
        [ProducesResponseType(typeof(GenericResponse<VMRadio>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] VMRadio modelo)
        {
            GenericResponse<VMRadio> genericResponse = new();

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
                var radioEntidad = _mapper.Map<Radio>(modelo);

                // Crear el radio usando el servicio (incluye validación de número de serie)
                var radioCreado = await _radioServicio.Crear(radioEntidad);

                // Mapear Entidad → ViewModel para devolver
                modelo = _mapper.Map<VMRadio>(radioCreado);

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
        /// Edita una radio existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar los datos de una radio ya registrada.  
        /// Valida que no exista otra radio con el mismo N° de serie antes de guardar los cambios.  
        /// Devuelve la entidad actualizada con sus relaciones cargadas (Usuario, si corresponde).
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMRadio"/> con los datos a actualizar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{VMRadio}"/> con:
        /// - <c>Estado</c>: true si se actualizó correctamente, false si hubo error.
        /// - <c>Mensaje</c>: descripción del error si aplica.
        /// - <c>Objeto</c>: la radio actualizada.
        /// </returns>
        /// <response code="200">Radio editada correctamente o error de validación.</response>
        /// <response code="500">Error interno del servidor al editar la radio.</response>
        [HttpPut("Editar")]
        [ProducesResponseType(typeof(GenericResponse<VMRadio>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromBody] VMRadio modelo)
        {
            GenericResponse<VMRadio> genericResponse = new();

            try
            {
                // Obtener el usuario autenticado (si aplica)
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity?.IsAuthenticated == true)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        modelo.IdUsuario = int.Parse(idUsuario);
                }

                // Mapear VM → Entidad
                var radioEntidad = _mapper.Map<Radio>(modelo);

                // Editar radio usando servicio
                var radioEditada = await _radioServicio.Editar(radioEntidad);

                // Mapear Entidad → VM
                modelo = _mapper.Map<VMRadio>(radioEditada);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Elimina lógicamente una radio, marcándola como eliminada y registrando quién realizó la acción.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza una eliminación lógica: no borra físicamente la radio,  
        /// sino que establece <c>Eliminado = true</c>, guarda la fecha de eliminación  
        /// y registra el usuario que ejecutó la acción.  
        /// Además, genera un reporte automático de la baja del recurso.
        /// </remarks>
        /// <param name="idRadio">ID de la radio a eliminar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{string}"/> con:  
        /// - <c>Estado</c>: true si se eliminó correctamente, false si hubo error.  
        /// - <c>Mensaje</c>: descripción del resultado de la operación.
        /// </returns>
        /// <response code="200">Radio eliminada correctamente.</response>
        /// <response code="400">Error de validación o radio no encontrada.</response>
        /// <response code="500">Error interno del servidor al eliminar la radio.</response>
        [HttpPatch("Eliminar/{idRadio}")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int idRadio)
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

                // Llamada al servicio de radio para realizar la eliminación lógica
                bool resultado = await _radioServicio.Eliminar(idRadio, idUsuarioInt);

                // Construcción de respuesta
                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "La radio fue eliminada correctamente."
                    : "No se pudo eliminar la radio.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Restablece una radio que fue eliminada lógicamente.
        /// </summary>
        /// <remarks>
        /// Este endpoint marca la radio como activa nuevamente, anulando la eliminación lógica
        /// y registrando el usuario que realizó la acción.
        /// </remarks>
        /// <param name="idRadio">ID de la radio a restablecer.</param>
        /// <param name="idUsuario">ID del usuario que realiza la acción.</param>
        /// <returns>JSON con estado y mensaje de la operación.</returns>
        /// <response code="200">Radio restablecida correctamente.</response>
        /// <response code="400">Error de validación o radio no encontrada.</response>
        /// <response code="500">Error interno del servidor al restablecer la radio.</response>
        [HttpPatch("Restablecer/{idRadio}/{idUsuario}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Restablecer(int idRadio, int idUsuario)
        {
            var response = new GenericResponse<bool>();

            try
            {
                bool resultado = await _radioServicio.Restablecer(idRadio, idUsuario);

                response.Estado = true;
                response.Objeto = resultado;
                response.Mensaje = resultado
                    ? "Radio restablecida correctamente."
                    : "No se pudo restablecer la radio.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Obtiene una radio específica por su ID, incluyendo sus relaciones (Usuario y Personal Policial).
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve la información completa de la radio solicitada, lista para ser consumida
        /// por componentes como DataTables o formularios de detalle.
        /// </remarks>
        /// <param name="idRadio">ID de la radio a consultar.</param>
        /// <returns>JSON con estado, mensaje y objeto de la radio.</returns>
        /// <response code="200">Radio obtenida correctamente.</response>
        /// <response code="400">Radio no encontrada o error de validación.</response>
        /// <response code="500">Error interno del servidor al obtener la radio.</response>
        [HttpGet("Obtener/{idRadio}")]
        [ProducesResponseType(typeof(GenericResponse<VMRadio>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int idRadio)
        {
            var response = new GenericResponse<VMRadio>();

            try
            {
                // Obtener la radio usando el servicio
                var radio = await _radioServicio.ObtenerPorId(idRadio);

                // Mapear entidad a ViewModel
                response.Objeto = _mapper.Map<VMRadio>(radio);
                response.Estado = true;
                response.Mensaje = "Radio obtenida correctamente.";
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
