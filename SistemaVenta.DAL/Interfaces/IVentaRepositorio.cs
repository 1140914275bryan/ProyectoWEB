using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;


namespace SistemaVenta.DAL.Interfaces
{
    public interface IVentaRepositorio : IRepositorioGenerico<Venta>
    {
        Task<Venta> Registrar(Venta entidad);
        Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin);
    }

}
