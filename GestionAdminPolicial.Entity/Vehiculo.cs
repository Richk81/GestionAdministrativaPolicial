using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public string? Tuc { get; set; }

    public int? IdUsuario { get; set; }

    public string? Tipo { get; set; }

    public string? Dominio { get; set; }

    public string? MarcayModelo { get; set; }

    public string? MotorNumero { get; set; }

    public string? ChasisNumero { get; set; }

    public DateOnly? AñoFabricacion { get; set; }

    public string? EstadoVehiculo { get; set; }

    public string? LugarDeReparacion { get; set; }

    public string? Observaciones { get; set; }

    public string? KmActual { get; set; }

    public string? UltimoService { get; set; }

    public bool? Eliminado { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
