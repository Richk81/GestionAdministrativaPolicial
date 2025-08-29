using GestionAdminPolicial.AplicacionWeb.Models.ViewModels;
using GestionAdminPolicial.Entity;
using AutoMapper;
using System.Globalization;


namespace GestionAdminPolicial.AplicacionWeb.Utilidades.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Nos permite ordenar el mapeo de las propiedades de las entidades a las vistas
            #region Rol 
            //Nos permite mapear las propiedades de la entidad Rol a la vista VMRol y viceversa
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion Rol

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(destino => destino.NombreRol,
                    opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion)
                );

            CreateMap<VMUsuario, Usuario>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino => destino.IdRolNavigation,
                    opt => opt.Ignore() // Ignoramos la navegación para evitar problemas de referencia circular
                );
            #endregion Usuario

            #region DependenciaPol
            CreateMap<Dependencia, VMDependenciaPol>().ReverseMap();
            #endregion DependenciaPol

            #region Arma
            CreateMap<Arma, VMArma>()
                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

            CreateMap<VMArma, Arma>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
            #endregion Arma

            #region Domicilio
            CreateMap<Domicilio, VMDomicilio>()
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

            CreateMap<VMDomicilio, Domicilio>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
            #endregion Domicilio

            #region PersonalPolicial
            CreateMap<PersonalPolicial, VMPersonalPolicial>()
                .ForMember(dest => dest.Armas,
                    opt => opt.MapFrom(src => src.Armas)) // AutoMapper usará el map Arma -> VMArma

                .ForMember(dest => dest.Domicilios,
                    opt => opt.MapFrom(src => src.Domicilios)) // AutoMapper usará el map Domicilio -> VMDomicilio

                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

                CreateMap<VMPersonalPolicial, PersonalPolicial>()
                    .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())
                    .ForMember(dest => dest.Armas, opt => opt.Ignore())
                    .ForMember(dest => dest.Domicilios, opt => opt.Ignore());
            #endregion

            #region Chaleco
            // De entidad a ViewModel (mostrar)
            CreateMap<Chaleco, VMChaleco>()
                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null))
                .ForMember(dest => dest.IdPersonalNavigation,
                    opt => opt.MapFrom(src => src.IdPersonalNavigation));
            // De ViewModel a entidad (editar/insertar)
            CreateMap<VMChaleco, Chaleco>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore())  // Ignorar navegación para evitar problemas
                .ForMember(dest => dest.IdPersonalNavigation, opt => opt.Ignore());
            #endregion Chaleco

            #region Escopeta
            // De entidad a ViewModel (mostrar)
            CreateMap<Escopeta, VMEscopeta>()
                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

            // De ViewModel a entidad (editar/insertar)
            CreateMap<VMEscopeta, Escopeta>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
            #endregion Escopeta

            #region Radio
            // De entidad a ViewModel (mostrar)
            CreateMap<Radio, VMRadio>()
                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

            // De ViewModel a entidad (editar/insertar)
            CreateMap<VMRadio, Radio>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
            #endregion Radio

            #region Vehiculo
            // De entidad a ViewModel (mostrar)
            CreateMap<Vehiculo, VMVehiculo>()
                .ForMember(dest => dest.NombreUsuario,
                    opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null));

            // De ViewModel a entidad (editar/insertar)
            CreateMap<VMVehiculo, Vehiculo>()
                .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
            #endregion Vehiculo

            #region Reporte
            CreateMap<Reporte, VMReporte>()
                    .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : null))
                    .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation));
            #endregion Reporte

            #region Menu
            CreateMap<Menu, VMMenu>()
            .ForMember(dest => dest.SubMenus, opt => opt.MapFrom(src => src.InverseIdMenuPadreNavigation));
            #endregion Menu

        }
    }
}