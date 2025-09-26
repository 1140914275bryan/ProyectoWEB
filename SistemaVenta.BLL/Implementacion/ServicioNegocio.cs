using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.BLL.Implementacion
{
    public class ServicioNegocio : IServicioNegocio
    {
        private readonly IRepositorioGenerico<Negocio> _repositorio;
        private readonly IFireBaseService _fireBaseService;

        public ServicioNegocio(IRepositorioGenerico<Negocio> repositorio, IFireBaseService fireBaseService)
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocio_encontrado = await _repositorio.Obtener(n => n.IdNegocio == 1); // Solo se trabaja con un negocio con id = 1
                return negocio_encontrado;
            }
            catch { throw; }
        }

        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocio_encontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);

                // Actualizar los datos principales
                negocio_encontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocio_encontrado.Nombre = entidad.Nombre;
                negocio_encontrado.Correo = entidad.Correo;
                negocio_encontrado.Direccion = entidad.Direccion;
                negocio_encontrado.Telefono = entidad.Telefono;
                negocio_encontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocio_encontrado.SimboloMoneda = entidad.SimboloMoneda;

                // Mantener el nombre del logo si ya existe
                negocio_encontrado.NombreLogo = negocio_encontrado.NombreLogo == ""? NombreLogo : negocio_encontrado.NombreLogo;

                // Si se sube una nueva imagen, actualizar la URL
                if (Logo != null)
                {
                    string urlLogo = await _fireBaseService.SubirStorage(Logo,"carpeta_logo",negocio_encontrado.NombreLogo);
                    Console.WriteLine("URL generada desde Firebase: " + urlLogo);

                    negocio_encontrado.UrlLogo = urlLogo;
                }
                await _repositorio.Editar(negocio_encontrado);
                return negocio_encontrado;
            }
            catch { throw; }
        }
    }
}