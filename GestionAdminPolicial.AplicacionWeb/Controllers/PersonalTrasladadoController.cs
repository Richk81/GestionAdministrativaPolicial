//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{
    public class PersonalTrasladadoController : Controller
    {
        private readonly IPersonalPolicialService _personalServicio;
        private readonly IMapper _mapper;

        public PersonalTrasladadoController(IPersonalPolicialService personalServicio, IMapper mapper)
        {
            _personalServicio = personalServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

    }

}
