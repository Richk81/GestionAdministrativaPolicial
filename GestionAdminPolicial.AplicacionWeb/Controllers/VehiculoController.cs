using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class VehiculoController : Controller
    {
        public IActionResult Vehiculo()
        {
            return View();
        }
    }
}
