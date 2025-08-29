using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IDependenciaPolService
    {
        Task<Dependencia> Obtener();

        Task<Dependencia> GuardarCambios(Dependencia entidad, Stream Logo = null, string NombreLogo = "");

    }
}
