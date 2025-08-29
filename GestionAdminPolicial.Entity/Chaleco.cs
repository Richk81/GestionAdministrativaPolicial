using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Chaleco
{
    public int IdChaleco { get; set; }

    public string? SerieChaleco { get; set; }

    public int? IdUsuario { get; set; }

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

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
