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
    public class PersonalTrasladadoController : Controller
    {
        private readonly IPersonalPolicialService _personalServicio;
        private readonly IMapper _mapper;

        public PersonalTrasladadoController(IPersonalPolicialService personalServicio, IMapper mapper)
        {
            _personalServicio = personalServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
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
