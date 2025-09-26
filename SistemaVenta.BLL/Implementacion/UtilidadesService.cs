using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;

namespace SistemaVenta.BLL.Implementacion
{
    // clase para encriptar la clave del usuario y generar una clave aleatoria
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);
            return clave;
        }
        public string ConvertirSha256(string texto) // Método para convertir un texto a su representación SHA256
        {
            StringBuilder sb = new StringBuilder(); // Crear un StringBuilder para almacenar el hash
            using (SHA256 hash = SHA256Managed.Create()) // crear instancia de SHA256
            {
                Encoding enc = Encoding.UTF8; // Convertir el texto a bytes

                byte[] result = hash.ComputeHash(enc.GetBytes(texto)); // Calcular el hash SHA256 del texto

                foreach (byte b in result) // Recorrer cada byte del resultado
                {
                    sb.Append(b.ToString("x2")); // Convertir el byte a su representación hexadecimal y agregarlo al StringBuilder
                }
            }
            return sb.ToString(); // Devolver el hash como una cadena hexadecimal
        }
    }
}
