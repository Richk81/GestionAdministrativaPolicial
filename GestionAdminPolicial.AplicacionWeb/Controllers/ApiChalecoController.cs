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
    /// Controlador API para gestionar a los CHALECOS
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiChalecoController : ControllerBase
    {
        private readonly IChalecoService _chalecoServicio;
        private readonly IMapper _mapper;


        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="chalecoServicio">Servicio de lógica de negocio para Chalecos.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiChalecoController(IChalecoService chalecoServicio, IMapper mapper)
        {
            _chalecoServicio = chalecoServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de chalecos.
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo los chalecos encontrados.
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

            var resultado = await _chalecoServicio.ListarPaginado(request);
            var listaVM = _mapper.Map<List<VMChaleco>>(resultado.Data);

            return Ok(new DataTableResponse<VMChaleco>
            {
                Draw = request.Draw,
                RecordsTotal = resultado.RecordsTotal,
                RecordsFiltered = resultado.RecordsFiltered,
                Data = listaVM
            });
        }

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de chalecos eliminados.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de registros de chalecos eliminados
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
        /// Retorna un objeto JSON con la estructura esperada por DataTables, conteniendo los chalecos eliminados encontrados.
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
                // Llamada al servicio que obtiene los chalecos eliminados
                var resultado = await _chalecoServicio.ListarPaginadoEliminados(request);

                // Mapear a ViewModel si usás AutoMapper
                var listaVM = _mapper.Map<List<VMChaleco>>(resultado.Data);

                // Retornar respuesta compatible con DataTables
                return Ok(new DataTableResponse<VMChaleco>
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
                // Podés loguear ex.Message aquí
                return StatusCode(500, $"Error al obtener los chalecos eliminados: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el listado completo de chalecos activos.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de chalecos en formato JSON,
        /// lista para ser consumida por componentes como DataTables.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la propiedad <c>data</c> que contiene una lista de <see cref="VMChaleco"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("Lista")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Lista()
        {
            try
            {
                // Obtener lista de chalecos activos
                var lista = await _chalecoServicio.Lista();
                var listaVM = _mapper.Map<List<VMChaleco>>(lista);

                // Retornar con la propiedad "data" para DataTables
                return Ok(new { data = listaVM });
            }
            catch (Exception ex)
            {
                // Retornar error 500 con mensaje
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensaje = "Error al obtener el listado de chalecos: " + ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el listado completo de chalecos eliminados lógicamente.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de chalecos eliminados en formato JSON,
        /// lista para ser consumida por componentes como DataTables.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la propiedad <c>data</c> que contiene una lista de <see cref="VMChaleco"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("ListaEliminados")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListaEliminados()
        {
            try
            {
                // Obtener lista de chalecos eliminados
                var lista = await _chalecoServicio.ListaEliminados();
                var listaVM = _mapper.Map<List<VMChaleco>>(lista);

                // Retornar con la propiedad "data" para DataTables
                return Ok(new { data = listaVM });
            }
            catch (Exception ex)
            {
                // Retornar error 500 con mensaje
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensaje = "Error al obtener el listado de chalecos eliminados: " + ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo chaleco en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un chaleco nuevo y devuelve la entidad creada.  
        /// Antes de crear, valida que no exista otro chaleco con el mismo número de serie para evitar duplicados.  
        /// El usuario autenticado será asignado automáticamente al registro.
        /// </remarks>
        /// <param name="modelo">Objeto de tipo <see cref="VMChaleco"/> que contiene los datos del chaleco a crear.</param>
        /// <returns>
        /// Retorna un objeto JSON de tipo <see cref="GenericResponse{VMChaleco}"/> que incluye:
        /// <list type="bullet">
        /// <item><description>Estado de la operación (true/false).</description></item>
        /// <item><description>Mensaje descriptivo en caso de error o validación.</description></item>
        /// <item><description>Objeto <see cref="VMChaleco"/> creado si la operación fue exitosa.</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Operación exitosa: chaleco creado correctamente o error de validación controlado.</response>
        /// <response code="400">Solicitud inválida (por ejemplo, modelo incompleto o mal formado).</response>
        /// <response code="500">Error interno del servidor al intentar crear el chaleco.</response>
        [HttpPost("Crear")]
        [ProducesResponseType(typeof(GenericResponse<VMChaleco>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] VMChaleco modelo)
        {
            GenericResponse<VMChaleco> genericResponse = new();

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

                // Mapear VM → entidad
                var chalecoEntidad = _mapper.Map<Chaleco>(modelo);

                // Crear chaleco usando el servicio (incluye validación de número de serie)
                var chalecoCreado = await _chalecoServicio.Crear(chalecoEntidad);

                // Mapear entidad → VM para devolver
                modelo = _mapper.Map<VMChaleco>(chalecoCreado);

                // Respuesta exitosa
                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                // Captura errores y devuelve mensaje
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            // Siempre devuelve HTTP 200 con el GenericResponse
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Edita un chaleco existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar los datos de un chaleco existente.  
        /// Valida que no exista otro chaleco con el mismo N° de serie.  
        /// Devuelve la entidad actualizada con sus relaciones cargadas (Usuario y Personal, si corresponde).
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMChaleco"/> con los datos a actualizar.</param>
        /// <returns>
        /// Devuelve un objeto JSON de tipo <see cref="GenericResponse{VMChaleco}"/> con:
        /// - <c>Estado</c>: true si se actualizó correctamente, false si hubo error.
        /// - <c>Mensaje</c>: descripción del error si aplica.
        /// - <c>Objeto</c>: el chaleco actualizado.
        /// </returns>
        /// <response code="200">Chaleco editado correctamente o error de validación.</response>
        /// <response code="500">Error interno del servidor al editar el chaleco.</response>
        [HttpPut("Editar")]
        [ProducesResponseType(typeof(GenericResponse<VMChaleco>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromBody] VMChaleco modelo)
        {
            GenericResponse<VMChaleco> genericResponse = new();

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

                // Mapear VM → entidad
                var chalecoEntidad = _mapper.Map<Chaleco>(modelo);

                // Editar chaleco usando servicio
                var chalecoEditado = await _chalecoServicio.Editar(chalecoEntidad);

                // Mapear entidad → VM
                modelo = _mapper.Map<VMChaleco>(chalecoEditado);

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
        /// Elimina lógicamente un chaleco, marcándolo como eliminado y registrando quién realizó la acción.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza una eliminación lógica: no borra físicamente el chaleco,
        /// sino que establece <c>Eliminado = true</c> y registra la fecha de eliminación y el usuario que ejecutó la acción.
        /// </remarks>
        /// <param name="idChaleco">ID del chaleco a eliminar.</param>
        /// <returns>JSON con estado y mensaje de la operación.</returns>
        /// <response code="200">Chaleco eliminado correctamente.</response>
        /// <response code="400">Error de validación o chaleco no encontrado.</response>
        /// <response code="500">Error interno del servidor al eliminar el chaleco.</response>
        [HttpPatch("Eliminar/{idChaleco}")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int idChaleco)
        {
            GenericResponse<string> genericResponse = new();

            try
            {
                int idUsuarioInt = 0;
                ClaimsPrincipal claimUser = HttpContext.User;

                // 🔹 Verificar si el usuario está autenticado y obtener su ID
                if (claimUser.Identity?.IsAuthenticated == true)
                {
                    string? idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        idUsuarioInt = int.Parse(idUsuario);
                }

                // 🔹 Llamada al servicio de chaleco (registro de eliminación lógica)
                bool resultado = await _chalecoServicio.Eliminar(idChaleco, idUsuarioInt);

                // 🔹 Construcción de respuesta
                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "El chaleco fue eliminado correctamente."
                    : "No se pudo eliminar el chaleco.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        /// <summary>
        /// Restablece un chaleco que fue eliminado lógicamente.
        /// </summary>
        /// <remarks>
        /// Este endpoint marca el chaleco como activo nuevamente, anulando la eliminación lógica
        /// y registrando el usuario que realizó la acción.
        /// </remarks>
        /// <param name="idChaleco">ID del chaleco a restablecer.</param>
        /// <param name="idUsuario">ID del usuario que realiza la acción.</param>
        /// <returns>JSON con estado y mensaje de la operación.</returns>
        /// <response code="200">Chaleco restablecido correctamente.</response>
        /// <response code="400">Error de validación o chaleco no encontrado.</response>
        /// <response code="500">Error interno del servidor al restablecer el chaleco.</response>
        [HttpPatch("Restablecer/{idChaleco}/{idUsuario}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Restablecer(int idChaleco, int idUsuario)
        {
            var response = new GenericResponse<bool>();

            try
            {
                bool resultado = await _chalecoServicio.Restablecer(idChaleco, idUsuario);

                response.Estado = true;
                response.Objeto = resultado;
                response.Mensaje = resultado
                    ? "Chaleco restablecido correctamente."
                    : "No se pudo restablecer el chaleco.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Obtiene un chaleco específico por su ID, incluyendo sus relaciones (Usuario y Personal Policial).
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve la información completa del chaleco solicitado, lista para ser consumida
        /// por componentes como DataTables o formularios de detalle.
        /// </remarks>
        /// <param name="idChaleco">ID del chaleco a consultar.</param>
        /// <returns>JSON con estado, mensaje y objeto del chaleco.</returns>
        /// <response code="200">Chaleco obtenido correctamente.</response>
        /// <response code="400">Chaleco no encontrado o error de validación.</response>
        /// <response code="500">Error interno del servidor al obtener el chaleco.</response>
        [HttpGet("Obtener/{idChaleco}")]
        [ProducesResponseType(typeof(GenericResponse<VMChaleco>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int idChaleco)
        {
            var response = new GenericResponse<VMChaleco>();

            try
            {
                // Obtener chaleco usando el servicio
                var chaleco = await _chalecoServicio.ObtenerPorId(idChaleco);

                // Mapear entidad a ViewModel
                response.Objeto = _mapper.Map<VMChaleco>(chaleco);
                response.Estado = true;
                response.Mensaje = "Chaleco obtenido correctamente.";
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Asigna un chaleco a un personal policial o quita la asignación si ya estaba asignado.
        /// </summary>
        /// <remarks>
        /// Si el chaleco no tiene asignado un personal, el <paramref name="idPersonal"/> será asignado.
        /// Si ya está asignado, se desasigna sin importar el valor de <paramref name="idPersonal"/>.
        /// </remarks>
        /// <param name="idChaleco">ID del chaleco a asignar/desasignar.</param>
        /// <param name="idPersonal">ID del personal a asignar (se ignora si se va a desasignar).</param>
        /// <returns>
        /// JSON con estado y mensaje de la operación.
        /// </returns>
        /// <response code="200">Chaleco asignado o desasignado correctamente.</response>
        /// <response code="400">Error de validación o datos incorrectos.</response>
        /// <response code="500">Error interno del servidor al realizar la operación.</response>
        [HttpPatch("Asignar/{idChaleco}/{idPersonal}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Asignar(int idChaleco, int? idPersonal)
        {
            var response = new GenericResponse<bool>();

            try
            {
                bool resultado = await _chalecoServicio.Asignar(idChaleco, idPersonal);

                response.Estado = resultado;
                response.Objeto = resultado;
                response.Mensaje = resultado
                    ? (idPersonal == null ? "Chaleco devuelto correctamente." : "Chaleco asignado correctamente.")
                    : "No se pudo realizar la operación.";
            }
            catch (Exception ex)
            {
                // Captura mensaje amigable del servicio
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }


        /// <summary>
        /// Desasigna un chaleco, quitando su relación con el personal policial asignado.
        /// </summary>
        /// <remarks>
        /// Este método elimina la asignación del chaleco identificado por <paramref name="idChaleco"/>.
        /// Si el chaleco no está actualmente asignado, la operación no genera error y devuelve un estado exitoso.
        /// </remarks>
        /// <param name="idChaleco">ID del chaleco a desasignar.</param>
        /// <returns>
        /// JSON con el estado de la operación.
        /// </returns>
        /// <response code="200">Chaleco desasignado correctamente o ya se encontraba disponible.</response>
        /// <response code="400">Error de validación o datos incorrectos.</response>
        /// <response code="500">Error interno del servidor al realizar la operación.</response>
        [HttpPatch("Desasignar/{idChaleco}")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Desasignar(int idChaleco)
        {
            try
            {
                bool resultado = await _chalecoServicio.Desasignar(idChaleco);
                return Ok(new { estado = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { estado = false, mensaje = ex.Message });
            }
        }


    }
}
