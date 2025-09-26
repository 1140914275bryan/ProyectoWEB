using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = ""); // Crea un nuevo usuario, opcionalmente con una foto y plantilla de correo
        Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = ""); // Edita un usuario, opcionalmente con una foto nueva
        Task<bool> Eliminar(int idUsuario); // Elimina un usuario por su ID
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave); // Obtiene un usuario por sus credenciales (correo y clave)
        Task<Usuario> ObtenerPorId(int idUsuario); // Obtiene un usuario por su ID
        Task<bool> GuardarPerfil(Usuario entidad); // Guarda el perfil del usuario actual
        Task<bool> CambiarClave(int idUsuario, string claveActual, string nuevaClave); // Cambia la clave del usuario
        Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo); // Restablece la clave del usuario enviando un correo con una nueva clave temporal
    }
}
