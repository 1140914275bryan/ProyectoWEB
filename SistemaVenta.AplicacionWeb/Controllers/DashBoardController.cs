using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardServicio;
        private readonly IVentaServicio _ventaServicio;

        public DashBoardController(IDashBoardService dashboardServicio, IVentaServicio ventaServicio)
        {
            _dashboardServicio = dashboardServicio;
            _ventaServicio = ventaServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();

            try
            {
                VMDashBoard vMDashBoard = new VMDashBoard
                {
                    TotalVentas = await _dashboardServicio.TotalVentasUltimaSemana(),
                    TotalIngresos = await _dashboardServicio.TotalIngresosUltimaSemana(),
                    TotalProductos = await _dashboardServicio.TotalProductos(),
                    TotalCategorias = await _dashboardServicio.TotalCategorias(),
                    VentasUltimaSemana = (await _dashboardServicio.VentasUltimaSemana())
                        .Select(item => new VMVentasSemana
                        {
                            Fecha = item.Key,
                            Total = item.Value
                        }).ToList(),
                    ProductosTopUltimaSemana = (await _dashboardServicio.ProductosTopUltimaSemana())
                        .Select(item => new VMProductosSemana
                        {
                            Producto = item.Key,
                            Cantidad = item.Value
                        }).ToList()
                };

                gResponse.Estado = true;
                gResponse.Objeto = vMDashBoard;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleVenta(int idVenta)
        {
            try
            {
                var detalle = await _ventaServicio.ObtenerDetalleVentaPorId(idVenta); // corregido

                if (detalle == null || !detalle.Any())
                {
                    return Json(new { estado = false, mensaje = "Venta no encontrada o sin productos" });
                }

                var resultado = detalle
                    .GroupBy(d => d.DescripcionProducto)
                    .Select(g => new
                    {
                        producto = g.Key,
                        cantidad = g.Sum(x => x.Cantidad)
                    }).ToList();

                return Json(new { estado = true, objeto = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { estado = false, mensaje = ex.Message });
            }
        }
    }
}
