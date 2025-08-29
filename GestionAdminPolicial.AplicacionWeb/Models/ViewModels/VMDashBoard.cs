namespace GestionAdminPolicial.AplicacionWeb.Models.ViewModels
{
    public class VMDashBoard
    {
        public int TotalPersonalPolicial { get; set; }
        public int TotalPersonalPolicialActivo { get; set; }
        public int TotalChaleco { get; set; }
        public int TotalEscopeta { get; set; }
        public int TotalRadio { get; set; }
        public int TotalVehiculo { get; set; }
        public int TotalTrasladadosMesActual { get; set; }
        public int TotalTrasladadosMesAnterior { get; set; }
        public int TotalNuevosMesActual { get; set; }
        public int TotalNuevosMesAnterior { get; set; }
    }
}
