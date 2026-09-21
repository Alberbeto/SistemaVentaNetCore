using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.NativeInterop;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["correo"] = correo;
            ViewData["clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }


        public IActionResult ReestablecerClave(string clave)
        {
            
            ViewData["clave"] = clave;
            return View();
        }
    }
}
