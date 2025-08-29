using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IPersonalPolicialService
    {
        // Listar todo el personal policial
        Task<List<PersonalPolicial>> Lista();

        // Crear personal con foto opcional (y arma/domicilio dentro de la entidad)
        Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "");

        // Editar personal y foto opcional
        Task<PersonalPolicial> Editar(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "");

        // Eliminar (o marcar como eliminado) personal policial
        Task<bool> Eliminar(int idPersonal);

        // Obtener personal por ID con arma y domicilio
        Task<PersonalPolicial> ObtenerPorId(int idPersonal);
    }
}
