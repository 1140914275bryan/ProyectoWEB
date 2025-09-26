using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IServicioNegocio _negocioServicio;
        private readonly IVentaServicio _ventaServicio;

        public PlantillaController(IMapper mapper, IServicioNegocio negocioServicio, IVentaServicio ventaServicio)
        {
            _mapper = mapper;
            _negocioServicio = negocioServicio;
            _ventaServicio = ventaServicio;
        }

        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";

            return View();
        }
        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }

        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VMVenta vMVenta = _mapper.Map<VMVenta>(await _ventaServicio.Detalle(numeroVenta));
            VMNegocio vMNegocio = _mapper.Map<VMNegocio>(await _negocioServicio.Obtener());
            VMPDFVenta modelo = new VMPDFVenta();

            modelo.negocio = vMNegocio;
            modelo.venta = vMVenta;


            return View(modelo);
        }
    }
}
