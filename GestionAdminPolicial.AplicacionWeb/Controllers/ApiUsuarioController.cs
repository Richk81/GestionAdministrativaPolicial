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

using GestionAdminPolicial.AplicacionWeb.Models; // <-- Importar ResponseLista

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    /// <summary>
    /// Controlador API para gestionar a los USUARIOS
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiUsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly IRolService _rolServicio;
        private readonly IMapper _mapper;

        public ApiUsuarioController(IUsuarioService usuarioServicio,
            IRolService rolServicio,
            IMapper mapper
            )
        {
            _usuarioServicio = usuarioServicio;
            _rolServicio = rolServicio;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene el listado completo de roles de los Usuarios: "Administrador" - "Empleado" - "Supervisor"
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de roles en formato JSON,
        /// lista para ser consumida por componentes de la interfaz o para otros servicios.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la lista de <see cref="VMRol"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("ListaRoles")]
        [ProducesResponseType(typeof(List<VMRol>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListaRoles()
        {
            try
            {
                List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServicio.Lista());
                return StatusCode(StatusCodes.Status200OK, vmListaRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al obtener roles", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene el listado completo de usuarios administrativos.
        /// </summary>
        /// <remarks>
        /// Este endpoint devuelve una colección de registros de usuarios en formato JSON,
        /// lista para ser consumida por componentes como DataTables o para otros servicios.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la propiedad <c>Data</c> que contiene una lista de <see cref="VMUsuario"/>.
        /// </returns>
        /// <response code="200">Listado obtenido correctamente.</response>
        /// <response code="500">Error interno del servidor al obtener los datos.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpGet("ListaUsuarios")]
        [ProducesResponseType(typeof(ResponseLista<VMUsuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Lista()
        {
            try
            {
                List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioServicio.Lista());

                // ✅ Envolvemos la lista en ResponseLista<T> para Swagger
                var response = new ResponseLista<VMUsuario>
                {
                    Data = vmUsuarioLista
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Error interno al obtener usuarios", detalle = ex.Message });
            }
        }


        /// <summary>
        /// Crea un nuevo usuario administrativo en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un usuario enviando los datos del usuario en JSON
        /// y, opcionalmente, una foto a través de un formulario multipart/form-data.
        /// También se genera un enlace de plantilla de correo para envío de clave inicial.
        /// </remarks>
        /// <param name="modelo">JSON con los datos del usuario a crear.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{VMUsuario}"/> indicando si la operación fue exitosa,
        /// y el usuario creado en la propiedad <c>Objeto</c>.
        /// </returns>
        /// <response code="200">Usuario creado correctamente.</response>
        /// <response code="500">Error interno del servidor al crear el usuario.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla o la deserialización del modelo falla.</exception>
        [HttpPost("CrearUsuario")]
        [ProducesResponseType(typeof(GenericResponse<VMUsuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromForm] IFormFile? foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                //Ya no usamos foto
                Stream fotoStream = null;
                string nombreFoto = "";

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await _usuarioServicio.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Edita un usuario administrativo existente en el sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite actualizar los datos de un usuario enviando un JSON con la información
        /// y, opcionalmente, una nueva foto mediante un formulario multipart/form-data.
        /// </remarks>
        /// <param name="modelo">JSON con los datos del usuario a actualizar.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{VMUsuario}"/> indicando si la operación fue exitosa,
        /// y el usuario actualizado en la propiedad <c>Objeto</c>.
        /// </returns>
        /// <response code="200">Usuario actualizado correctamente.</response>
        /// <response code="500">Error interno del servidor al editar el usuario.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla o la deserialización del modelo falla.</exception>
        [HttpPut("EditarUsuario")]
        [ProducesResponseType(typeof(GenericResponse<VMUsuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar([FromForm] IFormFile? foto, [FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<VMUsuario>();

            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                //Ya no usamos la foto
                Stream fotoStream = null;
                string nombreFoto = "";

                // Llamada al servicio, foto ignorada
                Usuario usuario_editado = await _usuarioServicio.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmUsuario;

            }
            catch (Exception ex)
            {
                // Atrapamos cualquier error para evitar que ASP.NET devuelva HTML
                genericResponse.Estado = false;
                genericResponse.Mensaje = "Error al actualizar usuario: " + ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        /// <summary>
        /// Elimina un Usuario administrativo del sistema.
        /// NOTA: Se deshabilito éste end point ya que necesito a los usuarios para el modulo reportes.
        /// Se implemento en editar usuario la opción de ACTIVO o NO ACTIVO.
        /// </summary>
        /// <remarks>
        /// Este endpoint realiza la eliminación lógica o física (según implementación) de un usuario
        /// en la base de datos usando su <c>IdUsuario</c>.
        /// </remarks>
        /// <param name="IdUsuario">ID único del usuario que se desea eliminar.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{string}"/> indicando si la operación fue exitosa.
        /// La propiedad <c>Estado</c> será <c>true</c> si se eliminó correctamente.
        /// </returns>
        /// <response code="200">Usuario eliminado correctamente o fallo controlado.</response>
        /// <response code="500">Error interno del servidor al intentar eliminar el usuario.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si el servicio de datos falla.</exception>
        [HttpDelete("EliminarUsuario")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int IdUsuario)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();
            try
            {
                genericResponse.Estado = await _usuarioServicio.Eliminar(IdUsuario);
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
