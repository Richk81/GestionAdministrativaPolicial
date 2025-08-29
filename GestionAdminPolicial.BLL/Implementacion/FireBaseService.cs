using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GestionAdminPolicial.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using GestionAdminPolicial.Entity;
using GestionAdminPolicial.DAL.Interfaces;

namespace GestionAdminPolicial.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => "FireBase_Storage".Equals(c.Recurso));


                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);


                // 🔧 IMPORTANTE:
                // Este código usa FirebaseAuthentication.net v3.1.0
                // porque versiones más recientes (como la 4.x) eliminan o cambian
                // las clases FirebaseAuthProvider y FirebaseConfig.
                // No actualizar sin verificar compatibilidad.
                var Auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await Auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;
            }
            catch {
                UrlImagen = "";


            }
            return UrlImagen;
        }

        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => "FireBase_Storage".Equals(c.Recurso));


                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);


                // 🔧 IMPORTANTE:
                // Este código usa FirebaseAuthentication.net v3.1.0
                // porque versiones más recientes (como la 4.x) eliminan o cambian
                // las clases FirebaseAuthProvider y FirebaseConfig.
                // No actualizar sin verificar compatibilidad.
                var Auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await Auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync();

                await task;

                return true;
            }
            catch
            {
                return false;


            }
        }
    }
}