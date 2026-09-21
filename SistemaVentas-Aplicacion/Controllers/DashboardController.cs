using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
