using System;
using System.Collections.Generic;

namespace GestionAdminPolicial.Entity;

public partial class PersonalPolicial
{
    public int IdPersonal { get; set; }

    public string? Legajo { get; set; }

    public int? IdUsuario { get; set; }

    public string? ApellidoYnombre { get; set; }

    public string? Grado { get; set; }

    public string? Chapa { get; set; }

    public string? Sexo { get; set; }

    public string? Funcion { get; set; }

    public string? Horario { get; set; }

    public string? SituacionRevista { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Telefono { get; set; }

    public string? TelefonoEmergencia { get; set; }

    public string? Dni { get; set; }

    public string? SubsidioSalud { get; set; }

    public string? EstudiosCurs { get; set; }

    public string? EstadoCivil { get; set; }

    public string? Especialidad { get; set; }

    public DateOnly? AltaEnDivision { get; set; }

    public DateOnly? AltaEnPolicia { get; set; }

    public string? DestinoAnterior { get; set; }

    public string? Email { get; set; }

    public bool? Trasladado { get; set; }

    public string? Detalles { get; set; }

    public string? UrlImagen { get; set; }

    public string? NombreImagen { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaEliminacion { get; set; }

    public virtual ICollection<Arma> Armas { get; set; } = new List<Arma>();

    public virtual ICollection<Chaleco> Chalecos { get; set; } = new List<Chaleco>();

    public virtual ICollection<Domicilio> Domicilios { get; set; } = new List<Domicilio>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
