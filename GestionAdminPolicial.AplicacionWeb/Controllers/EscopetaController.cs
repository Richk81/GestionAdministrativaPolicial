using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class EscopetaController : Controller
    {
        public IActionResult Escopeta()
        {
            return View();
        }
    }
}
