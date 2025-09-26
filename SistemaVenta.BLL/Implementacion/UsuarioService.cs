using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;
using System.Linq.Expressions;
using Microsoft.VisualBasic;
using Azure.Core;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IRepositorioGenerico<Usuario> _repositorio;
        private readonly IFireBaseService _supaBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly IServicioCorreo _correoService;

        public UsuarioService(
           IRepositorioGenerico<Usuario> repositorio,
           IFireBaseService supaBaseService,
           IUtilidadesService utilidadesService,
           IServicioCorreo correoService
            )
        {
            _repositorio = repositorio;
            _supaBaseService = supaBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;

        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(); // Obtiene todos los usuarios de la base de datos
            return query.Include(rol => rol.IdRolNavigation).ToList(); // Incluye la navegación a la entidad Rol y convierte el resultado a una lista
        }


        // Replace the obsolete WebRequest code with HttpClient
        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                string clave_genetada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_genetada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _supaBaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuario_creado = await _repositorio.Crear(entidad);
                if (usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo).Replace("[clave]", clave_genetada);

                    string htmlCorreo = "";
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(UrlPlantillaCorreo);
                        if (response.IsSuccessStatusCode)
                        {
                            htmlCorreo = await response.Content.ReadAsStringAsync();
                        }
                    }

                    if (htmlCorreo != "")
                        await _correoService.Enviarcorreo(usuario_creado.Correo, "Bienvenido al sistema", htmlCorreo);
                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(rol => rol.IdRolNavigation).First();

                return usuario_creado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)

                throw new TaskCanceledException("El correo ya existe");

            try
            {

                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario); // Consulta el usuario por su ID
                Usuario usuario_editar = queryUsuario.First(); // Obtiene el primer resultado de la consulta
                usuario_editar.Nombre = entidad.Nombre; // Actualiza el nombre del usuario
                usuario_editar.Correo = entidad.Correo; // Actualiza el correo del usuario
                usuario_editar.Telefono = entidad.Telefono; // Actualiza el teléfono del usuario
                usuario_editar.IdRol = entidad.IdRol; // Actualiza el ID del rol del usuario
                usuario_editar.EsActivo = entidad.EsActivo; // Actualiza el estado activo del usuario

                if (usuario_editar.NombreFoto == "") // Verifica si el nombre de la foto está vacío
                    usuario_editar.NombreFoto = NombreFoto; // Asigna el nombre de la foto si no está definido

                if(Foto != null)
                {
                    string urlFoto = await _supaBaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto); // Sube la foto al almacenamiento y obtiene la URL
                    usuario_editar.UrlFoto = urlFoto; // Asigna la URL de la foto al usuario
                }

                bool respuesta = await _repositorio.Editar(usuario_editar); // Edita el usuario en la base de datos

                if(!respuesta) // Verifica si la edición no fue exitosa
                    throw new TaskCanceledException("No se pudo editar el usuario"); // Lanza una excepción si no se pudo editar el usuario

                Usuario usuario_editado = queryUsuario.Include(rol => rol.IdRolNavigation).First(); // Vuelve a consultar el usuario editado para incluir la información del rol al sistema
                return usuario_editado; // Devuelve el usuario editado con la información de su rol y foto

            }
            catch{
                throw;
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {

                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario); // Busca el usuario por su ID
                if (usuario_encontrado == null) // Verifica si el usuario no fue encontrado
                    throw new TaskCanceledException("Usuario no encontrado"); // Lanza una excepción si el usuario no existe

                string nombreFoto = usuario_encontrado.NombreFoto; // Obtiene el nombre de la foto del usuario
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado); // Elimina el usuario de la base de datos

                if(respuesta)
                    await _supaBaseService.EliminarStorage("carpeta_usuario", nombreFoto); // Si la eliminación fue exitosa, elimina la foto del almacenamiento
                return true; // Devuelve true si la eliminación fue exitosa
            }
            catch{
                throw; // Lanza la excepción si ocurre un error
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave); // Convierte la clave a un hash SHA256
            Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave_encriptada)); // Busca el usuario por su correo y clave encriptada
             return usuario_encontrado; // Devuelve el usuario encontrado o null si no existe
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario); // Consulta el usuario por su ID

            Usuario resultado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault(); // Incluye la navegación a la entidad Rol y obtiene el primer resultado o null si no existe
            return resultado; // Devuelve el usuario encontrado o null si no existe
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario); // Busca el usuario por su ID

                if(usuario_encontrado == null) // Verifica si el usuario no fue encontrado
                    throw new TaskCanceledException("Usuario no encontrado"); // Lanza una excepción si el usuario no existe

                usuario_encontrado.Correo = entidad.Correo; // Actualiza el correo del usuario
                usuario_encontrado.Telefono = entidad.Telefono; // Actualiza el teléfono del usuario
                
                bool respuesta = await _repositorio.Editar(usuario_encontrado); // Edita el usuario en la base de datos

                return respuesta;
            }
            catch{
                throw;
            }
        }

        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario); // Busca el usuario por su ID
                if (usuario_encontrado == null) // Verifica si el usuario no fue encontrado
                    throw new TaskCanceledException("El usuario no existe"); // Lanza una excepción si el usuario no existe
                if (usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(nuevaClave)) // Verifica si la clave actual no coincide con la del usuario
                    throw new TaskCanceledException("La clave ingresada como actual no es correcta"); // Lanza una excepción si la clave actual no es correcta

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(nuevaClave); // Actualiza la clave del usuario con la nueva clave encriptada

                bool respuesta = await _repositorio.Editar(usuario_encontrado); // Edita el usuario en la base de datos
                return respuesta;
            }
            catch(Exception ex){
                throw;
            }
        }
        public async Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo == Correo);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No se encontro usuario asociado al correo");

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);

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
                    correo_enviado = await _correoService.Enviarcorreo(Correo, "Contraseña Restablecida", htmlCorreo);

                if (!correo_enviado)
                    throw new TaskCanceledException("Hay problemas. Intentalo mas tarde");

                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
