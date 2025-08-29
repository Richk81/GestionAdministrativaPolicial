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
        private readonly IFireBaseService _fireBaseService;

        // Constructor de la clase PersonalPolicialService
        public PersonalPolicialService(
            IGenericRepository<PersonalPolicial> repositorio,
            IFireBaseService fireBaseService
            )
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;

        }

        //Lógica del método listar PersonalPolicial
        public async Task<List<PersonalPolicial>> Lista()
        {
            // Consulta base
            IQueryable<PersonalPolicial> query = await _repositorio.Consultar();

            // Incluimos solo las relaciones necesarias
            query = query
                .Include(p => p.Armas)
                .Include(p => p.Domicilios)
                .Include(p => p.IdUsuarioNavigation); // Datos del usuario relacionado si se necesita

            // Retornamos la lista
            return await query.ToListAsync();
        }

        public Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "")
        {
            throw new NotImplementedException();
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
