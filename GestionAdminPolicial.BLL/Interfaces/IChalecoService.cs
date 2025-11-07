using GestionAdminPolicial.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace GestionAdminPolicial.BLL.Interfaces
{
    public interface IChalecoService
    {
        // Listar todos los chalecos Activos
        Task<List<Chaleco>> Lista();

        // Listar todos los chalecos Eliminados (lógicos)
        Task<List<Chaleco>> ListaEliminados();

        // Crear un nuevo chaleco
        Task<Chaleco> Crear(Chaleco entidad);

        // Editar un chaleco existente
        Task<Chaleco> Editar(Chaleco entidad);

        // Eliminar (o marcar como eliminado) un chaleco
        Task<bool> Eliminar(int idChaleco, int idUsuario);

        // Eliminar (o marcar como eliminado) un chaleco
        Task<bool> Restablecer(int idChaleco, int idUsuario);

        // Obtener un chaleco por su ID
        Task<Chaleco> ObtenerPorId(int idChaleco);

        // Asignar un chaleco a un personal policial si IdPersonal es null, y desasignar si IdPersonal tiene un valor
        Task<bool> Asignar(int idChaleco, int? idPersonal);

        Task<bool> Desasignar(int idChaleco);

        // Buscar chalecos por número de serie (desde la base de datos)
        Task<List<Chaleco>> BuscarPorNumeroSerie(string serieChaleco);

    }
}
