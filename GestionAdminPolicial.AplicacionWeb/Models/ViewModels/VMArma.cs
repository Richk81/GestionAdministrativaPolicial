using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMArma
    {
        public int IdArma { get; set; }

        public string? NumeroSerie { get; set; }

        public string? Marca { get; set; }

        public int? IdPersonal { get; set; }

        public int? IdUsuario { get; set; }

        public string? NombreUsuario { get; set; }
    }
}
