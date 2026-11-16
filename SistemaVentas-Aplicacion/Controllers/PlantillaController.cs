using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.NativeInterop;
using AutoMapper;
using SistemaVentas_Aplicacion.Models.VModels;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IVentaService _ventaService;
        public PlantillaController(IMapper mapper, INegocioService negocioService, IVentaService ventaService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            _ventaService = ventaService;
        }
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["correo"] = correo;
            ViewData["clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }


        public async Task<IActionResult> PDFVenta(string numeroVenta)
        {
            VMVenta venta  = _mapper.Map<VMVenta>(await _ventaService.Detalle(numeroVenta));
            VMNegocio negocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
            VMPDFVenta vMPDFVenta = new VMPDFVenta();

            vMPDFVenta.venta = venta;
            vMPDFVenta.negocio = negocio;

            return View(vMPDFVenta);
        }


        public IActionResult ReestablecerClave(string clave)
        {
            
            ViewData["clave"] = clave;
            return View();
        }
    }
}
