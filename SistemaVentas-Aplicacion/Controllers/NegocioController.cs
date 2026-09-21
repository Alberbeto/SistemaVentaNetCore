using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class NegocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
