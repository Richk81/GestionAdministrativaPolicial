using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using Newtonsoft.Json;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;

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

                // 3. Crear entidad con AutoMapper
                PersonalPolicial personalCreado = await _personalServicio.Crear(
                    _mapper.Map<PersonalPolicial>(vmPersonal),
                    fotoStream,
                    nombreFoto
                );

                // 4. Mapear de vuelta al ViewModel
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


    }
}
