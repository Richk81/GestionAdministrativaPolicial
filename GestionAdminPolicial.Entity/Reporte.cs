using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Reporte
{
    public int IdReporte { get; set; }

    public string? TipoRecurso { get; set; }

    public string? IdRecurso { get; set; }

    public string? Accion { get; set; }

    public DateTime? FechaAccion { get; set; }

    public int? IdUsuario { get; set; }

    public string? Observaciones { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
