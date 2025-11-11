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

    public class EscopetaController : Controller
    {
        private readonly IEscopetaService _escopetaServicio;
        private readonly IMapper _mapper;

        public EscopetaController(IEscopetaService escopetaServicio,
            IMapper mapper
            )
        {
            _escopetaServicio = escopetaServicio;
            _mapper = mapper;
        }

        public IActionResult Escopeta()
        {
            return View();
        }
    }
}
