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

    public class VehiculoController : Controller
    {
        private readonly IVehiculoService _vehiculoServicio;
        private readonly IMapper _mapper;

        public VehiculoController(IVehiculoService vehiculoServicio,
            IMapper mapper
            )
        {
            _vehiculoServicio = vehiculoServicio;
            _mapper = mapper;
        }

        public IActionResult Vehiculo()
        {
            return View();
        }
    }
}
