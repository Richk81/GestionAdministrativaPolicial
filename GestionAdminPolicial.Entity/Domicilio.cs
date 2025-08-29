using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Domicilio
{
    public int IdDomicilio { get; set; }

    public int? IdUsuario { get; set; }

    public string? CalleBarrio { get; set; }

    public string? Localidad { get; set; }

    public string? ComisariaJuris { get; set; }

    public int? IdPersonal { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual PersonalPolicial? IdPersonalNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
