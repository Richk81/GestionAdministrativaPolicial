using GestionAdminPolicial.BLL.Interfaces;
using GestionAdminPolicial.DAL.DBContext;
using GestionAdminPolicial.DAL.Interfaces;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IEscopetaService
    {
        //Paginacion en listado Activos
        Task<DataTableResponse<Escopeta>> ListarPaginado(DataTableRequest request);

        // Paginación en listado Eliminados
        Task<DataTableResponse<Escopeta>> ListarPaginadoEliminados(DataTableRequest request);

        // Crear un nuevo
        Task<Escopeta> Crear(Escopeta entidad);

        // Editar un existente
        Task<Escopeta> Editar(Escopeta entidad);

        // Eliminar (o marcar como eliminado) 
        Task<bool> Eliminar(int idEscopeta, int idUsuario);

        // Eliminar (o marcar como eliminado)
        Task<bool> Restablecer(int idEscopeta, int idUsuario);

        // Obtener por su ID
        Task<Escopeta> ObtenerPorId(int idRadio);

        // Para Dashboard - Cantidad de Escopetas activo
        Task<IQueryable<Escopeta>> Consultar();
    }
}
