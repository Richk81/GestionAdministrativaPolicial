using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.DAL.Interfaces;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class ChalecoService : IChalecoService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Chaleco> _repositorio;

        // Constructor de la clase ChalecoService
        public ChalecoService(
            IGenericRepository<Chaleco> repositorio
            )
        {
            _repositorio = repositorio;
        }


        public Task<List<Chaleco>> Lista()
        {
            throw new NotImplementedException();
        }

        public Task<Chaleco> Crear(Chaleco entidad)
        {
            throw new NotImplementedException();
        }

        public Task<Chaleco> Editar(Chaleco entidad)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int idChaleco)
        {
            throw new NotImplementedException();
        }

        public Task<Chaleco> ObtenerPorId(int idChaleco)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Asignar(int idChaleco, int? idPersonal)
        {
            throw new NotImplementedException();
        }


    }
}
