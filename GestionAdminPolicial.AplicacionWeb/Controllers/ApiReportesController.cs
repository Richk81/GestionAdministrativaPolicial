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
    /// Controlador API para Listar Reportes (Paginado)
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ApiReportesController : ControllerBase
    {
        private readonly IReporteService _reporteServicio;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="reporteServicio">Servicio de lógica de negocio para Chalecos.</param>
        /// <param name="mapper">Instancia de AutoMapper.</param>
        public ApiReportesController(IReporteService reporteServicio, IMapper mapper)
        {
            _reporteServicio = reporteServicio;
            _mapper = mapper;
        }


    }
}
