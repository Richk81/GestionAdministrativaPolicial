using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net;
using System.IO;


using Microsoft.EntityFrameworkCore;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.DAL.Interfaces;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class PersonalPolicialService : IPersonalPolicialService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<PersonalPolicial> _repositorio;
        private readonly IGenericRepository<Domicilio> _repositorioDomicilio;
        private readonly IGenericRepository<Arma> _repositorioArma;
        private readonly IFireBaseService _fireBaseService;

        // Constructor de la clase PersonalPolicialService
        public PersonalPolicialService(
            IGenericRepository<PersonalPolicial> repositorio,
            IGenericRepository<Domicilio> repositorioDomicilio,
            IGenericRepository<Arma> repositorioArma,
            IFireBaseService fireBaseService
            )
        {
            _repositorio = repositorio;
            _repositorioDomicilio = repositorioDomicilio;
            _repositorioArma = repositorioArma;
            _fireBaseService = fireBaseService;

        }

        //Lógica del método listar PersonalPolicial Activo
        public async Task<List<PersonalPolicial>> Lista()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == false) // activos
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        //Lógica del método listar PersonalPolicial Trasladados (eliminados lógicamente)
        public async Task<List<PersonalPolicial>> ListaTrasladados()
        {
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            query = query
                .Where(p => p.Trasladado == true) // trasladados
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation);

            return await query.ToListAsync();
        }

        // ✅ METODO para Crear un NUEVO PERSONAL POLICIAL (registrar nuevo)
        public async Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            try
            {
                // Valores por defecto
                entidad.Trasladado = false;
                entidad.FechaEliminacion = null;
                entidad.NombreImagen = NombreFoto;

                // ✅ Subida de imagen a Firebase
                if (Foto != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(Foto, "carpeta_personal", NombreFoto);
                    entidad.UrlImagen = urlImagen;
                }

                // ✅ Crear Personal
                PersonalPolicial personalCreado = await _repositorio.Crear(entidad);

                if (personalCreado.IdPersonal == 0)
                    throw new TaskCanceledException("No se pudo crear el personal policial.");

                // ✅ Guardar domicilio si se cargó en el formulario
                if (entidad.Domicilios != null && entidad.Domicilios.Any())
                {
                    foreach (var domicilio in entidad.Domicilios)
                    {
                        domicilio.IdPersonal = personalCreado.IdPersonal; // relación
                        await _repositorioDomicilio.Crear(domicilio);
                    }
                }

                // ✅ Guardar arma si se cargó en el formulario
                if (entidad.Armas != null && entidad.Armas.Any())
                {
                    foreach (var arma in entidad.Armas)
                    {
                        // si no se cargaron datos, guardar "No Provista"
                        arma.NumeroSerie = string.IsNullOrEmpty(arma.NumeroSerie) ? "No Provista" : arma.NumeroSerie;
                        arma.Marca = string.IsNullOrEmpty(arma.Marca) ? "No Provista" : arma.Marca;

                        arma.IdPersonal = personalCreado.IdPersonal; // relación
                        await _repositorioArma.Crear(arma);
                    }
                }

                // ✅ Cargar relaciones antes de devolver
                IQueryable<PersonalPolicial> query = await _repositorio.Consultar(p => p.IdPersonal == personalCreado.IdPersonal);
                personalCreado = query
                    .Include(p => p.Domicilios)
                    .Include(p => p.Armas)
                    .Include(p => p.IdUsuarioNavigation)
                    .First();

                return personalCreado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el personal policial: " + ex.Message);
            }
        }


        public Task<PersonalPolicial> Editar(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int idPersonal)
        {
            throw new NotImplementedException();
        }



        public Task<PersonalPolicial> ObtenerPorId(int idPersonal)
        {
            throw new NotImplementedException();
        }
    }
}
