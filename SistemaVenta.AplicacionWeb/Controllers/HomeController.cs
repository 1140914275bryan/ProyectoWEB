using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Diagnostics;
using System.Security.Claims;


namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IUsuarioService _usuarioServicio;
        private readonly IMapper _mapper;

        public HomeController(IUsuarioService usuarioServicio, IMapper mapper)
        {
            _usuarioServicio = usuarioServicio;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Cierra la sesión del usuario autenticado
            return RedirectToAction("Login", "Acceso"); // Redirige al usuario a la página de inicio de sesión
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User; // Obtiene el usuario autenticado
                string idUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioServicio.ObtenerPorId(int.Parse(idUsuario))); // Obtiene el usuario por su ID

                response.Estado = true;
                response.Objeto = usuario; // Asigna el usuario al objeto de respuesta

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response); // Devuelve el estado 200 OK con la respuesta
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User; // Obtiene el usuario autenticado
                string idUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo); // Mapea el modelo a la entidad Usuario
                entidad.IdUsuario = int.Parse(idUsuario); // Asigna el ID del usuario autenticado

                bool resultado = await _usuarioServicio.GuardarPerfil(entidad); // Guarda el perfil del usuario

                response.Estado = resultado;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response); // Devuelve el estado 200 OK con la respuesta
        }


    }
}
