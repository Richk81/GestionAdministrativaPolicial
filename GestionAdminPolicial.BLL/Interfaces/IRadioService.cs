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
    public interface IRadioService
    {
        //Paginacion en listado Activos
        Task<DataTableResponse<Radio>> ListarPaginado(DataTableRequest request);

        // Paginación en listado Eliminados
        Task<DataTableResponse<Radio>> ListarPaginadoEliminados(DataTableRequest request);

        // Crear un nuevo
        Task<Radio> Crear(Radio entidad);

        // Editar un existente
        Task<Radio> Editar(Radio entidad);

        // Eliminar (o marcar como eliminado) 
        Task<bool> Eliminar(int idRadio, int idUsuario);

        // Eliminar (o marcar como eliminado)
        Task<bool> Restablecer(int idRadio, int idUsuario);

        // Obtener por su ID
        Task<Radio> ObtenerPorId(int idRadio);
    }
}
