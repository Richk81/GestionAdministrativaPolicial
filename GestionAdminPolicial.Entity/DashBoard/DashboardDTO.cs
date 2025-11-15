using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.Entity.DashBoard
{
    public class DashboardDTO
    {
        public int TotalPersonal { get; set; }
        public int TotalChalecos { get; set; }
        public int TotalEscopetas { get; set; }
        public int TotalVehiculos { get; set; }
        public int TotalRadios { get; set; }
    }
}
