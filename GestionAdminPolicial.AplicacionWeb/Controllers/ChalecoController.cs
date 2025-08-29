using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class ChalecoController : Controller
    {

        public IActionResult Chaleco()
        {
            return View();
        }
    }
}
