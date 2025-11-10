using GestionAdminPolicial.Entity;
using GestionAdminPolicial.Entity.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionAdminPolicial.BLL.Interfaces
{
    //Nota: Task es una clase que representa una operación asíncrona.
    public interface IUsuarioService
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream Foto= null, string NombreFoto ="", string UrlPlantillaCorreo ="");
        Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int IdUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenerPorId(int IdUsuario);
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva);
        Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo);
        //Paginacion en el listado de USUARIOS
        Task<DataTableResponse<Usuario>> ListarPaginado(DataTableRequest request);
    }
}
