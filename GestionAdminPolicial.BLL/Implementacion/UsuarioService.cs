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
    public class UsuarioService : IUsuarioService
    {
        //(SERVICIOS QUE VOY A UTILIZAR)   
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        // Constructor de la clase UsuarioService
        public UsuarioService(
            IGenericRepository<Usuario> repositorio,
            IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        // Implementación de la lógica de los métodos de la interfaz IUsuarioService:

        //Lógica del método listar usuarios
        public async Task<List<Usuario>> Lista() 
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        //Lógica del método Crear usuarios
        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya está registrado.");

            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string url_foto = await _fireBaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = url_foto;
                }

                Usuario usuario_creado = await _repositorio.Crear(entidad);

                if (usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario.");

                // ✅ Enviar correo de bienvenida con HttpClient
                if (!string.IsNullOrEmpty(UrlPlantillaCorreo))
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo
                        .Replace("[correo]", usuario_creado.Correo)
                        .Replace("[clave]", clave_generada);

                    string htmlCorreo = "";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(UrlPlantillaCorreo);

                        if (response.IsSuccessStatusCode)
                        {
                            htmlCorreo = await response.Content.ReadAsStringAsync();
                        }
                    }

                    if (!string.IsNullOrEmpty(htmlCorreo))
                    {
                        await _correoService.EnviarCorreo(
                            usuario_creado.Correo,
                            "Cuenta Creada",
                            htmlCorreo
                        );
                    }
                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(r => r.IdRolNavigation).First();

                return usuario_creado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario: " + ex.Message);
            }
        }

        //Lógica del método Editar usuarios
        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya Existe");


            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                
                Usuario usuario_editar = queryUsuario.First();

                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo = entidad.EsActivo;

                if (usuario_editar.NombreFoto =="")
                    usuario_editar.NombreFoto = NombreFoto;

                if(Foto != null)
                {
                    string urlfoto = await _fireBaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlfoto;
                }

                bool respuesta = await _repositorio.Editar(usuario_editar);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar el usuario.");

                Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();
                
                return usuario_editado;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el usuario: " + ex.Message);
            }


        }

        // Implementación del método eliminar USUARIO
        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);

                if(respuesta)
                    await _fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario: " + ex.Message);

            }
        }

        // Implementación del método Obtener x Credenciales de la interfaz IUsuarioService:

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);

            Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) 
            && u.Clave.Equals(clave_encriptada));

            return usuario_encontrado;
        }

        public async Task<Usuario> ObtenerPorId(int IdUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == IdUsuario);

            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();

            return resultado;
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el perfil: " + ex.Message);
            }
        }

        public async Task<bool> CambiarClave(int IdUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                if (usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La clave ingresada como actual es incorrecta.");

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);

                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar la clave: " + ex.Message);
            }
        }

        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo == correo);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No se encontró ningún Usuario asociado al correo");

                string clave_generada = _utilidadesService.GenerarClave();

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);

                // Inserta la clave en la URL
                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", clave_generada);

                string htmlCorreo = "";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(UrlPlantillaCorreo);

                    if (response.IsSuccessStatusCode)
                    {
                        htmlCorreo = await response.Content.ReadAsStringAsync();
                    }
                }

                bool correo_enviado = false;

                if (htmlCorreo != "")
                    correo_enviado = await _correoService.EnviarCorreo(correo,"Contraseña Restablecida",htmlCorreo);

                if (!correo_enviado)
                    throw new TaskCanceledException("Tenemos problemas. Por favor intentalo de nuevo mas tarde.");

                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch (Exception ex)
            {
                throw new Exception("Error al restablecer la clave: " + ex.Message);
            }
        }
    }
}
