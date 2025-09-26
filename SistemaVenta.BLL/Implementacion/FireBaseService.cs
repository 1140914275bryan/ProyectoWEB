using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using Azure.Core;


namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IRepositorioGenerico<Configuracion> _repositorio;

        public FireBaseService(IRepositorioGenerico<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var autorizacion = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"])); // Autenticación con Supabase usando la clave de API
                var a = await autorizacion.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]); // Inicia sesión con email y contraseña

                var cancelacion = new CancellationTokenSource(); // Crea un token de cancelación para la operación

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken), // Obtiene el token de autenticación de Firebase
                        ThrowOnCancel = true // Lanza una excepción si la operación es cancelada
                    })
                    .Child(Config[CarpetaDestino]) // Especifica la carpeta de destino en Firebase Storage
                    .Child(NombreArchivo) // Especifica el nombre del archivo en Firebase Storage
                    .PutAsync(StreamArchivo, cancelacion.Token); // Sube el archivo al almacenamiento de Firebase

                UrlImagen = await task; // Espera a que la tarea de subida se complete y obtiene la URL del archivo subido
                Console.WriteLine("URL generada desde Firebase: " + UrlImagen);

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
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var autorizacion = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"])); // Autenticación con Supabase usando la clave de API
                var a = await autorizacion.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]); // Inicia sesión con email y contraseña

                var cancelacion = new CancellationTokenSource(); // Crea un token de cancelación para la operación

                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken), // Obtiene el token de autenticación de Firebase
                        ThrowOnCancel = true // Lanza una excepción si la operación es cancelada
                    })
                    .Child(Config[CarpetaDestino]) // Especifica la carpeta de destino en Firebase Storage
                    .Child(NombreArchivo) // Especifica el nombre del archivo en Firebase Storage
                    .DeleteAsync(); // Elimina el archivo del almacenamiento de Firebase
                await task; // Espera a que la tarea de eliminación se complete

                return true; // Retorna verdadero si la eliminación fue exitosa
            }
            catch 
            { 
                return false; // Retorna falso si ocurrió un error durante la eliminación
            }

        }

    }
}
