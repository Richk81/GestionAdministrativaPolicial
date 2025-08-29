using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
