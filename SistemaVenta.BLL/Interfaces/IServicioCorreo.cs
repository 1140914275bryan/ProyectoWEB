using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IServicioCorreo
    {
        Task<bool> Enviarcorreo(string CorreoDestino, string Asunto, string Mensaje); // Método para enviar correo electrónico
    }
}
