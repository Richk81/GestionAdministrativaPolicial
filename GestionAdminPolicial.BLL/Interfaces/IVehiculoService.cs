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
    public interface IVehiculoService
    {
        //Paginacion en listado Activos
        Task<DataTableResponse<Vehiculo>> ListarPaginado(DataTableRequest request);

        // Paginación en listado Eliminados
        Task<DataTableResponse<Vehiculo>> ListarPaginadoEliminados(DataTableRequest request);

        // Crear un nuevo
        Task<Vehiculo> Crear(Vehiculo entidad);

        // Editar un existente
        Task<Vehiculo> Editar(Vehiculo entidad);

        // Eliminar (o marcar como eliminado)
        Task<bool> Eliminar(int idVehiculo, int idUsuario);

        // Eliminar (o marcar como eliminado)
        Task<bool> Restablecer(int idVehiculo, int idUsuario);

        // Obtener por su ID
        Task<Vehiculo> ObtenerPorId(int idVehiculo);

    }
}
