using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IPersonalPolicialService
    {
        // Listar todo el personal policial
        Task<List<PersonalPolicial>> Lista();

        // Listar personal trasladado (eliminado lógico)
        Task<List<PersonalPolicial>> ListaTrasladados();

        // Crear personal con foto opcional (y arma/domicilio dentro de la entidad)
        Task<PersonalPolicial> Crear(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "");

        // Editar personal y foto opcional
        Task<PersonalPolicial> Editar(PersonalPolicial entidad, Stream Foto = null, string NombreFoto = "");

        // Eliminar (o marcar como eliminado) personal policial
        Task<bool> Trasladar(int idPersonal, int idUsuario);

        // Obtener personal por ID con arma y domicilio
        Task<PersonalPolicial> ObtenerPorId(int idPersonal);

        // Marcar personal como no trasladado (restituir)
        Task<bool> Restituir(int idPersonal, int idUsuario);

        //Paginacion en el listado de Perssonal Policial Activos
        Task<DataTableResponse<PersonalPolicial>> ListarPaginado(DataTableRequest request);
        
        //Paginacion en el listado de Personal Policial Trasladados
        Task<DataTableResponse<PersonalPolicial>> ListarPaginadoTrasladados(DataTableRequest request);

    }
}
