using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class RadioController : Controller
    {
        public IActionResult Radio()
        {
            return View();
        }
    }
}
