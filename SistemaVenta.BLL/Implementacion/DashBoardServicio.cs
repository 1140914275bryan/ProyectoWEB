using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.BLL.Implementacion
{
    public class DashBoardServicio : IDashBoardService
    {
        private readonly IVentaRepositorio _repositorioVenta;
        private readonly IRepositorioGenerico<DetalleVenta> _repositorioDetalleVenta;
        private readonly IRepositorioGenerico<Categoria> _repositorioCategoria;
        private readonly IRepositorioGenerico<Producto> _repositorioProducto;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardServicio(IVentaRepositorio repositorioVenta,
            IRepositorioGenerico<DetalleVenta> repositorioDetalleVenta,
            IRepositorioGenerico<Categoria> repositorioCategoria,
            IRepositorioGenerico<Producto> repositorioProducto)
        {

            _repositorioVenta = repositorioVenta;
            _repositorioDetalleVenta = repositorioDetalleVenta;
            _repositorioCategoria = repositorioCategoria;
            _repositorioProducto = repositorioProducto;

            FechaInicio = FechaInicio.AddDays(-30);
        }

        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                // obtener todas las ventas registradas despues de la fecha de inicio
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date); 
                int total = query.Count(); // cuenta las ventas que superan la fecha de inicio
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                // obtener todas las ventas registradas despues de la fecha de inicio
                IQueryable<Venta> query = await _repositorioVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                decimal resultado = query.Select(v => v.Total).Sum(v => v.Value); // se selecciona la columna y se suman los valores
                return Convert.ToString(resultado,new CultureInfo("es-CO"));
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalProductos()
        {
            try
            {
                // obtener todos los productos
                IQueryable<Producto> query = await _repositorioProducto.Consultar();
                int total = query.Count(); // cuenta los productos reigstrados
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalCategorias()
        {
            try
            {
                // obtener todas las categorias
                IQueryable<Categoria> query = await _repositorioCategoria.Consultar();
                int total = query.Count(); // cuenta las categorias registradas
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            { 
                IQueryable<Venta> query = await _repositorioVenta
                    .Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                // se agrupa el reporte de ventas atravez de la columna fecha de registro
                // de una manera decendiente
                // luego se crea un nuevo objeto con select que tiene la propiedad de fecha y total
                // en la fecha se envia el valor de la key( la agrupacion )
                // y en el total se hace un conteo por que fecha que encuentra dentro de la agrupacion
                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)

                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> ProductosTopUltimaSemana()
        {
            try
            {    // obtiene todos los detalles de venta
                IQueryable<DetalleVenta> query = await _repositorioDetalleVenta.Consultar();

                // detalle venta no tiene venta pero si venta, entonces por eso se incluye
                // para luego hacer un filtro a esa tabla de venta
                // se ordena de forma decendiente mostrando el producto mas vendido como primero etc

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date)
                    .GroupBy(dv => dv.DescripcionProducto).OrderByDescending(g => g.Count())

                    .Select(dv => new { producto = dv.Key, total = dv.Count()}).Take(4)
                    .ToDictionary(keySelector: r => r.producto, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }
    }
}
