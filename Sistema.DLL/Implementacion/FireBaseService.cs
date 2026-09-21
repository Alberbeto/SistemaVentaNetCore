using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.BLL;
using System;
using System.Collections.Generic;
using System.Text;

using Firebase.Auth;
using Firebase.Storage;

namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repository;

        public FireBaseService(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<string> SubirStorage(Stream StreamFoto, string CarpetaNombre, string FotoNombre)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> entidad = await _repository.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = entidad.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);


                var cancelacion = new CancellationTokenSource();

                var task = new FirebaseStorage(
                        config["ruta"],
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel= true
                        }).Child(CarpetaNombre)
                        .Child(FotoNombre)
                        .PutAsync(StreamFoto, cancelacion.Token);

                UrlImagen = await task;

               


            }catch (Exception ex)
            {
                UrlImagen = "";
            }
            return UrlImagen;

        }
        public async Task<bool> EliminarStorage(string CarpetaNombre, string NombreFoto)
        {
            try
            {
                IQueryable<Configuracion> entidad = await _repository.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = entidad.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);


                var cancelacion = new CancellationTokenSource();

                var task = new FirebaseStorage(
                        config["ruta"],
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        }).Child(CarpetaNombre)
                        .Child(NombreFoto)
                        .DeleteAsync();

                return true;




            }
            catch (Exception ex)
            {
                return false;
            }
        }

      
    }
}
