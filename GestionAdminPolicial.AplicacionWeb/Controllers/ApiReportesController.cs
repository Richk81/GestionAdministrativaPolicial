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
using System.Net;
using System.Security.Claims;

namespace GestionAdminPolicial.AplicacionWeb.Controllers
{

    /// <summary>
    /// Controlador API para la gestión de reportes del sistema.
    /// Permite listar reportes con paginación, búsqueda global y ordenamiento dinámico.
    /// Compatible con DataTables.
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

        // ========================================================================
        // ENDPOINT: Listar reportes (paginado, búsqueda y ordenamiento)
        // ========================================================================

        /// <summary>
        /// Obtiene un listado paginado, filtrado y ordenado de reportes del sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint está diseñado para integrarse con el componente <c>DataTables</c> en el cliente.
        /// Permite realizar búsqueda global, ordenamiento y paginación de los registros de reportes
        /// generados por las acciones de los distintos recursos del sistema (chalecos, radios, etc.).
        /// </remarks>
        /// <param name="request">Objeto con los parámetros de búsqueda, ordenamiento y paginación.</param>
        /// <returns>Listado paginado y filtrado de reportes del sistema.</returns>
        /// <response code="200">Retorna el listado paginado de reportes.</response>
        /// <response code="400">Si el formato del cuerpo de la solicitud es incorrecto.</response>
        /// <response code="500">Si ocurre un error interno en el servidor.</response>
        [HttpPost("ListarPaginado")]
        [ProducesResponseType(typeof(DataTableResponse<Reporte>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListarPaginado([FromBody] DataTableRequest request)
        {
            if (request == null)
                return BadRequest("La solicitud no puede estar vacía.");

            try
            {
                // 🔹 Llamada al servicio
                DataTableResponse<Reporte> result = await _reporteServicio.ListarPaginado(request);

                // 🔹 Validación: si no hay datos
                if (result == null || result.Data == null || !result.Data.Any())
                    return Ok(new DataTableResponse<Reporte>
                    {
                        Draw = request.Draw,
                        RecordsTotal = 0,
                        RecordsFiltered = 0,
                        Data = new List<Reporte>()
                    });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    $"Error al obtener los reportes: {ex.Message}");
            }
        }


    }
}
