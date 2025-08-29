using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IMenuService
    {
       Task<List<Menu>> ObtenerMenus(int idUsuario);

    }
}
