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

    public class ChalecoController : Controller
    {
        private readonly IChalecoService _chalecoServicio;
        private readonly IMapper _mapper;

        public ChalecoController(IChalecoService chalecoServicio,
            IMapper mapper
            )
        {
            _chalecoServicio = chalecoServicio;
            _mapper = mapper;
        }

        public IActionResult Chaleco()
        {
            return View();
        }
    }
}
