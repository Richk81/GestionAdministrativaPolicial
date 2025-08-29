using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMReporte
    {
        public int IdReporte { get; set; }

        public string? TipoRecurso { get; set; }

        public string? IdRecurso { get; set; }

        public string? Accion { get; set; }

        public DateTime? FechaAccion { get; set; }

        public int? IdUsuario { get; set; }
        
        public string? NombreUsuario { get; set; }

        public string? Observaciones { get; set; }

        public VMUsuario? Usuario { get; set; }
    }
}
