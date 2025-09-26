using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class ServicioCorreo : IServicioCorreo
    {
        private readonly IRepositorioGenerico<Configuracion> _repositorio;

        public ServicioCorreo(IRepositorioGenerico<Configuracion> repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public async Task<bool> Enviarcorreo(string CorreoDestino, string Asunto, string Mensaje)
        {
            try
            {
                var query = await _repositorio.Consultar(c =>
                    c.Recurso == "Servicio_Correo"
                );

                if (query == null || !query.Any())
                {
                    Console.WriteLine("No se encontró configuración SMTP.");
                    return false;
                }

                // Diccionario insensible a mayúsculas/minúsculas
                var configuracion = new Dictionary<string, string>(
                    query.ToDictionary(c => c.Propiedad!, c => c.Valor!),
                    StringComparer.OrdinalIgnoreCase
                );  // :contentReference[oaicite:1]{index=1}

                // Lectura de configuración
                var correoOrigen = configuracion["Correo"];
                var clave = configuracion["Clave"];
                var host = configuracion["Host"];
                var puerto = int.Parse(configuracion["Puerto"]);
                var usarSsl = bool.Parse(configuracion.GetValueOrDefault("EnableSsl", "true"));

                using var smtp = new SmtpClient
                {
                    Host = host,
                    Port = puerto,
                    EnableSsl = usarSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false // debe ir antes de asignar Credentials :contentReference[oaicite:2]{index=2}
                };

                smtp.Credentials = new NetworkCredential(correoOrigen, clave);

                var mail = new MailMessage
                {
                    From = new MailAddress(correoOrigen),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(CorreoDestino));

                await smtp.SendMailAsync(mail); // envío moderno y asincrónico :contentReference[oaicite:3]{index=3}
                Console.WriteLine($"Correo enviado correctamente a {CorreoDestino}");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Error SMTP ({smtpEx.StatusCode}): {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                    Console.WriteLine($"Detalle interno: {smtpEx.InnerException.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error genérico al enviar correo: {ex}");
                return false;
            }
        }
    }
}
