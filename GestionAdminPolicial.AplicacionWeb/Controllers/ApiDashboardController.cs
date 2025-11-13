using Asp.Versioning;
//Agrego las referencias que voy a utilizar --- RICHARD
using AutoMapper;
using GestionAdminPolicial.AplicacionWeb.Models; // <-- Importar ResponseLista
using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.AplicacionWeb.Utilidades.Response;
using GestionAdminPolicial.BLL.Implementacion;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{

    /// <summary>
    /// Controlador API para gestionar el DashBoard
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiDashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor del controlador Dashboard.
        /// </summary>
        /// <param name="dashboardService">Servicio de lógica de negocio para el módulo Dashboard.</param>
        public ApiDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }




    }
}
