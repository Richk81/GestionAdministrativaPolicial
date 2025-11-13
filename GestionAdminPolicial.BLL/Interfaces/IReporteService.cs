using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IReporteService
    {
        //Metodo para registrar un reporte (lo utilizo en otros servicios)
        //--->No individual por eso no va en el controlador ApiReportesController
        Task RegistrarReporteAsync(string tipoRecurso, string idRecurso, string accion, int idUsuario, string observaciones = null);

        // método para listar reportes con paginación, búsqueda y orden
        Task<DataTableResponse<Reporte>> ListarPaginado(DataTableRequest request);
    }
}
