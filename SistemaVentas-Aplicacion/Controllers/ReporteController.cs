using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
