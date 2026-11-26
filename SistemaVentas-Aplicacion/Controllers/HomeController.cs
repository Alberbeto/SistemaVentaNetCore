using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVentas_Aplicacion.Models;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVentas_Aplicacion.Utilidades.Response;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;



namespace TuProyecto.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
            IUsuarioService usuarioService,
            IMapper mapper)
        {
            _logger = logger;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> gresponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claims = HttpContext.User;

                string idUsuario = claims.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                VMUsuario vmUsuario = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerPorId(int.Parse(idUsuario)));

                gresponse.Estado = true;
                gresponse.Objeto = vmUsuario;

            }catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuarPerfil([FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gresponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claims = HttpContext.User;

                string idUsuario = claims.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                VMUsuario vmcategoria = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                Usuario entidad = _mapper.Map<Usuario>(vmcategoria);

                entidad.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usuarioService.GuardarPerfil(entidad);

                gresponse.Estado = resultado;
                

            }
            catch (Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }


        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> gresponse = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claims = HttpContext.User;

                string idUsuario = claims.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                bool resultado = await _usuarioService.CambiarClave(int.Parse(idUsuario), modelo.ClaveActual, modelo.ClaveNueva);

                gresponse.Estado = resultado;


            }
            catch (Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gresponse);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }
    }
}