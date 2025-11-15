using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.Entity.DashBoard
{
    public class ResumenMensualDTO
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; } = "";
        public int Total { get; set; }
    }
}
