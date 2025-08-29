using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public int? IdRol { get; set; }

    public string? UrlFoto { get; set; }

    public string? NombreFoto { get; set; }

    public string? Clave { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Arma> Armas { get; set; } = new List<Arma>();

    public virtual ICollection<Chaleco> Chalecos { get; set; } = new List<Chaleco>();

    public virtual ICollection<Domicilio> Domicilios { get; set; } = new List<Domicilio>();

    public virtual ICollection<Escopeta> Escopeta { get; set; } = new List<Escopeta>();

    public virtual Rol? IdRolNavigation { get; set; }

    public virtual ICollection<PersonalPolicial> PersonalPolicials { get; set; } = new List<PersonalPolicial>();

    public virtual ICollection<Radio> Radios { get; set; } = new List<Radio>();

    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
