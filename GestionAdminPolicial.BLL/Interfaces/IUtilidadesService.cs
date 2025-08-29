using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IUtilidadesService
    {
        //Genera codigo para que el usuario pueda loguearse
        string GenerarClave();

        //recibe texto y lo encripta en SHA256
        string ConvertirSha256(string texto);
    }
}
