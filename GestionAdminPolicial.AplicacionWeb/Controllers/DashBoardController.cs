using AutoMapper;
using GestionAdminPolicial.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class DashBoardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IMapper _mapper;

        public DashBoardController(IDashboardService dashboardService, IMapper mapper)
        {
            _dashboardService = dashboardService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
