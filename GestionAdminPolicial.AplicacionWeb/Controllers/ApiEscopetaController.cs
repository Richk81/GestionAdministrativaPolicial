using Asp.Versioning;
//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
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
    /// Controlador API para gestionar las ESCOPETAS
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiEscopetaController : ControllerBase
    {
        private readonly IEscopetaService _escopetaServicio;
        private readonly IMapper _mapper;


        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="escopetaServicio">Servicio de lógica de negocio para Chalecos.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiEscopetaController(IEscopetaService escopetaServicio, IMapper mapper)
        {
            _escopetaServicio = escopetaServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de escopetas activas.
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo las escopetas encontradas.
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

            var resultado = await _escopetaServicio.ListarPaginado(request);
            var listaVM = _mapper.Map<List<VMEscopeta>>(resultado.Data);

            return Ok(new DataTableResponse<VMEscopeta>
            {
                Draw = request.Draw,
                RecordsTotal = resultado.RecordsTotal,
                RecordsFiltered = resultado.RecordsFiltered,
                Data = listaVM
            });
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de escopetas eliminadas.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de registros de escopetas eliminadas
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo las escopetas eliminadas encontradas.
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
                // Llamada al servicio que obtiene las escopetas eliminadas
                var resultado = await _escopetaServicio.ListarPaginadoEliminados(request);

                // Mapear a ViewModel con AutoMapper
                var listaVM = _mapper.Map<List<VMEscopeta>>(resultado.Data);

                // Retornar respuesta compatible con DataTables
                return Ok(new DataTableResponse<VMEscopeta>
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
                return StatusCode(500, $"Error al obtener las escopetas eliminadas: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva escopeta en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar una escopeta nueva y devuelve la entidad creada.  
        /// Antes de crear, valida que no exista otra escopeta con el mismo número de serie para evitar duplicados.  
        /// El usuario autenticado será asignado automáticamente al registro, y se generará un reporte de alta del recurso.
        /// </remarks>
        /// <param name="modelo">Objeto de tipo <see cref="VMEscopeta"/> que contiene los datos de la escopeta a crear.</param>
        /// <returns>
        /// Retorna un objeto JSON de tipo <see cref="GenericResponse{VMEscopeta}"/> que incluye:
        /// <list type="bullet">
        /// <item><description>Estado de la operación (true/false).</description></item>
        /// <item><description>Mensaje descriptivo en caso de error o validación.</description></item>
        /// <item><description>Objeto <see cref="VMEscopeta"/> creado si la operación fue exitosa.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Operación exitosa: escopeta creada correctamente o error de validación controlado.</response>
        /// <response code="400">Solicitud inválida (por ejemplo, modelo incompleto o mal formado).</response>
        /// <response code="500">Error interno del servidor al intentar crear la escopeta.</response>
        [HttpPost("Crear")]
        [ProducesResponseType(typeof(GenericResponse<VMEscopeta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] VMEscopeta modelo)
        {
            GenericResponse<VMEscopeta> genericResponse = new();

            try
            {
                // Obtener el usuario autenticado
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
                var escopetaEntidad = _mapper.Map<Escopeta>(modelo);

                // Crear la escopeta usando el servicio (incluye validación de número de serie)
                var escopetaCreada = await _escopetaServicio.Crear(escopetaEntidad);

                // Mapear Entidad → ViewModel para devolver
                modelo = _mapper.Map<VMEscopeta>(escopetaCreada);

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
        /// Edita una escopeta existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar los datos de una escopeta ya registrada.  
        /// Valida que no exista otra escopeta con el mismo N° de serie antes de guardar los cambios.  
        /// Devuelve la entidad actualizada con sus relaciones cargadas (Usuario, si corresponde).
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMEscopeta"/> con los datos a actualizar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{VMEscopeta}"/> con:
        /// - <c>Estado</c>: true si se actualizó correctamente, false si hubo error.  
        /// - <c>Mensaje</c>: descripción del error si aplica.  
        /// - <c>Objeto</c>: la escopeta actualizada.
        /// </returns>
        /// <response code="200">Escopeta editada correctamente o error de validación.</response>
        /// <response code="500">Error interno del servidor al editar la escopeta.</response>
        [HttpPut("Editar")]
        [ProducesResponseType(typeof(GenericResponse<VMEscopeta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromBody] VMEscopeta modelo)
        {
            GenericResponse<VMEscopeta> genericResponse = new();

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
                var escopetaEntidad = _mapper.Map<Escopeta>(modelo);

                // Editar escopeta usando servicio
                var escopetaEditada = await _escopetaServicio.Editar(escopetaEntidad);

                // Mapear Entidad → VM
                modelo = _mapper.Map<VMEscopeta>(escopetaEditada);

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
        /// Elimina lógicamente una escopeta, marcándola como eliminada y registrando quién realizó la acción.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza una eliminación lógica: no borra físicamente la escopeta,
        /// sino que establece <c>Eliminado = true</c> y registra la fecha de eliminación y el usuario que ejecutó la acción.
        /// </remarks>
        /// <param name="idEscopeta">ID de la escopeta a eliminar.</param>
        /// <returns>JSON con el estado y mensaje de la operación.</returns>
        /// <response code="200">Escopeta eliminada correctamente.</response>
        /// <response code="400">Error de validación o escopeta no encontrada.</response>
        /// <response code="500">Error interno del servidor al eliminar la escopeta.</response>
        [HttpPatch("Eliminar/{idEscopeta}")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int idEscopeta)
        {
            GenericResponse<string> genericResponse = new();

            try
            {
                int idUsuarioInt = 0;
                ClaimsPrincipal claimUser = HttpContext.User;

                // Verificar si el usuario está autenticado y obtener su ID
                if (claimUser.Identity?.IsAuthenticated == true)
                {
                    string? idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        idUsuarioInt = int.Parse(idUsuario);
                }

                // Llamada al servicio de escopeta (registro de eliminación lógica)
                bool resultado = await _escopetaServicio.Eliminar(idEscopeta, idUsuarioInt);

                // Construcción de respuesta
                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "La escopeta fue eliminada correctamente."
                    : "No se pudo eliminar la escopeta.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Restablece una escopeta que fue eliminada lógicamente.
        /// </summary>
        /// <remarks>
        /// Este endpoint marca la escopeta como activa nuevamente, anulando la eliminación lógica
        /// y registrando el usuario que realizó la acción.
        /// </remarks>
        /// <param name="idEscopeta">ID de la escopeta a restablecer.</param>
        /// <param name="idUsuario">ID del usuario que realiza la acción.</param>
        /// <returns>JSON con estado y mensaje de la operación.</returns>
        /// <response code="200">Escopeta restablecida correctamente.</response>
        /// <response code="400">Error de validación o escopeta no encontrada.</response>
        /// <response code="500">Error interno del servidor al restablecer la escopeta.</response>
        [HttpPatch("Restablecer/{idEscopeta}/{idUsuario}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Restablecer(int idEscopeta, int idUsuario)
        {
            var response = new GenericResponse<bool>();

            try
            {
                bool resultado = await _escopetaServicio.Restablecer(idEscopeta, idUsuario);

                response.Estado = true;
                response.Objeto = resultado;
                response.Mensaje = resultado
                    ? "Escopeta restablecida correctamente."
                    : "No se pudo restablecer la escopeta.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Obtiene una escopeta específica por su ID, incluyendo sus relaciones (Usuario y Personal Policial).
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve la información completa de la escopeta solicitada, lista para ser consumida
        /// por componentes como DataTables o formularios de detalle.
        /// </remarks>
        /// <param name="idEscopeta">ID de la escopeta a consultar.</param>
        /// <returns>JSON con estado, mensaje y objeto de la escopeta.</returns>
        /// <response code="200">Escopeta obtenida correctamente.</response>
        /// <response code="400">Escopeta no encontrada o error de validación.</response>
        /// <response code="500">Error interno del servidor al obtener la escopeta.</response>
        [HttpGet("Obtener/{idEscopeta}")]
        [ProducesResponseType(typeof(GenericResponse<VMEscopeta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int idEscopeta)
        {
            var response = new GenericResponse<VMEscopeta>();

            try
            {
                // Obtener la escopeta usando el servicio
                var escopeta = await _escopetaServicio.ObtenerPorId(idEscopeta);

                // Mapear entidad a ViewModel
                response.Objeto = _mapper.Map<VMEscopeta>(escopeta);
                response.Estado = true;
                response.Mensaje = "Escopeta obtenida correctamente.";
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
