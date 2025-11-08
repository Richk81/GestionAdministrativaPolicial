using Asp.Versioning;
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    /// <summary>
    /// Controlador API para gestionar Home-Acceso
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiHomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="usuarioService">Servicio de lógica de negocio para Usuarios.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiHomeController(IUsuarioService usuarioService, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene la información del usuario actualmente autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint obtiene el ID del usuario desde los claims de la sesión y devuelve un objeto <see cref="VMUsuario"/> mapeado.
        /// La respuesta se encapsula dentro de un <see cref="GenericResponse{T}"/> indicando el estado y el objeto.
        /// </remarks>
        /// <returns>
        /// Retorna un objeto JSON con la estructura:
        /// <code>
        /// {
        ///     "estado": true | false,
        ///     "mensaje": "Información adicional en caso de error",
        ///     "objeto": { ...datos del usuario... }
        /// }
        /// </code>
        /// </returns>
        /// <response code="200">Información del usuario obtenida correctamente.</response>
        /// <response code="500">Error interno al obtener los datos del usuario.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si falla la obtención del usuario desde el servicio.</exception>
        [HttpGet("ObtenerUsuario")]
        [ProducesResponseType(typeof(GenericResponse<VMUsuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerPorId(int.Parse(idUsuario)));

                response.Estado = true;
                response.Objeto = usuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Guarda los cambios realizados en el perfil del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint recibe un objeto <see cref="VMUsuario"/> con los datos del perfil que se desean actualizar.
        /// El ID del usuario se obtiene de los claims de la sesión actual y se asegura que solo el propio usuario pueda modificar su información.
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMUsuario"/> con los datos del perfil del usuario.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{T}"/> indicando si la operación fue exitosa.
        /// La propiedad <c>Estado</c> será <c>true</c> si se guardaron los cambios correctamente, y <c>false</c> en caso contrario.
        /// </returns>
        /// <response code="200">Perfil actualizado correctamente o fallo en la actualización.</response>
        /// <response code="500">Error interno al intentar guardar los datos del usuario.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si falla el servicio de actualización de perfil.</exception>
        [HttpPost("GuardarPerfil")]
        [ProducesResponseType(typeof(GenericResponse<VMUsuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo);

                entidad.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usuarioService.GuardarPerfil(entidad);

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        /// <summary>
        /// Permite al usuario autenticado cambiar su contraseña.
        /// </summary>
        /// <remarks>
        /// Este endpoint recibe un objeto <see cref="VMCambiarClave"/> con la contraseña actual y la nueva contraseña.
        /// El ID del usuario se obtiene de los claims de la sesión actual, garantizando que solo el propio usuario pueda cambiar su contraseña.
        /// </remarks>
        /// <param name="modelo">Objeto <see cref="VMCambiarClave"/> que contiene <c>ClaveActual</c> y <c>NuevaClave</c>.</param>
        /// <returns>
        /// Retorna un objeto <see cref="GenericResponse{T}"/> indicando si la operación fue exitosa.
        /// La propiedad <c>Estado</c> será <c>true</c> si la contraseña fue cambiada correctamente, y <c>false</c> en caso contrario.
        /// </returns>
        /// <response code="200">Contraseña cambiada correctamente o fallo en la operación.</response>
        /// <response code="400">Datos inválidos enviados por el usuario (por ejemplo, contraseña actual incorrecta).</response>
        /// <response code="500">Error interno al intentar cambiar la contraseña.</response>
        /// <exception cref="Exception">Puede lanzar una excepción si falla el servicio de cambio de contraseña.</exception>
        [HttpPost("CambiarClave")]
        [ProducesResponseType(typeof(GenericResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                bool resultado = await _usuarioService.CambiarClave(
                    int.Parse(idUsuario),
                    modelo.ClaveActual,
                    modelo.NuevaClave
                    );

                response.Estado = resultado;
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
