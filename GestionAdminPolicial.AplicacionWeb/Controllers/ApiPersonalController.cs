using Microsoft.AspNetCore.Mvc;
//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

using Asp.Versioning;


namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    /// <summary>
    /// Controlador API para gestionar el personal policial.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiPersonalController : ControllerBase
    {
        private readonly IPersonalPolicialService _personalServicio;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="personalServicio">Servicio de lógica de negocio para personal policial.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiPersonalController(IPersonalPolicialService personalServicio, IMapper mapper)
        {
            _personalServicio = personalServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene el listado completo de personal policial activo.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de personal policial en formato JSON,
        /// lista para ser consumida por componentes como DataTables.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la propiedad <c>data</c> que contiene una lista de <see cref="VMPersonalPolicial"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("Lista")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Lista()
        {
            var lista = await _personalServicio.Lista();
            var vmLista = _mapper.Map<List<VMPersonalPolicial>>(lista);
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        /// <summary>
        /// Obtiene los datos completos de un personal policial para edición, incluyendo armas y domicilios.
        /// </summary>
        /// <remarks>
        /// Este endpoint se utiliza para cargar el formulario de edición de un personal policial.
        /// Devuelve un objeto con todos los datos relacionados, como información personal, armas asignadas y domicilios registrados.
        /// </remarks>
        /// <param name="id">ID único del personal policial a consultar.</param>
        /// <returns>
        /// Retorna un objeto <see cref="VMPersonalPolicial"/> con los datos completos del personal.
        /// </returns>
        /// <response code="200">Datos obtenidos correctamente.</response>
        /// <response code="404">No se encontró el personal con el ID especificado.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("ObtenerPersonalParaEditar/{id}")]
        [ProducesResponseType(typeof(VMPersonalPolicial), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPersonalParaEditar(int id)
        {
            try
            {
                var personal = await _personalServicio.ObtenerPorId(id);
                if (personal == null)
                    return NotFound(new { mensaje = "Personal no encontrado" });

                var vm = _mapper.Map<VMPersonalPolicial>(personal);
                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo registro de personal policial en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un nuevo personal policial, incluyendo su información general,
        /// armas asignadas, domicilios y una imagen opcional. Los datos deben enviarse como un formulario multipart.
        /// </remarks>
        /// <param name="modelo">Cadena JSON con los datos del personal, incluyendo relaciones como armas y domicilios.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{VMPersonalPolicial}"/> con el estado de la operación y el personal creado.
        /// </returns>
        /// <response code="200">El personal fue creado correctamente.</response>
        /// <response code="400">Los datos enviados son inválidos o incompletos.</response>
        /// <response code="500">Error interno del servidor al procesar la creación.</response>
        /// <exception cref="JsonException">Si el modelo JSON no puede ser deserializado correctamente.</exception>
        /// <exception cref="Exception">Cualquier otro error inesperado durante el proceso de creación.</exception>
        [HttpPost("Crear")]
        [ProducesResponseType(typeof(GenericResponse<VMPersonalPolicial>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromForm] IFormFile? foto, [FromForm] string modelo)
        {
            GenericResponse<VMPersonalPolicial> genericResponse = new();

            try
            {
                //Deserializar el modelo recibido
                var vmPersonal = JsonConvert.DeserializeObject<VMPersonalPolicial>(modelo);

                //No manejamos fotos (pero mantenemos la firma por compatibilidad)
                string nombreFoto = null;
                Stream fotoStream = null;

                //Obtener el usuario autenticado
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                    {
                        int idUsuarioInt = int.Parse(idUsuario);
                        vmPersonal.IdUsuario = idUsuarioInt;

                        if (vmPersonal.Armas != null)
                            foreach (var arma in vmPersonal.Armas)
                                arma.IdUsuario = idUsuarioInt;

                        if (vmPersonal.Domicilios != null)
                            foreach (var domicilio in vmPersonal.Domicilios)
                                domicilio.IdUsuario = idUsuarioInt;
                    }
                }

                //Crear el Personal Policial (foto y nombreFoto se envían como null)
                var personalCreado = await _personalServicio.Crear(
                    _mapper.Map<PersonalPolicial>(vmPersonal),
                    fotoStream,
                    nombreFoto
                );

                //Mapear la entidad creada al ViewModel para devolverla
                vmPersonal = _mapper.Map<VMPersonalPolicial>(personalCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmPersonal;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Edita un registro existente de personal policial en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar la información de un personal policial, incluyendo sus datos generales,
        /// armas asignadas, domicilios y una imagen opcional. Los datos deben enviarse como un formulario multipart.
        /// </remarks>
        /// <param name="modelo">Cadena JSON con los datos actualizados del personal, incluyendo relaciones como armas y domicilios.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{VMPersonalPolicial}"/> con el estado de la operación y el personal actualizado.
        /// </returns>
        /// <response code="200">El personal fue editado correctamente.</response>
        /// <response code="400">Los datos enviados son inválidos o no se pudieron deserializar.</response>
        /// <response code="500">Error interno del servidor al procesar la edición.</response>
        /// <exception cref="JsonException">Si el modelo JSON no puede ser deserializado correctamente.</exception>
        /// <exception cref="Exception">Cualquier otro error inesperado durante el proceso de edición.</exception>
        [HttpPut("Editar")]
        [ProducesResponseType(typeof(GenericResponse<VMPersonalPolicial>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromForm] IFormFile? foto, [FromForm] string modelo)
        {

            GenericResponse<VMPersonalPolicial> genericResponse = new();

            try
            {
                var vmPersonal = JsonConvert.DeserializeObject<VMPersonalPolicial>(modelo);

                //No manejamos fotos
                string nombreFoto = null;
                Stream fotoStream = null;

                //Obtener usuario autenticado
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                    {
                        int idUsuarioInt = int.Parse(idUsuario);

                        if (vmPersonal.Armas != null)
                            foreach (var arma in vmPersonal.Armas)
                                if (arma.IdUsuario == 0)
                                    arma.IdUsuario = idUsuarioInt;

                        if (vmPersonal.Domicilios != null)
                            foreach (var domicilio in vmPersonal.Domicilios)
                                if (domicilio.IdUsuario == 0)
                                    domicilio.IdUsuario = idUsuarioInt;
                    }
                }

                //Editar el personal policial (fotoStream y nombreFoto = null)
                var personalEditado = await _personalServicio.Editar(
                    _mapper.Map<PersonalPolicial>(vmPersonal),
                    fotoStream,
                    nombreFoto
                );

                //Mapear de nuevo a ViewModel
                vmPersonal = _mapper.Map<VMPersonalPolicial>(personalEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmPersonal;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message ?? "Error desconocido";
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Traslada a otro destino un personal policial existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar el traslado de un personal policial a una nueva ubicación o dependencia.
        /// El usuario autenticado queda registrado como responsable del traslado.
        /// </remarks>
        /// <param name="idPersonal">ID único del personal policial que será trasladado.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{string}"/> con el estado de la operación y un mensaje de confirmación o error.
        /// </returns>
        /// <response code="200">El traslado fue procesado correctamente.</response>
        /// <response code="400">El ID proporcionado es inválido o no se pudo procesar el traslado.</response>
        /// <response code="500">Error interno del servidor durante el proceso de traslado.</response>
        /// <exception cref="Exception">Cualquier error inesperado durante el traslado.</exception>
        [HttpPut("trasladar/{idPersonal}")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Trasladar(int idPersonal)
        {
            GenericResponse<string> genericResponse = new();

            try
            {
                int idUsuarioInt = 0;
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        idUsuarioInt = int.Parse(idUsuario);
                }

                bool resultado = await _personalServicio.Trasladar(idPersonal, idUsuarioInt);

                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "El personal fue trasladado correctamente."
                    : "No se pudo trasladar al personal policial.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        /// <summary>
        /// Obtiene el listado completo de personal policial que ha sido trasladado.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de personal policial que han sido trasladados,
        /// en formato JSON, lista para ser consumida por componentes como DataTables.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la propiedad <c>data</c> que contiene una lista de <see cref="VMPersonalPolicial"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        [HttpGet("ListaTrasladados")]
        public async Task<IActionResult> ListaTrasladados()
        {
            var lista = await _personalServicio.ListaTrasladados();
            var vmLista = _mapper.Map<List<VMPersonalPolicial>>(lista);

            // Se envía en formato { data: [...] } para DataTables
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        /// <summary>
        /// Restituye a su destino original a un personal policial previamente trasladado.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite revertir el traslado de un personal policial, devolviéndolo a su destino anterior.
        /// El usuario autenticado será registrado como responsable de la restitución.
        /// </remarks>
        /// <param name="idPersonal">Identificador único del personal policial a restituir.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{string}"/> indicando el estado de la operación y un mensaje descriptivo.
        /// </returns>
        /// <response code="200">La restitución se realizó correctamente o se devolvió un mensaje de error controlado.</response>
        /// <response code="400">Solicitud inválida o datos insuficientes para procesar la restitución.</response>
        /// <response code="500">Error interno del servidor al intentar restituir el personal.</response>
        [HttpPut("restituir/{idPersonal}")]
        public async Task<IActionResult> Restituir(int idPersonal)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try
            {
                int idUsuarioInt = 0;
                ClaimsPrincipal claimUser = HttpContext.User;
                if (claimUser.Identity.IsAuthenticated)
                {
                    string idUsuario = claimUser.Claims
                        .Where(c => c.Type == ClaimTypes.NameIdentifier)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    if (!string.IsNullOrEmpty(idUsuario))
                        idUsuarioInt = int.Parse(idUsuario);
                }

                bool resultado = await _personalServicio.Restituir(idPersonal, idUsuarioInt);

                genericResponse.Estado = resultado;
                genericResponse.Mensaje = resultado
                    ? "El personal fue *Restituido* correctamente."
                    : "No se pudo restituir al personal.";
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        } 


    }
}

