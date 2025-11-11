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

    public class RadioController : Controller
    {
        private readonly IRadioService _radioServicio;
        private readonly IMapper _mapper;

        public RadioController(IRadioService chalecoServicio,
            IMapper mapper
            )
        {
            _radioServicio = chalecoServicio;
            _mapper = mapper;
        }

        public IActionResult Radio()
        {
            return View();
        }
    }
}
