using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity.DashBoard;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDTO> ObtenerTotalesAsync();

        Task<List<ResumenMensualDTO>> AltasPersonalPorMesAsync(int anio);

        Task<List<ResumenMensualDTO>> TrasladosPersonalPorMesAsync(int anio);
    }
}
