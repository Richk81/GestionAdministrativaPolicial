using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GestionAdminPolicial.Entity;


namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IChalecoService
    {
        // Listar todos los chalecos
        Task<List<Chaleco>> Lista();

        // Crear un nuevo chaleco
        Task<Chaleco> Crear(Chaleco entidad);

        // Editar un chaleco existente
        Task<Chaleco> Editar(Chaleco entidad);

        // Eliminar (o marcar como eliminado) un chaleco
        Task<bool> Eliminar(int idChaleco);

        // Obtener un chaleco por su ID
        Task<Chaleco> ObtenerPorId(int idChaleco);

        // ✅ Asignar un chaleco a un personal policial (o quitar la asignación si IdPersonal es null)
        Task<bool> Asignar(int idChaleco, int? idPersonal);
    }
}
