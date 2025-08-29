using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMChaleco
    {
        public int IdChaleco { get; set; }

        public string? SerieChaleco { get; set; }

        public string? MarcaYmodelo { get; set; }

        public string? Talle { get; set; }

        public DateOnly? AnoFabricacion { get; set; }

        public DateOnly? AnoVencimiento { get; set; }

        public string? EstadoChaleco { get; set; }

        public string? Observaciones { get; set; }

        public int? IdPersonal { get; set; }

        public bool? Eliminado { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public DateTime? FechaEliminacion { get; set; }

        public virtual PersonalPolicial? IdPersonalNavigation { get; set; }

        public int? IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }

    }
}
