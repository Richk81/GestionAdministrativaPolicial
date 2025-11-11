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
        Task RegistrarReporteAsync(string tipoRecurso, string idRecurso, string accion, int idUsuario, string observaciones = null);

    }
}
