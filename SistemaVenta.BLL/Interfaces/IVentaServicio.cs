using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IVentaServicio
    {

        Task<List<Producto>> ObtenerProductos(string busqueda);
        Task<Venta> Registrar(Venta entidad);
        Task<List<Venta>>Historial(String numeroVenta, string fechaInicio, string fechaFin);
        Task<Venta> Detalle(string numeroVenta);
        Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin);
        Task<List<DetalleVenta>> ObtenerDetalleVentaPorId(int idVenta);

    }
}

