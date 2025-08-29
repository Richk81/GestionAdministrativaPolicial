using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    public class ReportesController : Controller
    {
        public IActionResult Reportes()
        {
            return View();
        }
    }
}
