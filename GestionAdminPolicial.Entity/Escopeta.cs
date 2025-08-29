using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Escopeta
{
    public int IdEscopeta { get; set; }

    public string? SerieEscopeta { get; set; }

    public int? IdUsuario { get; set; }

    public string? MarcayModelo { get; set; }

    public string? EstadoEscopeta { get; set; }

    public string? Observaciones { get; set; }

    public bool? Eliminado { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
