using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVentas_Aplicacion.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsusers = HttpContext.User;

            string nombreUsuario = "";
            string urlFotoUsuario = "";

            if (claimsusers.Identity.IsAuthenticated)
            {
                nombreUsuario = claimsusers.Claims
                    .Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimsusers.Identity).FindFirst("UrlFoto").Value;
            }

            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["urlFotoUsuario"] = urlFotoUsuario;

            return View();
        }
    }
}
