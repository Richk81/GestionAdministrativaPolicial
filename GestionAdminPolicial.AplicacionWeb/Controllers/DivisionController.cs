using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.AspNetCore.Authorization;



namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class DivisionController : Controller
    {

        private readonly IMapper _mapper;
        private readonly IDependenciaPolService _dependenciaPolService;

        
        //Constructor de la clase DivisionController
        public DivisionController(IMapper mapper, IDependenciaPolService dependenciaPolService)
        {
            _mapper = mapper;
            _dependenciaPolService = dependenciaPolService;
        }


        // Creamos todos los métodos que se necesiten para la vista de la división

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        // Método para obtener la información de la dependencia
        public async Task <IActionResult> Obtener()
        {
            GenericResponse<VMDependenciaPol> genericResponse = new GenericResponse<VMDependenciaPol>();

            try
            {
                VMDependenciaPol vmDependenciaPol = _mapper.Map<VMDependenciaPol>(await _dependenciaPolService.Obtener());

                genericResponse.Estado = true;
                genericResponse.Objeto = vmDependenciaPol;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPost]
        // Método para guardar los cambios en la dependencia
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm] string modelo)
        {
            GenericResponse<VMDependenciaPol> genericResponse = new GenericResponse<VMDependenciaPol>();

            try
            {
                VMDependenciaPol vmDependenciaPol = JsonConvert.DeserializeObject<VMDependenciaPol>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if(logo != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombre_en_codigo, extension);
                    logoStream = logo.OpenReadStream();
                }

                Dependencia dependencia_editado = await _dependenciaPolService.GuardarCambios(
                    _mapper.Map<Dependencia>(vmDependenciaPol),
                    logoStream,
                    nombreLogo);

                vmDependenciaPol = _mapper.Map<VMDependenciaPol>(dependencia_editado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmDependenciaPol;
                //genericResponse.Mensaje = "Los cambios se han guardado correctamente.";
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
