using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Arma
{
    public int IdArma { get; set; }

    public int? IdUsuario { get; set; }

    public string? NumeroSerie { get; set; }

    public string? Marca { get; set; }

    public int? IdPersonal { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual PersonalPolicial? IdPersonalNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
