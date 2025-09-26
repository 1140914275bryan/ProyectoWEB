using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class VentaServicio : IVentaServicio
    {
        private readonly IRepositorioGenerico<Producto> _repositorioProducto;
        private readonly IVentaRepositorio _repositorioVenta;

        public VentaServicio(IRepositorioGenerico<Producto> repositorioProducto, IVentaRepositorio repositorioVenta)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioVenta = repositorioVenta;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(
                p => p.EsActivo == true && p.Stock > 0 &&
                string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda));

            return query.Include(c => c.IdCategoriaNavigation).ToList();

        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch { throw; }
        }

        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if (fechaInicio != "" && fechaFin != "")
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

                return query.Where(v => v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                 v.FechaRegistro.Value.Date <= fecha_fin.Date
                 )
                     .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                     .Include(u => u.IdUsuarioNavigation)
                     .Include(dv => dv.DetalleVenta)
                     .ToList();
            }
            else
            {

                return query.Where(v => v.NumeroVenta == numeroVenta
                 )
              .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
              .Include(u => u.IdUsuarioNavigation)
              .Include(dv => dv.DetalleVenta)
              .ToList();

            }
        }
        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.NumeroVenta == numeroVenta);

            return query
            .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
            .Include(u => u.IdUsuarioNavigation)
            .Include(dv => dv.DetalleVenta)
            .First();

        }

        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {

            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-PE"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-PE"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fecha_inicio, fecha_fin);
            return lista;
        }

        public async Task<List<DetalleVenta>> ObtenerDetalleVentaPorId(int idVenta)
        {
            var venta = await _repositorioVenta.Consultar(
                v => v.IdVenta == idVenta
            );

            return await venta
                .Include(v => v.DetalleVenta)
                .SelectMany(v => v.DetalleVenta)
                .ToListAsync();
        }

    }
}
