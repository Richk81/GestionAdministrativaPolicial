using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    [Authorize]

    public class ReportesController : Controller
    {
        private readonly IReporteService _reporteServicio;
        private readonly IMapper _mapper;

        public ReportesController(IReporteService reporteServicio,
            IMapper mapper
            )
        {
            _reporteServicio = reporteServicio;
            _mapper = mapper;
        }

        public IActionResult Reportes()
        {
            return View();
        }
    }
}
