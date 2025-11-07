using System;
using System.Collections.Generic;
using GestionAdminPolicial.Entity;
using Microsoft.EntityFrameworkCore;

namespace GestionAdminPolicial.DAL.DBContext;

public partial class GestionOfPolicialContext : DbContext
{
    public GestionOfPolicialContext()
    {
    }

    public GestionOfPolicialContext(DbContextOptions<GestionOfPolicialContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arma> Armas { get; set; }

    public virtual DbSet<Chaleco> Chalecos { get; set; }

    public virtual DbSet<Configuracion> Configuracions { get; set; }

    public virtual DbSet<Dependencia> Dependencia { get; set; }

    public virtual DbSet<Domicilio> Domicilios { get; set; }

    public virtual DbSet<Escopeta> Escopeta { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<PersonalPolicial> PersonalPolicials { get; set; }

    public virtual DbSet<Radio> Radios { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Arma>(entity =>
        {
            entity.HasKey(e => e.IdArma).HasName("PK__Arma__2FC1809CA73E1B2E");

            entity.ToTable("Arma");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.NumeroSerie).HasMaxLength(50);

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.Armas)
                .HasForeignKey(d => d.IdPersonal)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Arma__IdPersonal__5535A963");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Armas)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Arma__IdUsuario__5441852A");
        });

        modelBuilder.Entity<Chaleco>(entity =>
        {
            entity.HasKey(e => e.IdChaleco).HasName("PK__Chaleco__4BA1A544DC72317D");

            entity.ToTable("Chaleco");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.EstadoChaleco).HasMaxLength(50);
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MarcaYmodelo)
                .HasMaxLength(255)
                .HasColumnName("MarcaYModelo");
            entity.Property(e => e.SerieChaleco)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Talle).HasMaxLength(50);

            entity.HasOne(d => d.IdPersonalNavigation)
                .WithMany(p => p.Chalecos)
                .HasForeignKey(d => d.IdPersonal)
                .IsRequired(false) // 👈 Esto es clave
                .HasConstraintName("FK__Chaleco__IdPerso__59FA5E80");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Chalecos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Chaleco__IdUsuar__59063A47");
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Configuracion");

            entity.Property(e => e.Propiedad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("propiedad");
            entity.Property(e => e.Recurso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("recurso");
            entity.Property(e => e.Valor)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<Dependencia>(entity =>
        {
            entity.HasKey(e => e.IdDependencia).HasName("PK__Dependen__A67AC7BE7E613F0D");

            entity.Property(e => e.IdDependencia)
                .ValueGeneratedNever()
                .HasColumnName("idDependencia");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreLogo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreLogo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlLogo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlLogo");
        });

        modelBuilder.Entity<Domicilio>(entity =>
        {
            entity.HasKey(e => e.IdDomicilio).HasName("PK__Domicili__1648AD8AB258AF4E");

            entity.ToTable("Domicilio");

            entity.Property(e => e.CalleBarrio).HasMaxLength(159);
            entity.Property(e => e.ComisariaJuris).HasMaxLength(75);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Localidad).HasMaxLength(159);

            entity.HasOne(d => d.IdPersonalNavigation).WithMany(p => p.Domicilios)
                .HasForeignKey(d => d.IdPersonal)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Domicilio__IdPer__5070F446");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Domicilios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Domicilio__IdUsu__4F7CD00D");
        });

        modelBuilder.Entity<Escopeta>(entity =>
        {
            entity.HasKey(e => e.IdEscopeta).HasName("PK__Escopeta__87DE375C39F35243");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.EstadoEscopeta).HasMaxLength(50);
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MarcayModelo).HasMaxLength(50);
            entity.Property(e => e.SerieEscopeta)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Escopeta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Escopeta__IdUsua__5EBF139D");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__Menu__C26AF483516DA55D");

            entity.ToTable("Menu");

            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.Controlador)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("controlador");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Icono)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("icono");
            entity.Property(e => e.IdMenuPadre).HasColumnName("idMenuPadre");
            entity.Property(e => e.PaginaAccion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("paginaAccion");

            entity.HasOne(d => d.IdMenuPadreNavigation).WithMany(p => p.InverseIdMenuPadreNavigation)
                .HasForeignKey(d => d.IdMenuPadre)
                .HasConstraintName("FK__Menu__idMenuPadr__37A5467C");
        });

        modelBuilder.Entity<PersonalPolicial>(entity =>
        {
            entity.HasKey(e => e.IdPersonal).HasName("PK__Personal__05A9201BDBB3E672");

            entity.ToTable("PersonalPolicial");

            entity.Property(e => e.ApellidoYnombre)
                .HasMaxLength(255)
                .HasColumnName("ApellidoYNombre");
            entity.Property(e => e.Chapa).HasMaxLength(50);
            entity.Property(e => e.DestinoAnterior).HasMaxLength(100);
            entity.Property(e => e.Detalles).HasDefaultValue("");
            entity.Property(e => e.Dni)
                .HasMaxLength(50)
                .HasColumnName("DNI");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Especialidad).HasMaxLength(50);
            entity.Property(e => e.EstadoCivil).HasMaxLength(50);
            entity.Property(e => e.EstudiosCurs).HasMaxLength(255);
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Funcion).HasMaxLength(100);
            entity.Property(e => e.Grado).HasMaxLength(50);
            entity.Property(e => e.Horario).HasMaxLength(50);
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Legajo).HasMaxLength(50);
            entity.Property(e => e.NombreImagen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreImagen");
            entity.Property(e => e.Sexo).HasMaxLength(21);
            entity.Property(e => e.SituacionRevista).HasMaxLength(50);
            entity.Property(e => e.SubsidioSalud).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(55);
            entity.Property(e => e.TelefonoEmergencia).HasMaxLength(55);
            entity.Property(e => e.Trasladado).HasDefaultValue(false);
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlImagen");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PersonalPolicials)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__PersonalP__idUsu__49C3F6B7");
        });

        modelBuilder.Entity<Radio>(entity =>
        {
            entity.HasKey(e => e.IdRadio).HasName("PK__Radio__B273BC9911085492");

            entity.ToTable("Radio");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.EstadoRadio).HasMaxLength(50);
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MarcayModelo).HasMaxLength(50);
            entity.Property(e => e.SerieRadio)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tipo).HasMaxLength(50);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Radios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Radio__IdUsuario__6383C8BA");
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("PK__Reportes__F95611364E28C199");

            entity.Property(e => e.Accion).HasMaxLength(10);
            entity.Property(e => e.FechaAccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdRecurso).HasMaxLength(100);
            entity.Property(e => e.TipoRecurso).HasMaxLength(50);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reportes)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Reportes__IdUsua__03F0984C");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__3C872F7623F81D49");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<RolMenu>(entity =>
        {
            entity.HasKey(e => e.IdRolMenu).HasName("PK__RolMenu__CD2045D8C35C30E3");

            entity.ToTable("RolMenu");

            entity.Property(e => e.IdRolMenu).HasColumnName("idRolMenu");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.IdMenu)
                .HasConstraintName("FK__RolMenu__idMenu__3F466844");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__RolMenu__idRol__3E52440B");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A6868466D7");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__idRol__4316F928");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo).HasName("PK__Vehiculo__70861215AA7EFFBB");

            entity.ToTable("Vehiculo");

            entity.Property(e => e.ChasisNumero).HasMaxLength(50);
            entity.Property(e => e.Dominio).HasMaxLength(50);
            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.EstadoVehiculo).HasMaxLength(50);
            entity.Property(e => e.FechaEliminacion).HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KmActual).HasMaxLength(50);
            entity.Property(e => e.LugarDeReparacion).HasMaxLength(255);
            entity.Property(e => e.MarcayModelo).HasMaxLength(50);
            entity.Property(e => e.MotorNumero).HasMaxLength(50);
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.Tuc)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TUC");
            entity.Property(e => e.UltimoService).HasMaxLength(50);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Vehiculo__IdUsua__68487DD7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
