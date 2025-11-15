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
    [Authorize]
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

        /// <summary>
        /// Obtiene los totales de todos los recursos del sistema.
        /// </summary>
        /// <remarks>
        /// Retorna la cantidad total de Personal, Chalecos, Escopetas, Radios y Vehículos.
        /// </remarks>
        /// <returns>Objeto con los totales de recursos.</returns>
        [HttpGet("totales")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotales()
        {
            var data = await _dashboardService.ObtenerTotalesAsync();
            return Ok(data);
        }

        /// <summary>
        /// Obtiene la cantidad de altas de personal por mes.
        /// </summary>
        /// <param name="anio">Año a consultar (Ej: 2024)</param>
        /// <returns>Lista con altas por mes.</returns>
        [HttpGet("altas-personal/{anio}")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAltas(int anio)
        {
            var data = await _dashboardService.AltasPersonalPorMesAsync(anio);
            return Ok(data);
        }


        /// <summary>
        /// Obtiene la cantidad de traslados (bajas) de personal por mes.
        /// </summary>
        /// <param name="anio">Año a consultar</param>
        /// <returns>Lista de traslados por mes.</returns>
        [HttpGet("traslados-personal/{anio}")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTraslados(int anio)
        {
            var data = await _dashboardService.TrasladosPersonalPorMesAsync(anio);
            return Ok(data);
        }
    }
}
