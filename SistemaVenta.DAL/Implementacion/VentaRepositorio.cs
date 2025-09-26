using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.DAL.DBContext;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepositorio : RepositorioGenerico<Venta>, IVentaRepositorio // hereda de RepositorioGenerico y de IVentaRepositorio
    {

        private readonly DbventaContext _dbContext; // contexto de la base de datos
        public VentaRepositorio(DbventaContext dbContext) : base(dbContext) // llama al constructor de la clase base RepositorioGenerico
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); // si el dbContext es nulo, lanza una excepcion
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta(); // se crea una nueva instancia de Venta para registrar la venta

                using (var transaction = _dbContext.Database.BeginTransaction()) // inicia una transaccion para asegurar que todas las operaciones se realicen correctamente
                {
                    try
                    {
                        foreach (DetalleVenta dv in entidad.DetalleVenta) // recorre cada detalle de la venta
                        {
                            Producto producto_encontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First(); // busca el producto en la base de datos
                            producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad; // actualiza el stock del producto restando la cantidad vendida
                            _dbContext.Productos.Update(producto_encontrado); // actualiza el producto en la base de datos
                        }
                        await _dbContext.SaveChangesAsync(); // guarda los cambios de forma asincrona

                        NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First(); // obtiene el numero correlativo para la venta 00 - 01 etc
                        correlativo.UltimoNumero = correlativo.UltimoNumero + 1; // incrementa el numero correlativo en +1
                        correlativo.FechaActualizacion = DateTime.Now; // actualiza la fecha de la ultima actualizacion

                        _dbContext.NumeroCorrelativos.Update(correlativo); // actualiza el numero correlativo en la base de datos
                        await _dbContext.SaveChangesAsync(); // guarda los cambios de forma asincrona

                        string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value)); // genera una cadena de ceros segun la cantidad de digitos del numero correlativo
                        string numeroVenta = ceros + correlativo.UltimoNumero.ToString(); // concatena los ceros con el ultimo numero
                        numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value,correlativo.CantidadDigitos.Value); // obtiene solo la cantidad de digitos del numero correlativo

                        entidad.NumeroVenta = numeroVenta; // asigna el numero de venta a la entidad
                        await _dbContext.Venta.AddRangeAsync(entidad); // agrega todo el objeto de venta a la base de datos
                        await _dbContext.SaveChangesAsync(); // guarda los cambios de forma asincrona

                        transaction.Commit(); // Si pasa todas las operaciones , se confirma la transaccion

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // si ocurre un error
                        throw ex; // lanza la excepcion para que sea manejada por el controlador
                    }
                }
            return ventaGenerada; // retorna la venta generada
         }

        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List< DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation) // incluye la entidad de venta relacionada
                .ThenInclude(u => u.IdUsuarioNavigation) // incluye la entidad de usuario relacionada
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date &&
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date).ToListAsync(); // filtra por fecha de registro

            return listaResumen;
        }
    }
}
