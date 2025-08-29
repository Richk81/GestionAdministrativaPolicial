using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.Interfaces;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class DependenciaPolService : IDependenciaPolService
    {
        private readonly IGenericRepository<Dependencia> _repositorio;
        private readonly IFireBaseService _firebaseService;

        public DependenciaPolService(IGenericRepository<Dependencia> repositorio, IFireBaseService firebaseService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
        }

        //NOTA: Este método está diseñado para obtener una dependencia específica con IdDependencia == 1.
        public async Task<Dependencia> Obtener()
        {
            try
            {            //NOTA: IdDependencia == 1 porque es la única dependencia que se maneja en el sistema.(solo se podra editar la misma)
                Dependencia dependencia_encontrada = await _repositorio.Obtener(n => n.IdDependencia == 1);
                return dependencia_encontrada;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dependencia> GuardarCambios(Dependencia entidad, Stream Logo = null, string NombreLogo = "")
        {
            try
            {
                Dependencia dependencia_encontrada = await _repositorio.Obtener(n => n.IdDependencia == 1);

                //Guardar los cambios en la dependencia
                dependencia_encontrada.Nombre = entidad.Nombre;
                dependencia_encontrada.Correo = entidad.Correo;
                dependencia_encontrada.Direccion = entidad.Direccion;
                dependencia_encontrada.Telefono = entidad.Telefono;

                dependencia_encontrada.NombreLogo = dependencia_encontrada.NombreLogo == "" ? NombreLogo : dependencia_encontrada.NombreLogo;

                if (Logo != null)
                {
                    //Subir el logo a Firebase Storage
                    string urlLogo = await _firebaseService.SubirStorage(Logo, "carpeta_logo", dependencia_encontrada.NombreLogo);
                    dependencia_encontrada.UrlLogo = urlLogo;
                }

                //Guardar los cambios en la base de datos
                await _repositorio.Editar(dependencia_encontrada);
                return dependencia_encontrada;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los cambios en la dependencia.", ex);
            }

        }

    }
}
