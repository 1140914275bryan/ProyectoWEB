using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User; // Obtiene el usuario autenticado
            string nombreUsuario = "";
            string urlFotoUsuario = "";

            if(claimUser.Identity.IsAuthenticated)
            {
                nombreUsuario = claimUser.Claims.Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault(); // Obtiene el nombre de usuario

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value; // Obtiene la URL de la foto del usuario
            }
            ViewData["nombreUsuario"] = nombreUsuario; // Asigna el nombre de usuario a ViewData
            ViewData["urlFotoUsuario"] = urlFotoUsuario; // Asigna la URL de la foto del usuario a ViewData
            return View();
        }
    }
}
