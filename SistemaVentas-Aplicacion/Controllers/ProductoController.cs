using Microsoft.AspNetCore.Mvc;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
