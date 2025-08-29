using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Dependencia
{
    public int IdDependencia { get; set; }

    public string? UrlLogo { get; set; }

    public string? NombreLogo { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }
}
