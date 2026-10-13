using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using AutoMapper;
using SistemaVentas_Aplicacion.Utilidades.Response;
using SistemaVentas_Aplicacion.Models.VModels;
using Newtonsoft.Json;

namespace SistemaVentas_Aplicacion.Controllers
{
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoVentaService _tiposervice;
        private readonly IVentaService _ventareservice;
        private readonly IMapper _mapper;


        public VentaController(ITipoDocumentoVentaService tiposervice, IMapper mapper, IVentaService ventareservice)
        {
            _tiposervice = tiposervice;
            _mapper = mapper;
            _ventareservice = ventareservice;
        }
        public IActionResult NuevaVenta()
        {
            return View();
        }


        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDetalleVenta()
        {
            try
            {
                List<TipoDocumentoVenta> queryListaTipoDocumento = await _tiposervice.Lista();
                return StatusCode(StatusCodes.Status200OK, new { data = queryListaTipoDocumento });

            }catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost]

        public async Task<IActionResult> RegistrarVenta([FromForm] string modelo)
        {
            GenericResponse<VMVenta> gresponse = new GenericResponse<VMVenta>();

            try
            {
                VMVenta vmVenta = JsonConvert.DeserializeObject<VMVenta>(modelo);
               Venta Venta = await _ventareservice.Registar(_mapper.Map<Venta>(vmVenta));

                vmVenta = _mapper.Map<VMVenta>(Venta);

                gresponse.Estado = true;
                gresponse.Objeto = vmVenta;

            }
            catch(Exception ex)
            {
                gresponse.Estado = false;
                gresponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gresponse);
        }
    }
}
