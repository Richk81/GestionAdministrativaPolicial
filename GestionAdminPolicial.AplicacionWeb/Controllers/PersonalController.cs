//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class PersonalController : Controller
    {
        private readonly IPersonalPolicialService _personalServicio;
        private readonly IMapper _mapper;

        public PersonalController(IPersonalPolicialService personalServicio,
            IMapper mapper
            )
        {
            _personalServicio = personalServicio;
            _mapper = mapper;
        }


        public IActionResult Personal()
        {
            return View();
        }

        //NOTA: Realizo todas las --> Peticiones HTTP

        //Metodo Lista de PERSONAL POLICIAL ACTIVOS
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _personalServicio.Lista();

            List<VMPersonalPolicial> vmLista = _mapper.Map<List<VMPersonalPolicial>>(lista);

            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        // Metodo Lista de PERSONAL POLICIAL TRASLADADO
        // GET: /Personal/ListaTrasladados
        [HttpGet]
        public async Task<IActionResult> ListaTrasladados()
        {
            var lista = await _personalServicio.ListaTrasladados();
            var vmLista = _mapper.Map<List<VMPersonalPolicial>>(lista);

            // Se envía en formato { data: [...] } para DataTables
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        //Metodo lista Personal con sus relaciones de Armas y Domicilio para el modal editar
        [HttpGet("ObtenerPersonalParaEditar/{id}")]
        public async Task<IActionResult> ObtenerPersonalParaEditar(int id)
        {
            try
            {
                var personal = await _personalServicio.ObtenerPorId(id);

                if (personal == null)
                    return NotFound(new { mensaje = "Personal no encontrado" });

                // 🔹 Mapeamos a ViewModel para que los nombres coincidan con tu JS
                var vm = _mapper.Map<VMPersonalPolicial>(personal);

                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        //Metodo para CREAR de Personal Policial
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMPersonalPolicial> genericResponse = new GenericResponse<VMPersonalPolicial>();

            try
            {
                // 1. Deserializar el JSON recibido desde el form
                VMPersonalPolicial vmPersonal = JsonConvert.DeserializeObject<VMPersonalPolicial>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                // 2. Procesar imagen si viene en el formulario
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                // 🔑 3. Obtener el usuario logueado
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

                        // Asignamos IdUsuario al personal
                        vmPersonal.IdUsuario = idUsuarioInt;

                        // 🔹 Propagar IdUsuario a armas y domicilios
                        if (vmPersonal.Armas != null)
                        {
                            foreach (var arma in vmPersonal.Armas)
                                arma.IdUsuario = idUsuarioInt;
                        }

                        if (vmPersonal.Domicilios != null)
                        {
                            foreach (var domicilio in vmPersonal.Domicilios)
                                domicilio.IdUsuario = idUsuarioInt;
                        }
                    }
                }

                // 4. Crear entidad con AutoMapper
                PersonalPolicial personalCreado = await _personalServicio.Crear(
                    _mapper.Map<PersonalPolicial>(vmPersonal),
                    fotoStream,
                    nombreFoto
                );

                // 5. Mapear de vuelta al ViewModel
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

        //Metodo para EDITAR un Personal Policial existente
        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMPersonalPolicial> genericResponse = new GenericResponse<VMPersonalPolicial>();

            try
            {
                // 1. Deserializar el JSON recibido desde el form
                VMPersonalPolicial vmPersonal = JsonConvert.DeserializeObject<VMPersonalPolicial>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                // 2. Procesar imagen si viene en el formulario
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                // 3. Obtener el usuario logueado (solo para referencia, no se sobrescribe IdUsuario)
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

                        // 🔹 Propagar IdUsuario a armas y domicilios si no están asignados
                        if (vmPersonal.Armas != null)
                        {
                            foreach (var arma in vmPersonal.Armas)
                            {
                                if (arma.IdUsuario == 0)
                                    arma.IdUsuario = idUsuarioInt;
                            }
                        }

                        if (vmPersonal.Domicilios != null)
                        {
                            foreach (var domicilio in vmPersonal.Domicilios)
                            {
                                if (domicilio.IdUsuario == 0)
                                    domicilio.IdUsuario = idUsuarioInt;
                            }
                        }
                    }
                }

                // 4. Llamar al servicio para editar el personal
                PersonalPolicial personalEditado = await _personalServicio.Editar(
                    _mapper.Map<PersonalPolicial>(vmPersonal),
                    fotoStream,
                    nombreFoto
                );

                // 5. Mapear de vuelta al ViewModel
                vmPersonal = _mapper.Map<VMPersonalPolicial>(personalEditado);

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

        [HttpPut("trasladar/{idPersonal}")]
        public async Task<IActionResult> Trasladar(int idPersonal)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();

            try
            {
                // ✅ Obtener usuario logueado
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

                // ✅ Llamar al servicio para trasladar
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
