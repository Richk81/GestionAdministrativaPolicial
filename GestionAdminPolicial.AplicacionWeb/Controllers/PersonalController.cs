using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class PersonalController : Controller
    {
        public IActionResult Personal()
        {
            return View();
        }
    }
}
