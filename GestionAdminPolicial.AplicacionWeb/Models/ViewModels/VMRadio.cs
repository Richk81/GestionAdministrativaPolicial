using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMRadio
    {
        public int IdRadio { get; set; }

        public string? SerieRadio { get; set; }

        public string? MarcayModelo { get; set; }

        public string? EstadoRadio { get; set; }

        public string? Tipo { get; set; }

        public string? Observaciones { get; set; }

        public bool? Eliminado { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public DateTime? FechaEliminacion { get; set; }

        public int? IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }
    }
}
