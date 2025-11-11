using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly IGenericRepository<Reporte> _reporteRepository;
    

        public ReporteService(
            IGenericRepository<Reporte> repositorio
 
        )
        {
            _reporteRepository = repositorio;

        }


        //Metodo para registrar un reporte (lo utilizo en otros servicios)
        //--->No individual por eso no va en el controlador ApiReportesController
        public async Task RegistrarReporteAsync(string tipoRecurso, string idRecurso, string accion, int idUsuario, string observaciones = null)
        {
            var reporte = new Reporte
            {
                TipoRecurso = tipoRecurso,
                IdRecurso = idRecurso,
                Accion = accion,
                FechaAccion = DateTime.Now,
                IdUsuario = idUsuario,
                Observaciones = observaciones
            };

            await _reporteRepository.Crear(reporte);
        }


    }
}

