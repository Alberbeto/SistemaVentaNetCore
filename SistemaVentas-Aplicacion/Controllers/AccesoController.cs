using Microsoft.AspNetCore.Mvc;

using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;


using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace SistemaVentas_Aplicacion.Controllers
{
   
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;

        public AccesoController(IUsuarioService usuarioService) {

            _usuarioServicio = usuarioService;
        
        }

        
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Restablecer()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Restablecer(VMUsuarioLogin modelo)
        {
            try
            {
                string UrlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/ReestablecerClave?clave=[clave]";

                bool resultado = await _usuarioServicio.RestablecerClave(modelo.Correo, UrlPlantillaCorreo);

                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue reestablecida, Revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = "Tenemos Problemas. Por favor intenelo denuevo mas tarde";
                    ViewData["MensajeError"] = null;
                }
            }catch(Exception ex)
            {
                ViewData["Mensaje"] = ex.Message;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMUsuarioLogin modelo)
        {
            Usuario usuario_econtrado = await _usuarioServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);

            if(usuario_econtrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            
            }

            ViewData["Mensaje"] = null;

            // guardar informacon del usuario
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_econtrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_econtrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_econtrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_econtrado.UrlFoto),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);



            return RedirectToAction("Index","Home");
        }

    }
}
