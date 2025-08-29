using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMDomicilio
    {
        public int IdDomicilio { get; set; }

        public string? CalleBarrio { get; set; }

        public string? Localidad { get; set; }

        public string? ComisariaJuris { get; set; }

        public int? IdPersonal { get; set; }

        public int? IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }
    }
}
